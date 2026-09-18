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
[Route("api/receipts")]
public sealed class ReceiptsController(SqlDatabase database) : ControllerBase
{
	[HttpGet("{transactionId:int}")]
	public async Task<IActionResult> Get(int transactionId, CancellationToken cancellationToken)
	{
		int num = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT P.datePaid, P.paidBy, P.paymentMode, P.Paid, P.Balance, T.TermName, S.FirstName, S.Middlename, S.LastName, COALESCE(U.Fullname, P.postedBy) AS PostedBy, FC.CategoryName, S.SchoolId, P.receipNumber, C.ClassName, A.School_name AS SchoolName, A.PhoneNumber, A.Slogan, A.Logo, A.School_address FROM Payments P JOIN Fees F ON F.FeesId = P.FeesId JOIN SchoolTerm T ON T.TermId = F.TermId JOIN Students S ON S.StudentID = F.studentId LEFT JOIN Users U ON U.Username = P.postedBy JOIN FeesCategory FC ON FC.CategoryId = F.fees_Category JOIN Classes C ON C.ClassID = S.CurrentClassID JOIN AllSchools A ON A.SchoolId = S.SchoolId WHERE P.TransanctionId = @TransactionId AND S.SchoolId = @SchoolId", cancellationToken, new SqlParameter("@TransactionId", transactionId), new SqlParameter("@SchoolId", num));
		IActionResult result;
		if (readOnlyList.Count != 0)
		{
			IActionResult actionResult = Ok(readOnlyList[0]);
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound();
			result = actionResult;
		}
		return result;
	}
}
