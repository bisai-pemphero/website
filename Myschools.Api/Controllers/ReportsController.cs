using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Myschools.Api.Data;

namespace Myschools.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/reports")]
public sealed class ReportsController(SqlDatabase database) : ControllerBase
{
	[HttpGet("payments")]
	public async Task<IActionResult> Payments([FromQuery] int termId, [FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken cancellationToken)
	{
		int num = SchoolId();
		return Ok(await database.QueryAsync("SELECT S.FirstName, S.Middlename, S.LastName, P.Paid AS AmountPaid, FC.CategoryName AS FeesFor, C.ClassName, P.paymentMode, P.paidBy, P.receipNumber, P.datePaid FROM Payments P JOIN Fees F ON F.FeesId = P.FeesId JOIN Students S ON S.StudentID = F.studentId JOIN FeesCategory FC ON FC.CategoryId = F.fees_Category JOIN Classes C ON C.ClassID = S.CurrentClassID WHERE F.TermId = @TermId AND S.SchoolId = @SchoolId AND P.IsDeleted = 0 AND CONVERT(date, P.datePaid) BETWEEN CONVERT(date, @Start) AND CONVERT(date, @End) ORDER BY S.CurrentClassID, P.datePaid", cancellationToken, new SqlParameter("@TermId", termId), new SqlParameter("@SchoolId", num), new SqlParameter("@Start", start.Date), new SqlParameter("@End", end.Date)));
	}

	[HttpGet("payments/by-category")]
	public async Task<IActionResult> ByCategory([FromQuery] int termId, [FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken cancellationToken)
	{
		return Ok(await database.QueryAsync("SELECT FC.CategoryName, SUM(P.Paid) AS TotalAmountPaid FROM Payments P JOIN Fees F ON P.FeesId = F.FeesId JOIN FeesCategory FC ON F.fees_Category = FC.CategoryId WHERE F.TermId = @TermId AND F.SchoolId = @SchoolId AND P.IsDeleted = 0 AND CONVERT(date, P.datePaid) BETWEEN CONVERT(date, @Start) AND CONVERT(date, @End) GROUP BY FC.CategoryName", cancellationToken, Parameters(termId, start, end)));
	}

	[HttpGet("payments/by-mode")]
	public async Task<IActionResult> ByMode([FromQuery] int termId, [FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken cancellationToken)
	{
		return Ok(await database.QueryAsync("SELECT P.paymentMode, SUM(P.Paid) AS TotalAmountPaid FROM Payments P JOIN Fees F ON F.FeesId = P.FeesId WHERE F.TermId = @TermId AND F.SchoolId = @SchoolId AND P.IsDeleted = 0 AND CONVERT(date, P.datePaid) BETWEEN CONVERT(date, @Start) AND CONVERT(date, @End) GROUP BY P.paymentMode", cancellationToken, Parameters(termId, start, end)));
	}

	[HttpGet("payments/by-class")]
	public async Task<IActionResult> ByClass([FromQuery] int termId, [FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken cancellationToken)
	{
		return Ok(await database.QueryAsync("SELECT C.ClassName, SUM(P.Paid) AS TotalAmountPaid FROM Classes C JOIN Students S ON C.ClassID = S.CurrentClassID JOIN Fees F ON F.studentId = S.StudentID JOIN Payments P ON P.FeesId = F.FeesId WHERE F.TermId = @TermId AND F.SchoolId = @SchoolId AND P.IsDeleted = 0 AND CONVERT(date, P.datePaid) BETWEEN CONVERT(date, @Start) AND CONVERT(date, @End) GROUP BY C.ClassName", cancellationToken, Parameters(termId, start, end)));
	}

	[HttpGet("payments/total")]
	public async Task<IActionResult> Total([FromQuery] int termId, [FromQuery] DateTime start, [FromQuery] DateTime end, CancellationToken cancellationToken)
	{
		return Ok((await database.QueryAsync("SELECT COALESCE(SUM(P.Paid), 0) AS Total FROM Payments P JOIN Fees F ON P.FeesId = F.FeesId WHERE F.SchoolId = @SchoolId AND F.TermId = @TermId AND P.IsDeleted = 0 AND CONVERT(date, P.datePaid) BETWEEN CONVERT(date, @Start) AND CONVERT(date, @End)", cancellationToken, Parameters(termId, start, end)))[0]);
	}

	private int SchoolId()
	{
		return int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
	}

	private SqlParameter[] Parameters(int termId, DateTime start, DateTime end)
	{
		return new SqlParameter[4]
		{
			new SqlParameter("@TermId", termId),
			new SqlParameter("@SchoolId", SchoolId()),
			new SqlParameter("@Start", start.Date),
			new SqlParameter("@End", end.Date)
		};
	}
}
