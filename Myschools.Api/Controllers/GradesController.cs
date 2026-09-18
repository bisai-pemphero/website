using System;
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
[Route("api/grades")]
public sealed class GradesController(SqlDatabase database) : ControllerBase
{
	[HttpGet("roster")]
	public async Task<IActionResult> Roster([FromQuery] int examId, [FromQuery] int subjectId, [FromQuery] int classId, CancellationToken cancellationToken)
	{
		int num = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		return Ok(await database.QueryAsync("SELECT S.StudentID, S.FirstName, S.Middlename, S.LastName, G.Marks FROM Students S LEFT JOIN Grades G ON G.StudentId = S.StudentID AND G.ExamId = @ExamId AND G.SubjectId = @SubjectId AND G.SchoolId = @SchoolId WHERE S.CurrentClassID = @ClassId AND S.SchoolId = @SchoolId AND S.Status = 'Active' AND S.IsDeleted = 0 ORDER BY S.LastName, S.FirstName", cancellationToken, new SqlParameter("@ExamId", examId), new SqlParameter("@SubjectId", subjectId), new SqlParameter("@ClassId", classId), new SqlParameter("@SchoolId", num)));
	}

	[HttpPost]
	public async Task<IActionResult> Save(GradeRequest request, CancellationToken cancellationToken)
	{
		if (request.Marks < 0m || request.Marks > 100m)
		{
			return BadRequest(new
			{
				message = "Marks must be between 0 and 100."
			});
		}
		int schoolId = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		SqlParameter[] parameters = new SqlParameter[4]
		{
			new SqlParameter("@ExamId", request.ExamId),
			new SqlParameter("@SubjectId", request.SubjectId),
			new SqlParameter("@StudentId", request.StudentId),
			new SqlParameter("@SchoolId", schoolId)
		};
		if (Convert.ToInt32((await database.QueryAsync("SELECT COUNT(*) AS Count FROM Grades WHERE ExamId = @ExamId AND SubjectId = @SubjectId AND StudentId = @StudentId AND SchoolId = @SchoolId", cancellationToken, parameters))[0]["Count"]) > 0)
		{
			SqlDatabase sqlDatabase = database;
			SqlParameter[] array = parameters;
			int num = 0;
			SqlParameter[] array2 = new SqlParameter[2 + array.Length];
			ReadOnlySpan<SqlParameter> readOnlySpan = new ReadOnlySpan<SqlParameter>(array);
			readOnlySpan.CopyTo(new Span<SqlParameter>(array2).Slice(num, readOnlySpan.Length));
			num += readOnlySpan.Length;
			array2[num] = new SqlParameter("@Marks", request.Marks);
			num++;
			array2[num] = new SqlParameter("@TeacherId", request.TeacherId);
			return Ok(new
			{
				message = "Marks updated successfully.",
				action = "update",
				rowsAffected = await sqlDatabase.ExecuteAsync("UPDATE Grades SET Marks = @Marks, TeacherId = @TeacherId, DateSubmitted = GETDATE() WHERE ExamId = @ExamId AND SubjectId = @SubjectId AND StudentId = @StudentId AND SchoolId = @SchoolId", cancellationToken, array2)
			});
		}
		return Ok(new
		{
			message = "Marks saved successfully.",
			action = "insert",
			rowsAffected = await database.ExecuteAsync("INSERT INTO Grades (ExamId, SubjectId, Marks, StudentId, TeacherId, SchoolId, DateSubmitted) VALUES (@ExamId, @SubjectId, @Marks, @StudentId, @TeacherId, @SchoolId, GETDATE())", cancellationToken, new SqlParameter("@ExamId", request.ExamId), new SqlParameter("@SubjectId", request.SubjectId), new SqlParameter("@Marks", request.Marks), new SqlParameter("@StudentId", request.StudentId), new SqlParameter("@TeacherId", request.TeacherId), new SqlParameter("@SchoolId", schoolId))
		});
	}
}
