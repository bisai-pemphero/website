using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Myschools.Api.Data;
using Myschools.Api.Models;

namespace Myschools.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/grading-system")]
public sealed class GradingSystemController(SqlDatabase database) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetGrades([FromQuery] string? level, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        string whereClause = "WHERE SchoolId = @SchoolId";
        var parameters = new List<SqlParameter> { new SqlParameter("@SchoolId", schoolId) };

        if (!string.IsNullOrWhiteSpace(level))
        {
            whereClause += " AND Level = @Level";
            parameters.Add(new SqlParameter("@Level", level.Trim()));
        }

        return Ok(await database.QueryAsync(
            $"SELECT Id, Grade, Minimum_Mark, Maxmum_Mark, Remark, Level, SchoolId FROM GradingSystem {whereClause} ORDER BY Minimum_Mark",
            cancellationToken,
            parameters.ToArray()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetGrade(int id, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        if (id <= 0)
        {
            return BadRequest(new { message = "A valid grade ID is required." });
        }

        var grades = await database.QueryAsync(
            "SELECT Id, Grade, Minimum_Mark, Maxmum_Mark, Remark, Level, SchoolId FROM GradingSystem WHERE Id = @Id AND SchoolId = @SchoolId",
            cancellationToken,
            new SqlParameter("@Id", id),
            new SqlParameter("@SchoolId", schoolId));

        if (grades.Count == 0)
        {
            return NotFound(new { message = "Grade not found." });
        }

        return Ok(grades[0]);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGrade(GradingSystemRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        var validationError = ValidateGrade(request);
        if (validationError != null)
        {
            return BadRequest(new { message = validationError });
        }

        // Check for duplicates
        var existingGrades = await database.QueryAsync(
            @"SELECT Id FROM GradingSystem 
              WHERE Grade = @Grade 
              AND Minimum_Mark = @MinimumMark 
              AND Maxmum_Mark = @MaximumMark 
              AND Remark = @Remark 
              AND Level = @Level 
              AND SchoolId = @SchoolId",
            cancellationToken,
            new SqlParameter("@Grade", request.Grade.Trim()),
            new SqlParameter("@MinimumMark", request.MinimumMark),
            new SqlParameter("@MaximumMark", request.MaximumMark),
            new SqlParameter("@Remark", request.Remark.Trim()),
            new SqlParameter("@Level", request.Level?.Trim() ?? ""),
            new SqlParameter("@SchoolId", schoolId));

        if (existingGrades.Count > 0)
        {
            return Conflict(new { message = "This grading system already exists. Please update instead." });
        }

        // Check for overlapping marks
        var overlappingGrades = await database.QueryAsync(
            @"SELECT Id FROM GradingSystem 
              WHERE SchoolId = @SchoolId 
              AND Level = @Level 
              AND ((@MinimumMark BETWEEN Minimum_Mark AND Maxmum_Mark) 
                   OR (@MaximumMark BETWEEN Minimum_Mark AND Maxmum_Mark)
                   OR (Minimum_Mark BETWEEN @MinimumMark AND @MaximumMark)
                   OR (Maxmum_Mark BETWEEN @MinimumMark AND @MaximumMark))",
            cancellationToken,
            new SqlParameter("@MinimumMark", request.MinimumMark),
            new SqlParameter("@MaximumMark", request.MaximumMark),
            new SqlParameter("@Level", request.Level?.Trim() ?? ""),
            new SqlParameter("@SchoolId", schoolId));

        if (overlappingGrades.Count > 0)
        {
            return Conflict(new { message = "The mark range overlaps with an existing grade. Please adjust the range." });
        }

        try
        {
            var result = await database.QueryAsync(
                @"INSERT INTO GradingSystem (Grade, Minimum_Mark, Maxmum_Mark, Remark, SchoolId, Level) 
                  OUTPUT INSERTED.Id AS Id 
                  VALUES (@Grade, @MinimumMark, @MaximumMark, @Remark, @SchoolId, @Level)",
                cancellationToken,
                new SqlParameter("@Grade", request.Grade.Trim()),
                new SqlParameter("@MinimumMark", request.MinimumMark),
                new SqlParameter("@MaximumMark", request.MaximumMark),
                new SqlParameter("@Remark", request.Remark.Trim()),
                new SqlParameter("@SchoolId", schoolId),
                new SqlParameter("@Level", request.Level?.Trim() ?? ""));

            return StatusCode(201, new
            {
                success = true,
                message = "Grading system created successfully.",
                id = result[0]["Id"]
            });
        }
        catch (SqlException ex)
        {
            return Problem($"Failed to create grading system: {ex.Message}", statusCode: 500);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateGrade(int id, GradingSystemRequest request, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        if (id <= 0)
        {
            return BadRequest(new { message = "A valid grade ID is required." });
        }

        var validationError = ValidateGrade(request);
        if (validationError != null)
        {
            return BadRequest(new { message = validationError });
        }

        // Check if grade exists
        var existingGrade = await database.QueryAsync(
            "SELECT Id FROM GradingSystem WHERE Id = @Id AND SchoolId = @SchoolId",
            cancellationToken,
            new SqlParameter("@Id", id),
            new SqlParameter("@SchoolId", schoolId));

        if (existingGrade.Count == 0)
        {
            return NotFound(new { message = "Grade not found." });
        }

        // Check for duplicates (excluding current record)
        var duplicateGrades = await database.QueryAsync(
            @"SELECT Id FROM GradingSystem 
              WHERE Grade = @Grade 
              AND Minimum_Mark = @MinimumMark 
              AND Maxmum_Mark = @MaximumMark 
              AND Remark = @Remark 
              AND Level = @Level 
              AND SchoolId = @SchoolId
              AND Id <> @Id",
            cancellationToken,
            new SqlParameter("@Grade", request.Grade.Trim()),
            new SqlParameter("@MinimumMark", request.MinimumMark),
            new SqlParameter("@MaximumMark", request.MaximumMark),
            new SqlParameter("@Remark", request.Remark.Trim()),
            new SqlParameter("@Level", request.Level?.Trim() ?? ""),
            new SqlParameter("@SchoolId", schoolId),
            new SqlParameter("@Id", id));

        if (duplicateGrades.Count > 0)
        {
            return Conflict(new { message = "Another grading system with these details already exists." });
        }

        // Check for overlapping marks (excluding current record)
        var overlappingGrades = await database.QueryAsync(
            @"SELECT Id FROM GradingSystem 
              WHERE SchoolId = @SchoolId 
              AND Level = @Level 
              AND Id <> @Id
              AND ((@MinimumMark BETWEEN Minimum_Mark AND Maxmum_Mark) 
                   OR (@MaximumMark BETWEEN Minimum_Mark AND Maxmum_Mark)
                   OR (Minimum_Mark BETWEEN @MinimumMark AND @MaximumMark)
                   OR (Maxmum_Mark BETWEEN @MinimumMark AND @MaximumMark))",
            cancellationToken,
            new SqlParameter("@MinimumMark", request.MinimumMark),
            new SqlParameter("@MaximumMark", request.MaximumMark),
            new SqlParameter("@Level", request.Level?.Trim() ?? ""),
            new SqlParameter("@SchoolId", schoolId),
            new SqlParameter("@Id", id));

        if (overlappingGrades.Count > 0)
        {
            return Conflict(new { message = "The mark range overlaps with an existing grade. Please adjust the range." });
        }

        try
        {
            var rowsAffected = await database.ExecuteAsync(
                @"UPDATE GradingSystem 
                  SET Grade = @Grade, 
                      Minimum_Mark = @MinimumMark, 
                      Maxmum_Mark = @MaximumMark, 
                      Remark = @Remark, 
                      Level = @Level 
                  WHERE Id = @Id AND SchoolId = @SchoolId",
                cancellationToken,
                new SqlParameter("@Grade", request.Grade.Trim()),
                new SqlParameter("@MinimumMark", request.MinimumMark),
                new SqlParameter("@MaximumMark", request.MaximumMark),
                new SqlParameter("@Remark", request.Remark.Trim()),
                new SqlParameter("@Level", request.Level?.Trim() ?? ""),
                new SqlParameter("@Id", id),
                new SqlParameter("@SchoolId", schoolId));

            if (rowsAffected > 0)
            {
                return Ok(new
                {
                    success = true,
                    message = "Grading system updated successfully.",
                    id = id
                });
            }

            return NotFound(new { message = "Grade not found." });
        }
        catch (SqlException ex)
        {
            return Problem($"Failed to update grading system: {ex.Message}", statusCode: 500);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGrade(int id, CancellationToken cancellationToken)
    {
        if (!TryGetSchoolId(out var schoolId))
        {
            return Unauthorized(new { message = "The authenticated user does not have a valid school." });
        }

        if (id <= 0)
        {
            return BadRequest(new { message = "A valid grade ID is required." });
        }

        try
        {
            var rowsAffected = await database.ExecuteAsync(
                "DELETE FROM GradingSystem WHERE Id = @Id AND SchoolId = @SchoolId",
                cancellationToken,
                new SqlParameter("@Id", id),
                new SqlParameter("@SchoolId", schoolId));

            if (rowsAffected > 0)
            {
                return Ok(new
                {
                    success = true,
                    message = "Grading system deleted successfully.",
                    id = id
                });
            }

            return NotFound(new { message = "Grade not found." });
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            return Conflict(new { message = "This grading system cannot be deleted because it is used by other records (e.g., exam results)." });
        }
        catch (SqlException ex)
        {
            return Problem($"Failed to delete grading system: {ex.Message}", statusCode: 500);
        }
    }

    private static string? ValidateGrade(GradingSystemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Grade))
        {
            return "Grade is required.";
        }

        if (string.IsNullOrWhiteSpace(request.Remark))
        {
            return "Remark is required.";
        }

        if (request.MinimumMark < 0 || request.MinimumMark > 100)
        {
            return "Minimum mark must be between 0 and 100.";
        }

        if (request.MaximumMark < 0 || request.MaximumMark > 100)
        {
            return "Maximum mark must be between 0 and 100.";
        }

        if (request.MinimumMark >= request.MaximumMark)
        {
            return "Minimum mark must be less than maximum mark.";
        }

        return null;
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
