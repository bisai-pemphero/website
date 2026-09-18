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
[Route("api/licences")]
public sealed class LicencesController(SqlDatabase database, LegacyPasswordCipher cipher) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		return Ok(await database.QueryAsync("SELECT A.SchoolId, A.School_name AS SchoolName, A.School_address AS SchoolAddress, A.Admin_name AS AdminName, SL.Status, SL.Mode, SL.DateUpdated, SL.ExpiryDate FROM AllSchools A LEFT JOIN School_Licence SL ON SL.SchoolId = A.SchoolId ORDER BY A.School_name", cancellationToken));
	}

	[HttpGet("{schoolId:int}")]
	public async Task<IActionResult> GetBySchool(int schoolId, CancellationToken cancellationToken)
	{
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT A.SchoolId, A.School_name AS SchoolName, A.Slogan, A.Logo, SL.Status, SL.Mode, SL.DateUpdated, SL.ExpiryDate FROM School_Licence SL JOIN AllSchools A ON A.SchoolId = SL.SchoolId WHERE SL.SchoolId = @SchoolId", cancellationToken, new SqlParameter("@SchoolId", schoolId));
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

	[HttpPost("{schoolId:int}")]
	public async Task<IActionResult> Generate(int schoolId, LicenceRequest request, CancellationToken cancellationToken)
	{
		string key = cipher.EncryptForLegacyLogin($"MI{schoolId}{request.ExpiryDate}S");
		IActionResult result;
		await using (SqlConnection connection = database.CreateConnection())
		{
			await connection.OpenAsync(cancellationToken);
			IActionResult actionResult4;
			await using (SqlTransaction transaction = (SqlTransaction)(await connection.BeginTransactionAsync(cancellationToken)))
			{
				try
				{
					IActionResult actionResult;
					await using (SqlCommand duplicate = new SqlCommand("SELECT COUNT(*) FROM T_Sys_Licence WHERE LicenceKey = @LicenceKey", connection, transaction))
					{
						duplicate.Parameters.Add(new SqlParameter("@LicenceKey", key));
						if (Convert.ToInt32(await duplicate.ExecuteScalarAsync(cancellationToken)) > 0)
						{
							await transaction.RollbackAsync(cancellationToken);
							actionResult = Conflict(new
							{
								message = "This licence key already exists."
							});
						}
						else
						{
							IActionResult actionResult3;
							await using (SqlCommand insert = new SqlCommand("INSERT INTO T_Sys_Licence (LicenceKey, Status, Category, Used, SchoolId, DateUpdated, ExpiryDate, Postedby, DateCreated) VALUES (@LicenceKey, 'Active', @Category, 'false', @SchoolId, @StartDate, @ExpiryDate, @PostedBy, GETDATE())", connection, transaction))
							{
								insert.Parameters.Add(new SqlParameter("@LicenceKey", key));
								insert.Parameters.Add(new SqlParameter("@Category", request.Category.Trim()));
								insert.Parameters.Add(new SqlParameter("@SchoolId", schoolId));
								insert.Parameters.Add(new SqlParameter("@StartDate", request.StartDate));
								insert.Parameters.Add(new SqlParameter("@ExpiryDate", request.ExpiryDate));
								insert.Parameters.Add(new SqlParameter("@PostedBy", base.User.Identity?.Name ?? string.Empty));
								await insert.ExecuteNonQueryAsync(cancellationToken);
								IActionResult actionResult2;
								await using (SqlCommand activate = new SqlCommand("UPDATE School_Licence SET LicenceKey = @LicenceKey, Mode = @Category, Status = 'Active', DateUpdated = @StartDate, ExpiryDate = @ExpiryDate WHERE SchoolId = @SchoolId", connection, transaction))
								{
									activate.Parameters.Add(new SqlParameter("@LicenceKey", key));
									activate.Parameters.Add(new SqlParameter("@Category", request.Category.Trim()));
									activate.Parameters.Add(new SqlParameter("@StartDate", request.StartDate));
									activate.Parameters.Add(new SqlParameter("@ExpiryDate", request.ExpiryDate));
									activate.Parameters.Add(new SqlParameter("@SchoolId", schoolId));
									await activate.ExecuteNonQueryAsync(cancellationToken);
									await transaction.CommitAsync(cancellationToken);
									actionResult2 = Ok(new
									{
										schoolId = schoolId,
										licenceKey = key,
										Category = request.Category,
										StartDate = request.StartDate,
										ExpiryDate = request.ExpiryDate
									});
								}
								actionResult3 = actionResult2;
							}
							actionResult = actionResult3;
						}
					}
					actionResult4 = actionResult;
				}
				catch
				{
					await transaction.RollbackAsync(cancellationToken);
					throw;
				}
			}
			result = actionResult4;
		}
		return result;
	}
}
