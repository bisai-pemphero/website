using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Myschools.Api.Data;

namespace Myschools.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/report-cards")]
public sealed class ReportCardsController(SqlDatabase database) : ControllerBase
{
	[HttpGet("marks")]
	public async Task<IActionResult> Marks([FromQuery] int studentId, [FromQuery] int examId, CancellationToken cancellationToken)
	{
		int num = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		if (await HasOutstandingBalance(studentId, num, cancellationToken))
		{
			return StatusCode(402, new { message = "Access Restricted: Outstanding Fees Balance. Please clear your balance to view report cards." });
		}
		return Ok(await database.QueryAsync("SELECT S.SubjectId, S.SubjectName, G.Marks, GS.Grade, GS.Remark FROM Grades G INNER JOIN Subjects S ON S.SubjectId = G.SubjectId INNER JOIN Classes C ON S.ClassId = C.ClassID INNER JOIN GradingSystem GS ON GS.Level = C.Level AND GS.SchoolId = C.SchoolId AND G.Marks BETWEEN GS.Minimum_Mark AND GS.Maxmum_Mark WHERE G.ExamId = @ExamId AND G.StudentId = @StudentId AND G.SchoolId = @SchoolId ORDER BY S.SubjectName", cancellationToken, new SqlParameter("@ExamId", examId), new SqlParameter("@StudentId", studentId), new SqlParameter("@SchoolId", num)));
	}

	[HttpGet("grading-system")]
	public async Task<IActionResult> GradingSystem([FromQuery] int classId, CancellationToken cancellationToken)
	{
		int num = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		return Ok(await database.QueryAsync("SELECT G.Grade, G.Minimum_Mark, G.Maxmum_Mark, G.Remark FROM GradingSystem G WHERE G.Level = (SELECT Level FROM Classes WHERE ClassID = @ClassId AND SchoolId = @SchoolId) AND G.SchoolId = @SchoolId ORDER BY G.Minimum_Mark DESC", cancellationToken, new SqlParameter("@ClassId", classId), new SqlParameter("@SchoolId", num)));
	}

	[HttpGet("remarks")]
	public async Task<IActionResult> Remarks([FromQuery] int studentId, [FromQuery] int examId, CancellationToken cancellationToken)
	{
		int num = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		if (await HasOutstandingBalance(studentId, num, cancellationToken))
		{
			return StatusCode(402, new { message = "Access Restricted: Outstanding Fees Balance." });
		}
		return Ok(new
		{
			teacher = await database.QueryAsync("SELECT TR.Remark, TR.DateCreated, U.Fullname FROM ExamRemarksTeacher TR JOIN Users U ON U.Username = TR.Postedby WHERE TR.ExamId = @ExamId AND TR.StudentId = @StudentId", cancellationToken, new SqlParameter("@ExamId", examId), new SqlParameter("@StudentId", studentId)),
			headTeacher = await database.QueryAsync("SELECT TR.Remark, TR.DateCreated, U.Fullname FROM ExamRemarksHeadTeacher TR JOIN Users U ON U.Username = TR.Postedby WHERE TR.ExamId = @ExamId AND TR.StudentId = @StudentId", cancellationToken, new SqlParameter("@ExamId", examId), new SqlParameter("@StudentId", studentId))
		});
	}

	private async Task<bool> HasOutstandingBalance(int studentId, int schoolId, CancellationToken cancellationToken)
	{
		var result = await database.QueryAsync("SELECT COUNT(*) AS Count FROM Fees WHERE studentId = @StudentId AND SchoolId = @SchoolId AND balance > 0", cancellationToken, new SqlParameter("@StudentId", studentId), new SqlParameter("@SchoolId", schoolId));
		return Convert.ToInt32(result[0]["Count"]) > 0;
	}
}
