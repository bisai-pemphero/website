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
[Route("api/invoices")]
public sealed class InvoicesController(SqlDatabase database) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] int? schoolId, CancellationToken cancellationToken)
	{
		schoolId.GetValueOrDefault();
		if (!schoolId.HasValue)
		{
			int value = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
			schoolId = value;
		}
		return Ok(await database.QueryAsync("SELECT InvoiceId, InvoiceNo, Description, Total, SchoolId FROM SchoolInvoice WHERE SchoolId = @SchoolId ORDER BY InvoiceId DESC", cancellationToken, new SqlParameter("@SchoolId", schoolId)));
	}

	[HttpGet("{invoiceId:int}")]
	public async Task<IActionResult> GetById(int invoiceId, CancellationToken cancellationToken)
	{
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT SI.*, AL.School_name AS SchoolName, AL.Admin_email AS SchoolEmail, AL.Admin_name AS SchoolAdmin, AL.School_address AS SchoolAddress, AL.PhoneNumber AS SchoolContact FROM SchoolInvoice SI JOIN AllSchools AL ON SI.SchoolId = AL.SchoolId WHERE SI.InvoiceId = @InvoiceId", cancellationToken, new SqlParameter("@InvoiceId", invoiceId));
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

	[HttpPost]
	public async Task<IActionResult> Create(InvoiceRequest request, CancellationToken cancellationToken)
	{
		string invoiceNo = "INVOICE-" + Convert.ToInt32((await database.QueryAsync("SELECT InvoiceNo + 1 AS Next FROM ValueSequence", cancellationToken))[0]["Next"]);
		IActionResult result;
		await using (SqlConnection connection = database.CreateConnection())
		{
			await connection.OpenAsync(cancellationToken);
			IActionResult actionResult;
			await using (SqlCommand command = new SqlCommand("INSERT INTO SchoolInvoice (InvoiceNo, SchoolId, Description, Total) VALUES (@InvoiceNo, @SchoolId, @Description, @Total); SELECT CAST(SCOPE_IDENTITY() AS int);", connection))
			{
				command.Parameters.AddRange(new SqlParameter[4]
				{
					new SqlParameter("@InvoiceNo", invoiceNo),
					new SqlParameter("@SchoolId", request.SchoolId),
					new SqlParameter("@Description", request.Description),
					new SqlParameter("@Total", request.Total)
				});
				int invoiceId = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
				actionResult = CreatedAtAction("GetById", new { invoiceId }, new { invoiceId, invoiceNo });
			}
			result = actionResult;
		}
		return result;
	}
}
