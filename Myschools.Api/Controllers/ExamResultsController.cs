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
[Route("api/exam-results")]
public sealed class ExamResultsController(SqlDatabase database) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] int examId, [FromQuery] int classId, CancellationToken cancellationToken)
	{
		int schoolId = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT SubjectId FROM Subjects WHERE ClassId = @ClassId AND SubjectName = 'English'", cancellationToken, new SqlParameter("@ClassId", classId));
		if (readOnlyList.Count == 0)
		{
			return NotFound(new
			{
				message = "English subject is not configured for this class."
			});
		}
		return Ok(await database.QueryAsync("WITH SubjectGrades AS (SELECT s.StudentID, s.FirstName, s.Middlename, s.LastName, s.CurrentClassID AS ClassID, c.Level, g.SubjectId, g.Marks, gs.Grade AS SubjectGrade, gs.Remark AS SubjectRemark, CASE WHEN g.SubjectId = @EnglishSubjectId AND g.Marks > 40 THEN 1 ELSE 0 END AS EnglishPassed FROM Grades g JOIN Students s ON s.StudentID = g.StudentId JOIN Classes c ON c.ClassID = s.CurrentClassID JOIN GradingSystem gs ON g.Marks BETWEEN gs.Minimum_Mark AND gs.Maxmum_Mark AND gs.Level = c.Level AND gs.SchoolId = @SchoolId WHERE g.ExamId = @ExamId AND g.SchoolId = @SchoolId AND s.SchoolId = @SchoolId AND c.SchoolId = @SchoolId AND s.CurrentClassID = @ClassId AND s.Status = 'Active' AND s.IsDeleted = 0), English AS (SELECT * FROM SubjectGrades WHERE SubjectId = @EnglishSubjectId), OtherSubjects AS (SELECT *, ROW_NUMBER() OVER (PARTITION BY StudentID ORDER BY Marks DESC) AS rn FROM SubjectGrades WHERE SubjectId <> @EnglishSubjectId), BestSix AS (SELECT StudentID, ClassID, Level, Marks, EnglishPassed, SubjectId, SubjectGrade, SubjectRemark FROM English UNION ALL SELECT StudentID, ClassID, Level, Marks, 0, SubjectId, SubjectGrade, SubjectRemark FROM OtherSubjects WHERE rn <= 5), Totals AS (SELECT StudentID, ClassID, Level, COUNT(SubjectId) AS SubjectsCount, SUM(Marks) AS TotalMarks, SUM(Marks) * 100.0 / 600 AS Percentage, MAX(EnglishPassed) AS PassedEnglish, CASE WHEN MAX(EnglishPassed) = 1 AND SUM(Marks) >= 240 THEN 1 ELSE 0 END AS OverallPass, MAX(CASE WHEN SubjectId = @EnglishSubjectId THEN Marks END) AS EnglishMarks FROM BestSix GROUP BY StudentID, ClassID, Level HAVING COUNT(SubjectId) = 6) SELECT t.StudentID, s.FirstName, s.Middlename, s.LastName, c.ClassName, t.Level, t.TotalMarks, t.Percentage, CASE WHEN t.EnglishMarks > 40 THEN 'PASSED' ELSE 'FAILED' END AS EnglishResult, gs.Grade AS FinalGrade, gs.Remark AS FinalRemark, CASE WHEN t.OverallPass = 1 THEN DENSE_RANK() OVER (PARTITION BY t.ClassID ORDER BY t.TotalMarks DESC) END AS PositionInPassedStudents, DENSE_RANK() OVER (PARTITION BY t.ClassID ORDER BY t.TotalMarks DESC) AS ClassRank, CASE WHEN t.OverallPass = 1 THEN 'PASS' ELSE 'FAIL' END AS FinalResult FROM Totals t JOIN Students s ON s.StudentID = t.StudentID JOIN Classes c ON c.ClassID = t.ClassID JOIN GradingSystem gs ON t.Percentage BETWEEN gs.Minimum_Mark AND gs.Maxmum_Mark AND gs.Level = t.Level AND gs.SchoolId = @SchoolId ORDER BY t.TotalMarks DESC", cancellationToken, new SqlParameter("@EnglishSubjectId", readOnlyList[0]["SubjectId"]), new SqlParameter("@ExamId", examId), new SqlParameter("@SchoolId", schoolId), new SqlParameter("@ClassId", classId)));
	}
}
