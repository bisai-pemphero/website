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
[Route("api/student-promotion")]
public sealed class StudentPromotionController(SqlDatabase database) : ControllerBase
{
    [HttpGet("classes")]
    public async Task<IActionResult> GetClasses(CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        var classes = await database.QueryAsync(
            @"SELECT ClassID AS Id, ClassName 
              FROM Classes 
              WHERE SchoolId = @SchoolId 
              ORDER BY ClassName",
            cancellationToken,
            new SqlParameter("@SchoolId", schoolId));

        // Add Graduate option
        var result = new List<dynamic>(classes);
        result.Insert(0, new { Id = 0, ClassName = "Graduate" });

        return Ok(result);
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudentsByClass([FromQuery] int classId, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        if (classId < 0)
        {
            return BadRequest(new { message = "A valid class ID is required." });
        }

        string query;
        SqlParameter[] parameters;

        if (classId == 0)
        {
            // Special case: Get graduated students (for moving back to active)
            query = @"SELECT StudentID, FirstName, Middlename, LastName, CurrentClassID, PreviousclassID, Status
                      FROM Students 
                      WHERE SchoolId = @SchoolId AND Status = 'Graduated' AND IsDeleted = 0
                      ORDER BY LastName, FirstName";
            parameters = new[] { new SqlParameter("@SchoolId", schoolId) };
        }
        else
        {
            query = @"SELECT StudentID, FirstName, Middlename, LastName, CurrentClassID, PreviousclassID, Status
                      FROM Students 
                      WHERE SchoolId = @SchoolId AND CurrentClassID = @ClassId AND Status = 'Active' AND IsDeleted = 0
                      ORDER BY LastName, FirstName";
            parameters = new[] 
            { 
                new SqlParameter("@SchoolId", schoolId),
                new SqlParameter("@ClassId", classId)
            };
        }

        return Ok(await database.QueryAsync(query, cancellationToken, parameters));
    }

    [HttpPost("promote")]
    public async Task<IActionResult> PromoteStudents(PromoteStudentsRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        if (request.StudentIds == null || request.StudentIds.Length == 0)
        {
            return BadRequest(new { message = "At least one student must be selected." });
        }

        if (request.NewClassId < 0)
        {
            return BadRequest(new { message = "A valid new class must be selected." });
        }

        // Verify the new class belongs to the school (unless it's 0 for graduation)
        if (request.NewClassId > 0)
        {
            var classCheck = await database.QueryAsync(
                "SELECT ClassID FROM Classes WHERE ClassID = @ClassId AND SchoolId = @SchoolId",
                cancellationToken,
                new SqlParameter("@ClassId", request.NewClassId),
                new SqlParameter("@SchoolId", schoolId));

            if (classCheck.Count == 0)
            {
                return NotFound(new { message = "New class not found in your school." });
            }
        }

        // Build list of student IDs for SQL IN clause
        var studentIdParams = new List<SqlParameter>();
        var studentIdPlaceholders = new List<string>();
        
        for (int i = 0; i < request.StudentIds.Length; i++)
        {
            var paramName = $"@StudentId{i}";
            studentIdParams.Add(new SqlParameter(paramName, request.StudentIds[i]));
            studentIdPlaceholders.Add(paramName);
        }

        // First, get current class info for all students to use as previous class
        var studentCurrentClasses = await database.QueryAsync(
            $"SELECT StudentID, CurrentClassID FROM Students WHERE StudentID IN ({string.Join(",", studentIdPlaceholders)}) AND SchoolId = @SchoolId AND IsDeleted = 0",
            cancellationToken,
            [...studentIdParams, new SqlParameter("@SchoolId", schoolId)]);

        if (studentCurrentClasses.Count == 0)
        {
            return NotFound(new { message = "No valid students found for promotion." });
        }

        try
        {
            using var connection = new SqlConnection(database.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            foreach (var student in studentCurrentClasses)
            {
                var studentId = (int)student["StudentID"];
                var currentClassId = student["CurrentClassID"] != DBNull.Value ? (int)student["CurrentClassID"] : 0;
                
                string updateQuery;
                string status;

                if (request.NewClassId == 0)
                {
                    // Graduating the student
                    updateQuery = @"UPDATE Students 
                                   SET CurrentClassID = @NewClassId, 
                                       PreviousclassID = @PreviousClassId, 
                                       Status = 'Graduated' 
                                   WHERE StudentID = @StudentId AND SchoolId = @SchoolId";
                    status = "Graduated";
                }
                else
                {
                    // Promoting to new class
                    updateQuery = @"UPDATE Students 
                                   SET CurrentClassID = @NewClassId, 
                                       PreviousclassID = @PreviousClassId, 
                                       Status = 'Active' 
                                   WHERE StudentID = @StudentId AND SchoolId = @SchoolId";
                    status = "Active";
                }

                using var command = new SqlCommand(updateQuery, connection, transaction);
                command.Parameters.AddWithValue("@NewClassId", request.NewClassId == 0 ? (object)DBNull.Value : request.NewClassId);
                command.Parameters.AddWithValue("@PreviousClassId", currentClassId == 0 ? (object)DBNull.Value : currentClassId);
                command.Parameters.AddWithValue("@StudentId", studentId);
                command.Parameters.AddWithValue("@SchoolId", schoolId);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            return Ok(new
            {
                success = true,
                message = $"{studentCurrentClasses.Count} student(s) successfully promoted.",
                promotedCount = studentCurrentClasses.Count,
                newClassId = request.NewClassId,
                newStatus = request.NewClassId == 0 ? "Graduated" : "Active"
            });
        }
        catch (SqlException ex)
        {
            return Problem($"Failed to promote students: {ex.Message}", statusCode: 500);
        }
    }

    [HttpPost("bulk-change-class")]
    public async Task<IActionResult> BulkChangeClass(BulkClassChangeRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        if (request.StudentIds == null || request.StudentIds.Length == 0)
        {
            return BadRequest(new { message = "At least one student must be selected." });
        }

        if (request.NewClassId <= 0)
        {
            return BadRequest(new { message = "A valid new class must be selected." });
        }

        // Verify the new class belongs to the school
        var classCheck = await database.QueryAsync(
            "SELECT ClassID FROM Classes WHERE ClassID = @ClassId AND SchoolId = @SchoolId",
            cancellationToken,
            new SqlParameter("@ClassId", request.NewClassId),
            new SqlParameter("@SchoolId", schoolId));

        if (classCheck.Count == 0)
        {
            return NotFound(new { message = "New class not found in your school." });
        }

        // Build list of student IDs for SQL IN clause
        var studentIdParams = new List<SqlParameter>();
        var studentIdPlaceholders = new List<string>();
        
        for (int i = 0; i < request.StudentIds.Length; i++)
        {
            var paramName = $"@StudentId{i}";
            studentIdParams.Add(new SqlParameter(paramName, request.StudentIds[i]));
            studentIdPlaceholders.Add(paramName);
        }

        // Get current class info for all students
        var studentCurrentClasses = await database.QueryAsync(
            $"SELECT StudentID, CurrentClassID FROM Students WHERE StudentID IN ({string.Join(",", studentIdPlaceholders)}) AND SchoolId = @SchoolId AND IsDeleted = 0",
            cancellationToken,
            [...studentIdParams, new SqlParameter("@SchoolId", schoolId)]);

        if (studentCurrentClasses.Count == 0)
        {
            return NotFound(new { message = "No valid students found." });
        }

        try
        {
            using var connection = new SqlConnection(database.ConnectionString);
            await connection.OpenAsync(cancellationToken);
            using var transaction = await connection.BeginTransactionAsync(cancellationToken);

            foreach (var student in studentCurrentClasses)
            {
                var studentId = (int)student["StudentID"];
                var currentClassId = student["CurrentClassID"] != DBNull.Value ? (int)student["CurrentClassID"] : 0;
                
                var updateQuery = @"UPDATE Students 
                                   SET CurrentClassID = @NewClassId, 
                                       PreviousclassID = @PreviousClassId, 
                                       Status = 'Active' 
                                   WHERE StudentID = @StudentId AND SchoolId = @SchoolId";

                using var command = new SqlCommand(updateQuery, connection, transaction);
                command.Parameters.AddWithValue("@NewClassId", request.NewClassId);
                command.Parameters.AddWithValue("@PreviousClassId", currentClassId == 0 ? (object)DBNull.Value : currentClassId);
                command.Parameters.AddWithValue("@StudentId", studentId);
                command.Parameters.AddWithValue("@SchoolId", schoolId);

                await command.ExecuteNonQueryAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);

            return Ok(new
            {
                success = true,
                message = $"{studentCurrentClasses.Count} student(s) successfully changed to new class.",
                changedCount = studentCurrentClasses.Count,
                newClassId = request.NewClassId
            });
        }
        catch (SqlException ex)
        {
            return Problem($"Failed to change student class: {ex.Message}", statusCode: 500);
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

// Request DTOs
public class PromoteStudentsRequest
{
    public int[] StudentIds { get; set; } = Array.Empty<int>();
    public int NewClassId { get; set; } // 0 means Graduate
}

public class BulkClassChangeRequest
{
    public int[] StudentIds { get; set; } = Array.Empty<int>();
    public int NewClassId { get; set; }
}
