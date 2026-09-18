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
[Route("api/schools")]
public sealed class SchoolsController(SqlDatabase database) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		return Ok(await database.QueryAsync("SELECT SchoolId, School_name AS SchoolName, School_email AS SchoolEmail, PhoneNumber, School_address AS SchoolAddress, Slogan, Admin_name AS AdminName, Admin_email AS AdminEmail, Admin_phone AS AdminPhone FROM AllSchools ORDER BY School_name", cancellationToken));
	}

	[HttpGet("{schoolId:int}")]
	public async Task<IActionResult> Get(int schoolId, CancellationToken cancellationToken)
	{
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT * FROM AllSchools WHERE SchoolId = @SchoolId", cancellationToken, new SqlParameter("@SchoolId", schoolId));
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
	public async Task<IActionResult> Create(SchoolRequest request, CancellationToken cancellationToken)
	{
		IActionResult result;
		await using (SqlConnection connection = database.CreateConnection())
		{
			await connection.OpenAsync(cancellationToken);
			IActionResult actionResult;
			await using (SqlCommand command = new SqlCommand("INSERT INTO AllSchools (School_name, School_email, PhoneNumber, School_address, Slogan, Admin_name, Admin_email, Admin_phone) VALUES (@SchoolName, @SchoolEmail, @PhoneNumber, @SchoolAddress, @Slogan, @AdminName, @AdminEmail, @AdminPhone); SELECT CAST(SCOPE_IDENTITY() AS int);", connection))
			{
				command.Parameters.AddRange(ToParameters(request));
				int schoolId = Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
				actionResult = CreatedAtAction("Get", new { schoolId }, new { schoolId });
			}
			result = actionResult;
		}
		return result;
	}

	private static SqlParameter[] ToParameters(SchoolRequest request)
	{
		return new SqlParameter[8]
		{
			new SqlParameter("@SchoolName", request.SchoolName),
			new SqlParameter("@SchoolEmail", ((object)request.SchoolEmail) ?? ((object)DBNull.Value)),
			new SqlParameter("@PhoneNumber", ((object)request.PhoneNumber) ?? ((object)DBNull.Value)),
			new SqlParameter("@SchoolAddress", ((object)request.SchoolAddress) ?? ((object)DBNull.Value)),
			new SqlParameter("@Slogan", ((object)request.Slogan) ?? ((object)DBNull.Value)),
			new SqlParameter("@AdminName", ((object)request.AdminName) ?? ((object)DBNull.Value)),
			new SqlParameter("@AdminEmail", ((object)request.AdminEmail) ?? ((object)DBNull.Value)),
			new SqlParameter("@AdminPhone", ((object)request.AdminPhone) ?? ((object)DBNull.Value))
		};
	}
}
