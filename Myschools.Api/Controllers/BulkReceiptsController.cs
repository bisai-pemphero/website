using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Myschools.Api.Data;

namespace Myschools.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/bulk-receipts")]
public sealed class BulkReceiptsController(SqlDatabase database) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] DateTime? date, [FromQuery] int? classId, [FromQuery] int? feesId, [FromQuery] string? transactionIds, CancellationToken cancellationToken)
	{
		if (!date.HasValue && !classId.HasValue && !feesId.HasValue && string.IsNullOrWhiteSpace(transactionIds))
		{
			return BadRequest(new
			{
				message = "Provide date, classId, feesId, or transactionIds."
			});
		}
		int num = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		List<string> list = new List<string> { "F.SchoolId = @SchoolId", "P.IsDeleted = 0" };
		List<SqlParameter> list2 = new List<SqlParameter>
		{
			new SqlParameter("@SchoolId", num)
		};
		if (date.HasValue)
		{
			list.Add("CONVERT(date, P.datePaid) = @Date");
			list2.Add(new SqlParameter("@Date", date.Value.Date));
		}
		if (classId.HasValue)
		{
			list.Add("C.ClassID = @ClassId");
			list2.Add(new SqlParameter("@ClassId", classId.Value));
		}
		if (feesId.HasValue)
		{
			list.Add("F.FeesId = @FeesId");
			list2.Add(new SqlParameter("@FeesId", feesId.Value));
		}
		if (!string.IsNullOrWhiteSpace(transactionIds))
		{
			int[] array = (from text in transactionIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
				select (!int.TryParse(text.Trim(), out var result)) ? ((int?)null) : new int?(result) into id
				where id.HasValue
				select id.Value).ToArray();
			if (array.Length == 0)
			{
				return BadRequest(new
				{
					message = "transactionIds must contain integer IDs."
				});
			}
			string[] value = array.Select((int _, int index) => "@Transaction" + index).ToArray();
			list.Add("P.TransanctionId IN (" + string.Join(",", value) + ")");
			list2.AddRange(array.Select((int id, int index) => new SqlParameter("@Transaction" + index, id)));
		}
		string sql = "SELECT P.TransanctionId AS TransactionId, F.FeesId, CONVERT(date, P.datePaid) AS PaymentDate, P.paidBy, P.paymentMode, P.Paid, P.Balance, T.TermName, CONCAT(S.FirstName, ' ', S.Middlename, ' ', S.LastName) AS StudentName, P.postedBy, FC.CategoryName, P.receipNumber, C.ClassName, SCH.School_name, SCH.PhoneNumber, SCH.Slogan, SCH.Logo, SCH.School_address FROM Payments P JOIN Fees F ON F.FeesId = P.FeesId JOIN SchoolTerm T ON F.TermId = T.TermId JOIN Students S ON S.StudentID = F.studentId JOIN Classes C ON C.ClassID = S.CurrentClassID JOIN FeesCategory FC ON FC.CategoryId = F.fees_Category JOIN AllSchools SCH ON SCH.SchoolId = F.SchoolId WHERE " + string.Join(" AND ", list) + " ORDER BY P.datePaid, P.receipNumber";
		return Ok(await database.QueryAsync(sql, cancellationToken, list2.ToArray()));
	}
}
