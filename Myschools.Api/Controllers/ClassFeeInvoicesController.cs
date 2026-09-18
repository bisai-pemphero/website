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
[Route("api/class-fee-invoices")]
public sealed class ClassFeeInvoicesController(SqlDatabase database) : ControllerBase
{
	[HttpGet("current-term")]
	public async Task<IActionResult> GetCurrentTermInvoices(CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		return Ok(await database.QueryAsync("SELECT F.FeesId, F.studentId AS StudentId, S.FirstName, S.Middlename, \r\n        S.LastName, S.CurrentClassID AS ClassId, C.ClassName, F.TermId, T.TermName, \r\n        F.fees_Category AS FeesCategoryId, FC.CategoryName, F.totalFees, F.FeesStatus,\r\n        F.InitalPaymentDate, F.amountPaid, F.balance, F.registeredBy, F.dateRegistered \r\n        FROM Fees F JOIN Students S ON S.StudentID = F.studentId JOIN Classes C \r\n        ON C.ClassID = S.CurrentClassID JOIN SchoolTerm T ON T.TermId = F.TermId\r\n        JOIN FeesCategory FC ON FC.CategoryId = F.fees_Category WHERE F.SchoolId = @SchoolId\r\n        AND T.TermId = (SELECT TermId FROM SchoolTerm WHERE IsActive = 'True' AND\r\n        SchoolId = @SchoolId) AND S.IsDeleted = 0 ORDER BY C.ClassName, S.LastName, S.FirstName", cancellationToken, new SqlParameter("@SchoolId", schoolId)));
	}

	[HttpGet("term")]
	public async Task<IActionResult> GetInvoicesByTerm([FromQuery] int termId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (termId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid term is required."
			});
		}
		return Ok(await database.QueryAsync("SELECT F.FeesId, F.studentId AS StudentId, S.FirstName, S.Middlename, S.LastName, S.CurrentClassID AS ClassId, C.ClassName, F.TermId, T.TermName, F.fees_Category AS FeesCategoryId, FC.CategoryName, F.totalFees, F.FeesStatus, F.InitalPaymentDate, F.amountPaid, F.balance, F.registeredBy, F.dateRegistered FROM Fees F JOIN Students S ON S.StudentID = F.studentId JOIN Classes C ON C.ClassID = S.CurrentClassID JOIN SchoolTerm T ON T.TermId = F.TermId JOIN FeesCategory FC ON FC.CategoryId = F.fees_Category WHERE F.SchoolId = @SchoolId AND F.TermId = @TermId AND T.SchoolId = @SchoolId AND S.IsDeleted = 0 ORDER BY C.ClassName, S.LastName, S.FirstName", cancellationToken, new SqlParameter("@SchoolId", schoolId), new SqlParameter("@TermId", termId)));
	}

	[HttpPost]
	public async Task<IActionResult> Generate(ClassFeeInvoiceRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (request.ClassId <= 0 || request.TermId <= 0 || request.FeesCategoryId <= 0)
		{
			return BadRequest(new
			{
				message = "Class, term, and fee category are required."
			});
		}
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT Amount FROM FeesCategory WHERE CategoryId = @CategoryId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@CategoryId", request.FeesCategoryId), new SqlParameter("@SchoolId", schoolId));
		if (readOnlyList.Count == 0)
		{
			return NotFound(new
			{
				message = "Fee category was not found."
			});
		}
		decimal amount = Convert.ToDecimal(readOnlyList[0]["Amount"]);
		int num = await database.ExecuteAsync("INSERT INTO Fees (fees_Category, TermId, studentId, SchoolId, totalFees, FeesStatus, InitalPaymentDate, amountPaid, balance, registeredBy, dateRegistered) SELECT @FeesCategoryId, @TermId, s.StudentID, @SchoolId, @TotalFees, 'Pending', GETDATE(), 0, @TotalFees, @RegisteredBy, GETDATE() FROM Students s WHERE s.CurrentClassID = @ClassId AND s.Status = 'Active' AND s.IsDeleted = 0 AND NOT EXISTS (SELECT 1 FROM Fees f WHERE f.studentId = s.StudentID AND f.TermId = @TermId AND f.SchoolId = @SchoolId AND f.fees_Category = @FeesCategoryId)", cancellationToken, new SqlParameter("@FeesCategoryId", request.FeesCategoryId), new SqlParameter("@TermId", request.TermId), new SqlParameter("@SchoolId", schoolId), new SqlParameter("@TotalFees", amount), new SqlParameter("@RegisteredBy", base.User.Identity?.Name ?? string.Empty), new SqlParameter("@ClassId", request.ClassId));
		IActionResult result;
		if (num != 0)
		{
			IActionResult actionResult = Ok(new
			{
				success = true,
				message = "Class fee invoices generated successfully.",
				inserted = num,
				amount = amount
			});
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound(new
			{
				message = "No active students were found, or invoices already exist for this class."
			});
			result = actionResult;
		}
		return result;
	}

	[HttpPost("individual")]
	public async Task<IActionResult> GenerateIndividual(IndividualFeeInvoiceRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (request.StudentId <= 0 || request.TermId <= 0 || request.FeesCategoryId <= 0)
		{
			return BadRequest(new
			{
				message = "Student, term, and fee category are required."
			});
		}
		IReadOnlyList<Dictionary<string, object?>> categoryRows = await database.QueryAsync("SELECT Amount FROM FeesCategory WHERE CategoryId = @CategoryId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@CategoryId", request.FeesCategoryId), new SqlParameter("@SchoolId", schoolId));
		if (categoryRows.Count == 0)
		{
			return NotFound(new
			{
				message = "Fee category was not found."
			});
		}
		if ((await database.QueryAsync("SELECT StudentID FROM Students WHERE StudentID = @StudentId AND SchoolId = @SchoolId AND IsDeleted = 0", cancellationToken, new SqlParameter("@StudentId", request.StudentId), new SqlParameter("@SchoolId", schoolId))).Count == 0)
		{
			return NotFound(new
			{
				message = "Student was not found."
			});
		}
		if ((await database.QueryAsync("SELECT FeesId FROM Fees WHERE studentId = @StudentId AND TermId = @TermId AND SchoolId = @SchoolId AND fees_Category = @FeesCategoryId", cancellationToken, new SqlParameter("@StudentId", request.StudentId), new SqlParameter("@TermId", request.TermId), new SqlParameter("@SchoolId", schoolId), new SqlParameter("@FeesCategoryId", request.FeesCategoryId))).Count > 0)
		{
			return Conflict(new
			{
				message = "An invoice already exists for this student, term, and fee category."
			});
		}
		decimal amount = Convert.ToDecimal(categoryRows[0]["Amount"]);
		int feesId = Convert.ToInt32((await database.QueryAsync("INSERT INTO Fees (fees_Category, TermId, studentId, SchoolId, totalFees, FeesStatus, InitalPaymentDate, amountPaid, balance, registeredBy, dateRegistered) OUTPUT INSERTED.FeesId AS FeesId VALUES (@FeesCategoryId, @TermId, @StudentId, @SchoolId, @TotalFees, 'Pending', GETDATE(), 0, @TotalFees, @RegisteredBy, GETDATE())", cancellationToken, new SqlParameter("@FeesCategoryId", request.FeesCategoryId), new SqlParameter("@TermId", request.TermId), new SqlParameter("@StudentId", request.StudentId), new SqlParameter("@SchoolId", schoolId), new SqlParameter("@TotalFees", amount), new SqlParameter("@RegisteredBy", base.User.Identity?.Name ?? string.Empty)))[0]["FeesId"]);
		return StatusCode(201, new
		{
			success = true,
			message = "Individual fee invoice created successfully.",
			feesId = feesId,
			amount = amount
		});
	}

	[HttpDelete("class/{classId:int}")]
	public async Task<IActionResult> DeleteClass(int classId, [FromQuery] int termId, [FromQuery] int feesCategoryId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (classId <= 0 || termId <= 0 || feesCategoryId <= 0)
		{
			return BadRequest(new
			{
				message = "Class, term, and fee category are required."
			});
		}
		return await DeleteInvoices("s.CurrentClassID = @ClassId", classId, termId, feesCategoryId, schoolId, "class", cancellationToken);
	}

	[HttpDelete("{feesId:int}")]
	public async Task<IActionResult> DeleteIndividual(int feesId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (feesId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid fee invoice is required."
			});
		}
		return await DeleteInvoices("f.FeesId = @FeesId", feesId, null, null, schoolId, "individual", cancellationToken);
	}

	private async Task<IActionResult> DeleteInvoices(string filter, int filterId, int? termId, int? feesCategoryId, int schoolId, string scope, CancellationToken cancellationToken)
	{
		IActionResult result;
		await using (SqlConnection connection = database.CreateConnection())
		{
			await connection.OpenAsync(cancellationToken);
			IActionResult actionResult;
			await using (SqlTransaction transaction = (SqlTransaction)(await connection.BeginTransactionAsync(cancellationToken)))
			{
				try
				{
					List<SqlParameter> list = new List<SqlParameter>
					{
						new SqlParameter("@FilterId", filterId),
						new SqlParameter("@SchoolId", schoolId)
					};
					string text = string.Empty;
					if (termId.HasValue)
					{
						text += " AND f.TermId = @TermId";
						list.Add(new SqlParameter("@TermId", termId.Value));
					}
					if (feesCategoryId.HasValue)
					{
						text += " AND f.fees_Category = @FeesCategoryId";
						list.Add(new SqlParameter("@FeesCategoryId", feesCategoryId.Value));
					}
					string cmdText = "SELECT f.FeesId FROM Fees f JOIN Students s ON s.StudentID = f.studentId WHERE " + filter.Replace("@ClassId", "@FilterId").Replace("@FeesId", "@FilterId") + " AND f.SchoolId = @SchoolId" + text;
					List<int> invoiceIds = new List<int>();
					await using (SqlCommand command = new SqlCommand(cmdText, connection, transaction))
					{
						command.Parameters.AddRange(list.ToArray());
						await using SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
						while (await reader.ReadAsync(cancellationToken))
						{
							invoiceIds.Add(reader.GetInt32(0));
						}
					}
					IActionResult actionResult2;
					if (invoiceIds.Count == 0)
					{
						await transaction.RollbackAsync(cancellationToken);
						actionResult = NotFound(new
						{
							message = "The " + scope + " fee invoice was not found."
						});
					}
					else
					{
						await using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Payments WHERE FeesId IN (" + string.Join(',', invoiceIds) + ") AND IsDeleted = 0", connection, transaction))
						{
							if (Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken)) > 0)
							{
								await transaction.RollbackAsync(cancellationToken);
								actionResult2 = Conflict(new
								{
									message = "Paid invoices cannot be deleted."
								});
								goto IL_0827;
							}
						}
						actionResult2 = null;
						await using (SqlCommand delete = new SqlCommand("DELETE FROM Fees WHERE FeesId IN (" + string.Join(',', invoiceIds) + ") AND SchoolId = @SchoolId", connection, transaction))
						{
							delete.Parameters.Add(new SqlParameter("@SchoolId", schoolId));
							await delete.ExecuteNonQueryAsync(cancellationToken);
							await transaction.CommitAsync(cancellationToken);
							actionResult2 = Ok(new
							{
								success = true,
								message = $"{invoiceIds.Count} {scope} fee invoice(s) deleted successfully.",
								deleted = invoiceIds.Count
							});
						}
						actionResult = actionResult2;
					}
					goto end_IL_01b9;
					IL_0827:
					actionResult = actionResult2;
					end_IL_01b9:;
				}
				catch (SqlException)
				{
					await transaction.RollbackAsync(cancellationToken);
					actionResult = Problem("The fee invoice could not be deleted.", null, 500, "Invoice deletion failed.");
				}
			}
			result = actionResult;
		}
		return result;
	}

	private bool TryGetSchoolId(out int schoolId)
	{
		if (int.TryParse(base.User.FindFirst("schoolId")?.Value, out schoolId))
		{
			return schoolId > 0;
		}
		return false;
	}
}
