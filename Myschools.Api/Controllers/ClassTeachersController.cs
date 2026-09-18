using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Myschools.Api.Data;

namespace Myschools.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/class-teachers")]
public sealed class ClassTeachersController(SqlDatabase database) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetClassTeachers(CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        return Ok(await database.QueryAsync(
            @"SELECT FT.FormTeacherId, FT.TeacherId, FT.TeacherName, FT.ClassId, FT.SchoolId, C.ClassName 
              FROM FormTeachers FT 
              INNER JOIN Classes C ON C.ClassID = FT.ClassId 
              WHERE FT.SchoolId = @SchoolId 
              ORDER BY C.ClassName",
            cancellationToken,
            new SqlParameter("@SchoolId", schoolId)));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetClassTeacher(int id, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        if (id <= 0)
        {
            return BadRequest(new { message = "A valid class teacher assignment ID is required." });
        }

        var assignments = await database.QueryAsync(
            @"SELECT FT.FormTeacherId, FT.TeacherId, FT.TeacherName, FT.ClassId, FT.SchoolId, C.ClassName 
              FROM FormTeachers FT 
              INNER JOIN Classes C ON C.ClassID = FT.ClassId 
              WHERE FT.FormTeacherId = @Id AND FT.SchoolId = @SchoolId",
            cancellationToken,
            new SqlParameter("@Id", id),
            new SqlParameter("@SchoolId", schoolId));

        if (assignments.Count == 0)
        {
            return NotFound(new { message = "Class teacher assignment not found." });
        }

        return Ok(assignments[0]);
    }

    [HttpPost]
    public async Task<IActionResult> AssignClassTeacher(TeacherAssignmentRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        if (request.ClassId <= 0 || request.TeacherId <= 0 || string.IsNullOrWhiteSpace(request.TeacherName))
        {
            return BadRequest(new { message = "Class, teacher, and teacher name are required." });
        }

        // Verify class belongs to school
        var classCheck = await database.QueryAsync(
            "SELECT ClassID FROM Classes WHERE ClassID = @ClassId AND SchoolId = @SchoolId",
            cancellationToken,
            new SqlParameter("@ClassId", request.ClassId),
            new SqlParameter("@SchoolId", schoolId));

        if (classCheck.Count == 0)
        {
            return NotFound(new { message = "Class not found in your school." });
        }

        // Check if class already has a teacher assigned
        var existingAssignment = await database.QueryAsync(
            "SELECT FormTeacherId FROM FormTeachers WHERE ClassId = @ClassId AND SchoolId = @SchoolId",
            cancellationToken,
            new SqlParameter("@ClassId", request.ClassId),
            new SqlParameter("@SchoolId", schoolId));

        if (existingAssignment.Count > 0)
        {
            return Conflict(new { message = "A class teacher is already assigned to this class. Please update the assignment instead." });
        }

        try
        {
            await database.ExecuteAsync(
                @"INSERT INTO FormTeachers (TeacherId, TeacherName, ClassId, SchoolId) 
                  VALUES (@TeacherId, @TeacherName, @ClassId, @SchoolId)",
                cancellationToken,
                new SqlParameter("@TeacherId", request.TeacherId),
                new SqlParameter("@TeacherName", request.TeacherName.Trim()),
                new SqlParameter("@ClassId", request.ClassId),
                new SqlParameter("@SchoolId", schoolId));

            return StatusCode(201, new
            {
                success = true,
                message = "Class teacher assigned successfully."
            });
        }
        catch (SqlException ex)
        {
            return Problem($"Failed to assign class teacher: {ex.Message}", statusCode: 500);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateClassTeacher(int id, TeacherAssignmentRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        if (id <= 0)
        {
            return BadRequest(new { message = "A valid assignment ID is required." });
        }

        if (request.ClassId <= 0 || request.TeacherId <= 0 || string.IsNullOrWhiteSpace(request.TeacherName))
        {
            return BadRequest(new { message = "Class, teacher, and teacher name are required." });
        }

        // Verify assignment exists and belongs to school
        var existingAssignment = await database.QueryAsync(
            "SELECT FormTeacherId FROM FormTeachers WHERE FormTeacherId = @Id AND SchoolId = @SchoolId",
            cancellationToken,
            new SqlParameter("@Id", id),
            new SqlParameter("@SchoolId", schoolId));

        if (existingAssignment.Count == 0)
        {
            return NotFound(new { message = "Class teacher assignment not found." });
        }

        // Verify class belongs to school
        var classCheck = await database.QueryAsync(
            "SELECT ClassID FROM Classes WHERE ClassID = @ClassId AND SchoolId = @SchoolId",
            cancellationToken,
            new SqlParameter("@ClassId", request.ClassId),
            new SqlParameter("@SchoolId", schoolId));

        if (classCheck.Count == 0)
        {
            return NotFound(new { message = "Class not found in your school." });
        }

        // Check if another class already has this teacher (optional business rule)
        // Uncomment if you want to prevent a teacher from being assigned to multiple classes
        /*
        var duplicateTeacher = await database.QueryAsync(
            "SELECT FormTeacherId FROM FormTeachers WHERE TeacherId = @TeacherId AND FormTeacherId <> @Id AND SchoolId = @SchoolId",
            cancellationToken,
            new SqlParameter("@TeacherId", request.TeacherId),
            new SqlParameter("@Id", id),
            new SqlParameter("@SchoolId", schoolId));

        if (duplicateTeacher.Count > 0)
        {
            return Conflict(new { message = "This teacher is already assigned to another class." });
        }
        */

        try
        {
            var rowsAffected = await database.ExecuteAsync(
                @"UPDATE FormTeachers 
                  SET TeacherId = @TeacherId, 
                      TeacherName = @TeacherName, 
                      ClassId = @ClassId 
                  WHERE FormTeacherId = @Id AND SchoolId = @SchoolId",
                cancellationToken,
                new SqlParameter("@TeacherId", request.TeacherId),
                new SqlParameter("@TeacherName", request.TeacherName.Trim()),
                new SqlParameter("@ClassId", request.ClassId),
                new SqlParameter("@Id", id),
                new SqlParameter("@SchoolId", schoolId));

            if (rowsAffected > 0)
            {
                return Ok(new
                {
                    success = true,
                    message = "Class teacher updated successfully.",
                    id = id
                });
            }

            return NotFound(new { message = "Assignment not found." });
        }
        catch (SqlException ex)
        {
            return Problem($"Failed to update class teacher: {ex.Message}", statusCode: 500);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteClassTeacher(int id, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        if (id <= 0)
        {
            return BadRequest(new { message = "A valid assignment ID is required." });
        }

        try
        {
            var rowsAffected = await database.ExecuteAsync(
                "DELETE FROM FormTeachers WHERE FormTeacherId = @Id AND SchoolId = @SchoolId",
                cancellationToken,
                new SqlParameter("@Id", id),
                new SqlParameter("@SchoolId", schoolId));

            if (rowsAffected > 0)
            {
                return Ok(new
                {
                    success = true,
                    message = "Class teacher assignment removed successfully.",
                    id = id
                });
            }

            return NotFound(new { message = "Assignment not found." });
        }
        catch (SqlException ex)
        {
            return Problem($"Failed to delete class teacher assignment: {ex.Message}", statusCode: 500);
        }
    }

    private bool TryGetSchoolId(out int schoolId)
    {
        if (int.TryParse(User.FindFirst("schoolId")?.Value, out schoolId))
        {
            return schoolId > 0;
        }
        return false;
    }
}
