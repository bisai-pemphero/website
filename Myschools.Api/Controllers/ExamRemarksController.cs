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
[Route("api/exams/remarks")]
public sealed class ExamRemarksController(SqlDatabase database) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Save(ExamRemarkRequest request, CancellationToken cancellationToken)
	{
		int schoolId = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		string postedBy = base.User.Identity?.Name ?? "system";
		string tableName = request.IsHeadTeacher ? "ExamRemarksHeadTeacher" : "ExamRemarksTeacher";

		// Check if record exists
		string checkSql = $"SELECT COUNT(*) AS Count FROM {tableName} WHERE ExamId = @ExamId AND StudentId = @StudentId AND SchoolId = @SchoolId";
		var checkResult = await database.QueryAsync(checkSql, cancellationToken,
			new SqlParameter("@ExamId", request.ExamId),
			new SqlParameter("@StudentId", request.StudentId),
			new SqlParameter("@SchoolId", schoolId));

		bool exists = Convert.ToInt32(checkResult[0]["Count"]) > 0;

		if (exists)
		{
			string updateSql = $"UPDATE {tableName} SET Remark = @Remark, DateCreated = GETDATE(), Postedby = @PostedBy WHERE ExamId = @ExamId AND StudentId = @StudentId AND SchoolId = @SchoolId";
			await database.ExecuteAsync(updateSql, cancellationToken,
				new SqlParameter("@Remark", request.Remark),
				new SqlParameter("@PostedBy", postedBy),
				new SqlParameter("@ExamId", request.ExamId),
				new SqlParameter("@StudentId", request.StudentId),
				new SqlParameter("@SchoolId", schoolId));

			return Ok(new { message = "Remark updated successfully.", action = "update" });
		}
		else
		{
			string insertSql = $"INSERT INTO {tableName} (Remark, DateCreated, ExamId, StudentId, Postedby, SchoolId) VALUES (@Remark, GETDATE(), @ExamId, @StudentId, @PostedBy, @SchoolId)";
			await database.ExecuteAsync(insertSql, cancellationToken,
				new SqlParameter("@Remark", request.Remark),
				new SqlParameter("@ExamId", request.ExamId),
				new SqlParameter("@StudentId", request.StudentId),
				new SqlParameter("@PostedBy", postedBy),
				new SqlParameter("@SchoolId", schoolId));

			return Ok(new { message = "Remark saved successfully.", action = "insert" });
		}
	}
}
