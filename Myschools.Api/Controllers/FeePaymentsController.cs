using System;
using System.Collections.Generic;
using System.Globalization;
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
[Route("api/fee-payments")]
public sealed class FeePaymentsController(SqlDatabase database) : ControllerBase
{
	[HttpGet("{feesId:int}")]
	public async Task<IActionResult> GetFee(int feesId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT F.FeesId, S.StudentID, S.FirstName, S.Middlename, S.LastName, P.PhoneNumber, F.TermId, F.fees_Category, F.totalFees, F.amountPaid, F.balance, T.TermName, FC.CategoryName FROM Students S JOIN StudentParent P ON S.ParentID = P.ParentID JOIN Fees F ON F.studentId = S.StudentID JOIN SchoolTerm T ON T.TermId = F.TermId JOIN FeesCategory FC ON FC.CategoryId = F.fees_Category WHERE F.FeesId = @FeesId AND F.SchoolId = @SchoolId", cancellationToken, new SqlParameter("@FeesId", feesId), new SqlParameter("@SchoolId", schoolId));
		IActionResult result;
		if (readOnlyList.Count != 0)
		{
			IActionResult actionResult = Ok(readOnlyList[0]);
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound(new
			{
				message = "Fee invoice not found."
			});
			result = actionResult;
		}
		return result;
	}

	[HttpPost]
	public async Task<IActionResult> Pay(FeePaymentRequest request, CancellationToken cancellationToken)
	{
		if (request.Amount <= 0m)
		{
			return BadRequest(new
			{
				message = "Payment amount must be greater than zero."
			});
		}
		if (request.FeesId <= 0 || string.IsNullOrWhiteSpace(request.PaymentMode) || string.IsNullOrWhiteSpace(request.PaidBy))
		{
			return BadRequest(new
			{
				message = "Fee invoice, payment mode, and payer are required."
			});
		}
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		IActionResult result;
		await using (SqlConnection connection = database.CreateConnection())
		{
			await connection.OpenAsync(cancellationToken);
			IActionResult actionResult3;
			await using (SqlTransaction transaction = (SqlTransaction)(await connection.BeginTransactionAsync(cancellationToken)))
			{
				try
				{
					decimal amountPaid;
					decimal currentBalance;
					string schoolName;
					IActionResult actionResult2;
					await using (SqlCommand detailCommand = new SqlCommand("SELECT F.amountPaid, F.balance, A.School_name FROM Fees F JOIN AllSchools A ON A.SchoolId = F.SchoolId WHERE F.FeesId = @FeesId AND F.SchoolId = @SchoolId", connection, transaction))
					{
						detailCommand.Parameters.Add(new SqlParameter("@FeesId", request.FeesId));
						detailCommand.Parameters.Add(new SqlParameter("@SchoolId", schoolId));
						IActionResult actionResult;
						await using (SqlDataReader reader = await detailCommand.ExecuteReaderAsync(cancellationToken))
						{
							if (!(await reader.ReadAsync(cancellationToken)))
							{
								await transaction.RollbackAsync(cancellationToken);
								actionResult = NotFound(new
								{
									message = "Fee invoice not found."
								});
								goto IL_05e4;
							}
							amountPaid = Convert.ToDecimal(reader.GetValue(0), CultureInfo.InvariantCulture);
							currentBalance = Convert.ToDecimal(reader.GetValue(1), CultureInfo.InvariantCulture);
							schoolName = (reader.IsDBNull(2) ? string.Empty : reader.GetString(2));
						}
						goto end_IL_02f9;
						IL_05e4:
						actionResult2 = actionResult;
						goto IL_06c2;
						end_IL_02f9:;
					}
					actionResult2 = null;
					if (request.Amount > currentBalance)
					{
						await transaction.RollbackAsync(cancellationToken);
						actionResult3 = BadRequest(new
						{
							message = "Payment amount cannot be greater than the outstanding balance.",
							balance = currentBalance
						});
					}
					else
					{
						decimal newBalance = currentBalance - request.Amount;
						decimal newTotal = amountPaid + request.Amount;
						string receiptNumber = ReceiptNumber(schoolName, request.PaymentDate);
						await using (SqlCommand detailCommand = new SqlCommand("SELECT COUNT(*) FROM Payments WHERE FeesId = @FeesId AND Paid = @Paid AND paymentMode = @PaymentMode AND datePaid = @PaymentDate", connection, transaction))
						{
							detailCommand.Parameters.Add(new SqlParameter("@FeesId", request.FeesId));
							detailCommand.Parameters.Add(new SqlParameter("@Paid", request.Amount));
							detailCommand.Parameters.Add(new SqlParameter("@PaymentMode", request.PaymentMode.Trim()));
							detailCommand.Parameters.Add(new SqlParameter("@PaymentDate", request.PaymentDate));
							if (Convert.ToInt32(await detailCommand.ExecuteScalarAsync(cancellationToken)) > 0)
							{
								await transaction.RollbackAsync(cancellationToken);
								actionResult2 = Conflict(new
								{
									message = "Payment already made, please check."
								});
								goto IL_0a7d;
							}
						}
						actionResult2 = null;
						await using (SqlCommand detailCommand = new SqlCommand("UPDATE Fees SET FeesStatus = @FeesStatus, amountPaid = @AmountPaid, balance = @Balance WHERE FeesId = @FeesId AND SchoolId = @SchoolId", connection, transaction))
						{
							detailCommand.Parameters.Add(new SqlParameter("@FeesStatus", (newBalance > 0m) ? "Pending" : "Completed"));
							detailCommand.Parameters.Add(new SqlParameter("@AmountPaid", newTotal));
							detailCommand.Parameters.Add(new SqlParameter("@Balance", newBalance));
							detailCommand.Parameters.Add(new SqlParameter("@FeesId", request.FeesId));
							detailCommand.Parameters.Add(new SqlParameter("@SchoolId", schoolId));
							if (await detailCommand.ExecuteNonQueryAsync(cancellationToken) == 0)
							{
								await transaction.RollbackAsync(cancellationToken);
								actionResult2 = NotFound(new
								{
									message = "Fee invoice not found."
								});
								goto IL_0d65;
							}
						}
						actionResult2 = null;
						await using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Payments (FeesId, Paid, Balance, paymentMode, paidBy, paymentReference, receipNumber, datePaid, postedBy, dateRegistered, IsDeleted) VALUES (@FeesId, @Paid, @Balance, @PaymentMode, @PaidBy, @PaymentReference, @ReceiptNumber, @PaymentDate, @PostedBy, GETDATE(), 0); SELECT CAST(SCOPE_IDENTITY() AS int);", connection, transaction))
						{
							insertCommand.Parameters.Add(new SqlParameter("@FeesId", request.FeesId));
							insertCommand.Parameters.Add(new SqlParameter("@Paid", request.Amount));
							insertCommand.Parameters.Add(new SqlParameter("@Balance", newBalance));
							insertCommand.Parameters.Add(new SqlParameter("@PaymentMode", request.PaymentMode.Trim()));
							insertCommand.Parameters.Add(new SqlParameter("@PaidBy", request.PaidBy.Trim()));
							insertCommand.Parameters.Add(new SqlParameter("@PaymentReference", ((object)request.PaymentReference?.Trim()) ?? ((object)DBNull.Value)));
							insertCommand.Parameters.Add(new SqlParameter("@ReceiptNumber", receiptNumber));
							insertCommand.Parameters.Add(new SqlParameter("@PaymentDate", request.PaymentDate));
							insertCommand.Parameters.Add(new SqlParameter("@PostedBy", base.User.Identity?.Name ?? string.Empty));
							int transactionId = Convert.ToInt32(await insertCommand.ExecuteScalarAsync(cancellationToken));
							await transaction.CommitAsync(cancellationToken);
							actionResult2 = Ok(new
							{
								success = true,
								message = "Fee payment recorded successfully.",
								transactionId = transactionId,
								receiptNumber = receiptNumber,
								amountPaid = newTotal,
								balance = newBalance,
								status = ((newBalance > 0m) ? "Pending" : "Completed")
							});
						}
						actionResult3 = actionResult2;
					}
					goto end_IL_02b6;
					IL_06c2:
					actionResult3 = actionResult2;
					goto end_IL_02b6;
					IL_0d65:
					actionResult3 = actionResult2;
					goto end_IL_02b6;
					IL_0a7d:
					actionResult3 = actionResult2;
					end_IL_02b6:;
				}
				catch (SqlException ex) when (((Func<bool>)delegate
				{
					// Could not convert BlockContainer to single expression
					int number = ex.Number;
					return ((number == 2601 || number == 2627) ? 1 : 0) != 0;
				}).Invoke())
				{
					await transaction.RollbackAsync(cancellationToken);
					actionResult3 = Conflict(new
					{
						message = "This payment has already been recorded."
					});
				}
				catch (SqlException)
				{
					await transaction.RollbackAsync(cancellationToken);
					actionResult3 = Problem("The fee payment could not be recorded.", null, 500, "Fee payment failed.");
				}
			}
			result = actionResult3;
		}
		return result;
	}

	[HttpPut("{transactionId:int}")]
	public async Task<IActionResult> Update(int transactionId, FeePaymentRequest request, CancellationToken cancellationToken)
	{
		if (transactionId <= 0 || request.Amount <= 0m || request.FeesId <= 0 || string.IsNullOrWhiteSpace(request.PaymentMode) || string.IsNullOrWhiteSpace(request.PaidBy))
		{
			return BadRequest(new
			{
				message = "Payment, fee invoice, amount, payment mode, and payer are required."
			});
		}
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		IActionResult result;
		await using (SqlConnection connection = database.CreateConnection())
		{
			await connection.OpenAsync(cancellationToken);
			IActionResult actionResult3;
			await using (SqlTransaction transaction = (SqlTransaction)(await connection.BeginTransactionAsync(cancellationToken)))
			{
				try
				{
					int oldFeesId;
					decimal totalFees;
					IActionResult actionResult2;
					await using (SqlCommand command = new SqlCommand("SELECT P.FeesId, P.Paid, F.totalFees FROM Payments P JOIN Fees F ON F.FeesId = P.FeesId WHERE P.TransanctionId = @TransactionId AND P.IsDeleted = 0 AND F.SchoolId = @SchoolId", connection, transaction))
					{
						command.Parameters.Add(new SqlParameter("@TransactionId", transactionId));
						command.Parameters.Add(new SqlParameter("@SchoolId", schoolId));
						IActionResult actionResult;
						await using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
						{
							if (!(await reader.ReadAsync(cancellationToken)))
							{
								await transaction.RollbackAsync(cancellationToken);
								actionResult = NotFound(new
								{
									message = "Payment not found."
								});
								goto IL_0574;
							}
							oldFeesId = reader.GetInt32(0);
							Convert.ToDecimal(reader.GetValue(1), CultureInfo.InvariantCulture);
							totalFees = Convert.ToDecimal(reader.GetValue(2), CultureInfo.InvariantCulture);
						}
						goto end_IL_02a7;
						IL_0574:
						actionResult2 = actionResult;
						goto IL_0652;
						end_IL_02a7:;
					}
					actionResult2 = null;
					if (request.FeesId != oldFeesId)
					{
						await transaction.RollbackAsync(cancellationToken);
						actionResult3 = BadRequest(new
						{
							message = "A payment cannot be moved to another fee invoice."
						});
					}
					else
					{
						decimal newAmountPaid = await ActivePaid(connection, transaction, oldFeesId, transactionId, cancellationToken) + request.Amount;
						if (newAmountPaid > totalFees)
						{
							await transaction.RollbackAsync(cancellationToken);
							actionResult3 = BadRequest(new
							{
								message = "Payment amount cannot exceed the invoice balance.",
								balance = totalFees - (newAmountPaid - request.Amount)
							});
						}
						else
						{
							decimal newBalance = totalFees - newAmountPaid;
							await using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Payments WHERE FeesId = @FeesId AND Paid = @Paid AND paymentMode = @PaymentMode AND datePaid = @PaymentDate AND TransanctionId <> @TransactionId AND IsDeleted = 0", connection, transaction))
							{
								command.Parameters.Add(new SqlParameter("@FeesId", oldFeesId));
								command.Parameters.Add(new SqlParameter("@Paid", request.Amount));
								command.Parameters.Add(new SqlParameter("@PaymentMode", request.PaymentMode.Trim()));
								command.Parameters.Add(new SqlParameter("@PaymentDate", request.PaymentDate));
								command.Parameters.Add(new SqlParameter("@TransactionId", transactionId));
								if (Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken)) > 0)
								{
									await transaction.RollbackAsync(cancellationToken);
									actionResult2 = Conflict(new
									{
										message = "This payment already exists."
									});
									goto IL_0b39;
								}
							}
							await using (SqlCommand command = new SqlCommand("UPDATE Payments SET Paid = @Paid, Balance = @Balance, paymentMode = @PaymentMode, paidBy = @PaidBy, paymentReference = @PaymentReference, datePaid = @PaymentDate WHERE TransanctionId = @TransactionId AND IsDeleted = 0", connection, transaction))
							{
								command.Parameters.Add(new SqlParameter("@Paid", request.Amount));
								command.Parameters.Add(new SqlParameter("@Balance", newBalance));
								command.Parameters.Add(new SqlParameter("@PaymentMode", request.PaymentMode.Trim()));
								command.Parameters.Add(new SqlParameter("@PaidBy", request.PaidBy.Trim()));
								command.Parameters.Add(new SqlParameter("@PaymentReference", ((object)request.PaymentReference?.Trim()) ?? ((object)DBNull.Value)));
								command.Parameters.Add(new SqlParameter("@PaymentDate", request.PaymentDate));
								command.Parameters.Add(new SqlParameter("@TransactionId", transactionId));
								await command.ExecuteNonQueryAsync(cancellationToken);
							}
							await UpdateFeeTotals(connection, transaction, oldFeesId, totalFees, newBalance, cancellationToken);
							await transaction.CommitAsync(cancellationToken);
							actionResult3 = Ok(new
							{
								success = true,
								message = "Fee payment updated successfully.",
								transactionId = transactionId,
								amountPaid = newAmountPaid,
								balance = newBalance,
								status = ((newBalance > 0m) ? "Pending" : "Completed")
							});
						}
					}
					goto end_IL_0264;
					IL_0652:
					actionResult3 = actionResult2;
					goto end_IL_0264;
					IL_0b39:
					actionResult3 = actionResult2;
					end_IL_0264:;
				}
				catch (SqlException)
				{
					await transaction.RollbackAsync(cancellationToken);
					actionResult3 = Problem("The fee payment could not be updated.", null, 500, "Fee payment update failed.");
				}
			}
			result = actionResult3;
		}
		return result;
	}

	[HttpDelete("{transactionId:int}")]
	public async Task<IActionResult> Delete(int transactionId, CancellationToken cancellationToken)
	{
		if (transactionId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid payment is required."
			});
		}
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		IActionResult result;
		await using (SqlConnection connection = database.CreateConnection())
		{
			await connection.OpenAsync(cancellationToken);
			IActionResult actionResult2;
			await using (SqlTransaction transaction = (SqlTransaction)(await connection.BeginTransactionAsync(cancellationToken)))
			{
				try
				{
					IActionResult actionResult;
					int feesId;
					await using (SqlCommand command = new SqlCommand("SELECT P.FeesId FROM Payments P JOIN Fees F ON F.FeesId = P.FeesId WHERE P.TransanctionId = @TransactionId AND P.IsDeleted = 0 AND F.SchoolId = @SchoolId", connection, transaction))
					{
						command.Parameters.Add(new SqlParameter("@TransactionId", transactionId));
						command.Parameters.Add(new SqlParameter("@SchoolId", schoolId));
						object obj = await command.ExecuteScalarAsync(cancellationToken);
						if (obj == null)
						{
							await transaction.RollbackAsync(cancellationToken);
							actionResult = NotFound(new
							{
								message = "Payment not found."
							});
							goto IL_0423;
						}
						feesId = Convert.ToInt32(obj);
					}
					await using (SqlCommand command = new SqlCommand("UPDATE Payments SET IsDeleted = 1 WHERE TransanctionId = @TransactionId", connection, transaction))
					{
						command.Parameters.Add(new SqlParameter("@TransactionId", transactionId));
						await command.ExecuteNonQueryAsync(cancellationToken);
					}
					(decimal totalFees, decimal amountPaid, decimal balance) totals = await GetFeeTotals(connection, transaction, feesId, cancellationToken);
					await UpdateFeeTotals(connection, transaction, feesId, totals.totalFees, totals.balance, cancellationToken);
					await transaction.CommitAsync(cancellationToken);
					actionResult2 = Ok(new
					{
						success = true,
						message = "Fee payment deleted successfully.",
						transactionId = transactionId,
						amountPaid = totals.amountPaid,
						balance = totals.balance,
						status = ((totals.balance > 0m) ? "Pending" : "Completed")
					});
					goto end_IL_01df;
					IL_0423:
					actionResult2 = actionResult;
					end_IL_01df:;
				}
				catch (SqlException)
				{
					await transaction.RollbackAsync(cancellationToken);
					actionResult2 = Problem("The fee payment could not be deleted.", null, 500, "Fee payment deletion failed.");
				}
			}
			result = actionResult2;
		}
		return result;
	}

	private static async Task<decimal> ActivePaid(SqlConnection connection, SqlTransaction transaction, int feesId, int excludedTransactionId, CancellationToken cancellationToken)
	{
		decimal result;
		await using (SqlCommand command = new SqlCommand("SELECT COALESCE(SUM(Paid), 0) FROM Payments WHERE FeesId = @FeesId AND TransanctionId <> @TransactionId AND IsDeleted = 0", connection, transaction))
		{
			command.Parameters.Add(new SqlParameter("@FeesId", feesId));
			command.Parameters.Add(new SqlParameter("@TransactionId", excludedTransactionId));
			result = Convert.ToDecimal(await command.ExecuteScalarAsync(cancellationToken), CultureInfo.InvariantCulture);
		}
		return result;
	}

	private static async Task<(decimal totalFees, decimal amountPaid, decimal balance)> GetFeeTotals(SqlConnection connection, SqlTransaction transaction, int feesId, CancellationToken cancellationToken)
	{
		(decimal totalFees, decimal amountPaid, decimal balance) result;
		await using (SqlCommand command = new SqlCommand("SELECT totalFees, COALESCE((SELECT SUM(Paid) FROM Payments WHERE FeesId = @FeesId AND IsDeleted = 0), 0) FROM Fees WHERE FeesId = @FeesId", connection, transaction))
		{
			command.Parameters.Add(new SqlParameter("@FeesId", feesId));
			(decimal totalFees, decimal amountPaid, decimal balance) tuple;
			await using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
			{
				await reader.ReadAsync(cancellationToken);
				decimal num = Convert.ToDecimal(reader.GetValue(0), CultureInfo.InvariantCulture);
				decimal num2 = Convert.ToDecimal(reader.GetValue(1), CultureInfo.InvariantCulture);
				tuple = (totalFees: num, amountPaid: num2, balance: num - num2);
			}
			result = tuple;
		}
		return result;
	}

	private static async Task UpdateFeeTotals(SqlConnection connection, SqlTransaction transaction, int feesId, decimal totalFees, decimal balance, CancellationToken cancellationToken)
	{
		await using SqlCommand command = new SqlCommand("UPDATE Fees SET amountPaid = @AmountPaid, balance = @Balance, FeesStatus = @FeesStatus WHERE FeesId = @FeesId", connection, transaction);
		command.Parameters.Add(new SqlParameter("@AmountPaid", totalFees - balance));
		command.Parameters.Add(new SqlParameter("@Balance", balance));
		command.Parameters.Add(new SqlParameter("@FeesStatus", (balance > 0m) ? "Pending" : "Completed"));
		command.Parameters.Add(new SqlParameter("@FeesId", feesId));
		await command.ExecuteNonQueryAsync(cancellationToken);
	}

	private bool TryGetSchoolId(out int schoolId)
	{
		if (int.TryParse(base.User.FindFirst("schoolId")?.Value, out schoolId))
		{
			return schoolId > 0;
		}
		return false;
	}

	private static string ReceiptNumber(string schoolName, DateTime date)
	{
		if (string.IsNullOrEmpty(schoolName)) return date.ToString("ddMMyy-HHmmss");
		string prefix = schoolName.Length < 2 ? schoolName : schoolName.Substring(0, 2);
		return $"{prefix}{date:ddMMyy-HHmmss}";
	}
}
