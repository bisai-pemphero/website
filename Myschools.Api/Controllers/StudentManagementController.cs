using System;
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
[Route("api/student-management")]
public sealed class StudentManagementController(SqlDatabase database) : ControllerBase
{
	[HttpGet("deleted")]
	public async Task<IActionResult> Deleted(CancellationToken cancellationToken)
	{
		int num = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		return Ok(await database.QueryAsync("SELECT S.StudentID, S.FirstName, S.Middlename, S.LastName, S.Gender, C.ClassName FROM Students S JOIN Classes C ON C.ClassID = S.CurrentClassID WHERE S.IsDeleted = 1 AND S.SchoolId = @SchoolId", cancellationToken, new SqlParameter("@SchoolId", num)));
	}

	[HttpPut("{studentId:int}")]
	public async Task<IActionResult> Update(int studentId, StudentUpdateRequest request, CancellationToken cancellationToken)
	{
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
					IActionResult actionResult;
					await using (SqlCommand parent = new SqlCommand("UPDATE StudentParent SET FullName = @ParentFullName, Gender = @ParentGender, PhoneNumber = @ParentPhone, AlternatePhone = @ParentAlternatePhone, Email = @ParentEmail, Address = @ParentAddress, Relationship = @ParentRelationship, Occupation = @ParentOccupation, UpdatedAt = GETDATE() WHERE ParentID = @ParentId", connection, transaction))
					{
						parent.Parameters.AddRange(ParentParameters(request));
						if (await parent.ExecuteNonQueryAsync(cancellationToken) == 0)
						{
							await transaction.RollbackAsync(cancellationToken);
							actionResult = NotFound(new
							{
								message = "Parent record not found."
							});
						}
						else
						{
							IActionResult actionResult2;
							await using (SqlCommand student = new SqlCommand("UPDATE Students SET FirstName = @FirstName, Middlename = @MiddleName, LastName = @LastName, Gender = @Gender, DateOfBirth = @DateOfBirth, AdmissionDate = @AdmissionDate, CurrentClassID = @CurrentClassId, PreviousclassID = @PreviousClassId, PrevSchool = @PreviousSchool, SpecialNeeds = @SpecialNeeds, UpdatedAt = GETDATE() WHERE StudentID = @StudentId AND SchoolId = @SchoolId", connection, transaction))
							{
								student.Parameters.AddRange(StudentParameters(request));
								student.Parameters.Add(new SqlParameter("@StudentId", studentId));
								student.Parameters.Add(new SqlParameter("@SchoolId", schoolId));
								if (await student.ExecuteNonQueryAsync(cancellationToken) == 0)
								{
									await transaction.RollbackAsync(cancellationToken);
									actionResult2 = NotFound();
								}
								else
								{
									await transaction.CommitAsync(cancellationToken);
									actionResult2 = Ok(new
									{
										message = "Student updated successfully."
									});
								}
							}
							actionResult = actionResult2;
						}
					}
					actionResult3 = actionResult;
				}
				catch
				{
					await transaction.RollbackAsync(cancellationToken);
					throw;
				}
			}
			result = actionResult3;
		}
		return result;
	}

	[HttpPost("{studentId:int}/recover")]
	public async Task<IActionResult> Recover(int studentId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		IActionResult result;
		if (await database.ExecuteAsync("UPDATE Students SET IsDeleted = 0, Status = 'Active', UpdatedAt = GETDATE() WHERE StudentID = @StudentId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@StudentId", studentId), new SqlParameter("@SchoolId", schoolId)) != 0)
		{
			IActionResult actionResult = Ok(new
			{
				message = "Student recovered successfully."
			});
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound(new
			{
				message = "Student not found."
			});
			result = actionResult;
		}
		return result;
	}

	[HttpDelete("{studentId:int}")]
	public async Task<IActionResult> Delete(int studentId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		IActionResult result;
		if (await database.ExecuteAsync("UPDATE Students SET IsDeleted = 1, Status = 'Inactive', UpdatedAt = GETDATE() WHERE StudentID = @StudentId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@StudentId", studentId), new SqlParameter("@SchoolId", schoolId)) != 0)
		{
			IActionResult actionResult = Ok(new
			{
				message = "Student deleted successfully."
			});
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound(new
			{
				message = "Student not found."
			});
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

	private static SqlParameter[] ParentParameters(StudentUpdateRequest request)
	{
		return new SqlParameter[9]
		{
			new SqlParameter("@ParentFullName", request.ParentFullName.Trim()),
			new SqlParameter("@ParentGender", request.ParentGender.Trim()),
			new SqlParameter("@ParentPhone", request.ParentPhone.Trim()),
			new SqlParameter("@ParentAlternatePhone", ((object)request.ParentAlternatePhone?.Trim()) ?? ((object)DBNull.Value)),
			new SqlParameter("@ParentEmail", ((object)request.ParentEmail?.Trim()) ?? ((object)DBNull.Value)),
			new SqlParameter("@ParentAddress", ((object)request.ParentAddress?.Trim()) ?? ((object)DBNull.Value)),
			new SqlParameter("@ParentRelationship", request.ParentRelationship.Trim()),
			new SqlParameter("@ParentOccupation", ((object)request.ParentOccupation?.Trim()) ?? ((object)DBNull.Value)),
			new SqlParameter("@ParentId", request.ParentId)
		};
	}

	private static SqlParameter[] StudentParameters(StudentUpdateRequest request)
	{
		return new SqlParameter[10]
		{
			new SqlParameter("@FirstName", request.FirstName.Trim()),
			new SqlParameter("@MiddleName", ((object)request.MiddleName?.Trim()) ?? ((object)DBNull.Value)),
			new SqlParameter("@LastName", request.LastName.Trim()),
			new SqlParameter("@Gender", request.Gender.Trim()),
			new SqlParameter("@DateOfBirth", request.DateOfBirth.Date),
			new SqlParameter("@AdmissionDate", request.AdmissionDate.Date),
			new SqlParameter("@CurrentClassId", request.CurrentClassId),
			new SqlParameter("@PreviousClassId", request.PreviousClassId),
			new SqlParameter("@PreviousSchool", ((object)request.PreviousSchool?.Trim()) ?? ((object)DBNull.Value)),
			new SqlParameter("@SpecialNeeds", ((object)request.SpecialNeeds?.Trim()) ?? ((object)DBNull.Value))
		};
	}
}
