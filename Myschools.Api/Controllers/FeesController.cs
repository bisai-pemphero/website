using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Myschools.Api.Data;

namespace Myschools.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/fees")]
public sealed class FeesController(SqlDatabase database) : ControllerBase
{
	[HttpGet("outstanding")]
	public async Task<IActionResult> Outstanding([FromQuery] int termId, [FromQuery] int feesCategory, [FromQuery] int classId, CancellationToken cancellationToken)
	{
		int schoolId = base.User.GetSchoolId();
		return Ok(await database.QueryAsync("SELECT F.FeesId, S.FirstName, S.Middlename, S.LastName, ST.TermName, C.ClassName, CONCAT(DATENAME(YEAR, AY.Start_year), '-', YEAR(AY.End_year)) AS AcademicYear, FC.CategoryName, F.amountPaid, F.balance, S.StudentID FROM Fees F JOIN SchoolTerm ST ON F.TermId = ST.TermId JOIN Academic_Year AY ON ST.AcademicYearId = AY.AcademicyearId JOIN Students S ON F.studentId = S.StudentID JOIN Classes C ON C.ClassID = S.CurrentClassID JOIN FeesCategory FC ON FC.CategoryId = F.fees_Category WHERE F.balance > 0 AND F.SchoolId = @SchoolId AND F.TermId = @TermId AND F.fees_Category = @FeesCategory AND S.CurrentClassID = @ClassId AND S.IsDeleted = 0", cancellationToken, new SqlParameter("@SchoolId", schoolId), new SqlParameter("@TermId", termId), new SqlParameter("@FeesCategory", feesCategory), new SqlParameter("@ClassId", classId)));
	}

	[HttpGet("categories")]
	public async Task<IActionResult> Categories(CancellationToken cancellationToken)
	{
		int schoolId = base.User.GetSchoolId();
		return Ok(await database.QueryAsync("SELECT CategoryId, CategoryName, Amount FROM FeesCategory WHERE SchoolId = @SchoolId ORDER BY CategoryName", cancellationToken, new SqlParameter("@SchoolId", schoolId)));
	}
}
