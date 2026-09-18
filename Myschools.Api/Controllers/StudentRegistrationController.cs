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
[Route("api/student-registration")]
public sealed class StudentRegistrationController(SqlDatabase database) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Register(StudentRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.StudentGender) || string.IsNullOrWhiteSpace(request.ParentFullName) || string.IsNullOrWhiteSpace(request.ParentPhone) || string.IsNullOrWhiteSpace(request.ParentGender) || string.IsNullOrWhiteSpace(request.ParentRelationship))
		{
			return BadRequest(new
			{
				message = "Student and parent required fields must be provided."
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
					await using (SqlCommand duplicateCommand = new SqlCommand("SELECT COUNT(*) FROM Students WHERE FirstName = @FirstName AND Middlename = @MiddleName AND LastName = @LastName AND Gender = @Gender AND DateOfBirth = @DateOfBirth AND SchoolId = @SchoolId AND IsDeleted = 0", connection, transaction))
					{
						duplicateCommand.Parameters.AddRange(StudentParameters(request, schoolId));
						int num = Convert.ToInt32(await duplicateCommand.ExecuteScalarAsync(cancellationToken));
						if (num > 0)
						{
							await transaction.RollbackAsync(cancellationToken);
							actionResult = Conflict(new
							{
								message = "This student already exists, try searching him/her."
							});
							goto IL_04cc;
						}
					}
					actionResult = null;
					int parentId;
					await using (SqlCommand duplicateCommand = new SqlCommand("INSERT INTO StudentParent (FullName, Gender, PhoneNumber, AlternatePhone, Email, Address, Relationship, Occupation, CreatedAt, UpdatedAt) VALUES (@FullName, @Gender, @Phone, @AlternatePhone, @Email, @Address, @Relationship, @Occupation, GETDATE(), GETDATE()); SELECT CAST(SCOPE_IDENTITY() AS int);", connection, transaction))
					{
						duplicateCommand.Parameters.AddRange(ParentParameters(request));
						parentId = Convert.ToInt32(await duplicateCommand.ExecuteScalarAsync(cancellationToken));
					}
					string schoolName;
					await using (SqlCommand duplicateCommand = new SqlCommand("SELECT School_name FROM AllSchools WHERE SchoolId = @SchoolId", connection, transaction))
					{
						duplicateCommand.Parameters.Add(new SqlParameter("@SchoolId", schoolId));
						object obj = await duplicateCommand.ExecuteScalarAsync(cancellationToken);
						if (obj == null || obj == DBNull.Value)
						{
							await transaction.RollbackAsync(cancellationToken);
							actionResult = NotFound(new
							{
								message = "School not found."
							});
							goto IL_0895;
						}
						schoolName = Convert.ToString(obj) ?? string.Empty;
					}
					actionResult = null;
					string prefix = SchoolPrefix(schoolName);
					string admissionNo = string.Empty;
					for (int i = 0; i < 20; i++)
					{
						admissionNo = $"{prefix}-{request.CurrentClassId}{Random.Shared.Next(100, 1000)}";
						await using (SqlCommand duplicateCommand = new SqlCommand("SELECT COUNT(*) FROM Students WHERE AdmissionNo = @AdmissionNo AND SchoolId = @SchoolId", connection, transaction))
						{
							duplicateCommand.Parameters.Add(new SqlParameter("@AdmissionNo", admissionNo));
							duplicateCommand.Parameters.Add(new SqlParameter("@SchoolId", schoolId));
							if (Convert.ToInt32(await duplicateCommand.ExecuteScalarAsync(cancellationToken)) != 0)
							{
								if (i == 19)
								{
									throw new InvalidOperationException("Could not generate a unique admission number.");
								}
								continue;
							}
						}
						break;
					}
					await using (SqlCommand duplicateCommand = new SqlCommand("INSERT INTO Students (AdmissionNo, FirstName, Middlename, LastName, Gender, DateOfBirth, AdmissionDate, CurrentClassID, PreviousclassID, PrevSchool, SpecialNeeds, SchoolId, Status, ParentID, IsDeleted, CreatedAt, UpdatedAt, RegisteredBy) VALUES (@AdmissionNo, @FirstName, @MiddleName, @LastName, @Gender, @DateOfBirth, @AdmissionDate, @CurrentClassId, @PreviousClassId, @PreviousSchool, @SpecialNeeds, @SchoolId, 'Active', @ParentId, 0, GETDATE(), GETDATE(), @RegisteredBy); SELECT CAST(SCOPE_IDENTITY() AS int);", connection, transaction))
					{
						duplicateCommand.Parameters.Add(new SqlParameter("@AdmissionNo", admissionNo));
						duplicateCommand.Parameters.AddRange(StudentParameters(request, schoolId));
						duplicateCommand.Parameters.Add(new SqlParameter("@AdmissionDate", request.AdmissionDate));
						duplicateCommand.Parameters.Add(new SqlParameter("@CurrentClassId", request.CurrentClassId));
						duplicateCommand.Parameters.Add(new SqlParameter("@PreviousClassId", request.PreviousClassId));
						duplicateCommand.Parameters.Add(new SqlParameter("@PreviousSchool", ((object)request.PreviousSchool) ?? ((object)DBNull.Value)));
						duplicateCommand.Parameters.Add(new SqlParameter("@SpecialNeeds", ((object)request.SpecialNeeds) ?? ((object)DBNull.Value)));
						duplicateCommand.Parameters.Add(new SqlParameter("@ParentId", parentId));
						duplicateCommand.Parameters.Add(new SqlParameter("@RegisteredBy", base.User.Identity?.Name ?? string.Empty));
						int studentId = Convert.ToInt32(await duplicateCommand.ExecuteScalarAsync(cancellationToken));
						await transaction.CommitAsync(cancellationToken);
						actionResult = CreatedAtAction("GetById", "Students", new
						{
							studentId = studentId,
							includeDeleted = false
						}, new
						{
							success = true,
							message = "Student registered successfully.",
							studentId = studentId,
							admissionNo = admissionNo
						});
					}
					actionResult2 = actionResult;
					goto end_IL_02bd;
					IL_04cc:
					actionResult2 = actionResult;
					goto end_IL_02bd;
					IL_0895:
					actionResult2 = actionResult;
					end_IL_02bd:;
				}
				catch (SqlException ex) when (((Func<bool>)delegate
				{
					// Could not convert BlockContainer to single expression
					int number = ex.Number;
					return ((number == 2601 || number == 2627) ? 1 : 0) != 0;
				}).Invoke())
				{
					await transaction.RollbackAsync(cancellationToken);
					actionResult2 = Conflict(new
					{
						message = "A student with the same details or admission number already exists."
					});
				}
				catch (SqlException)
				{
					await transaction.RollbackAsync(cancellationToken);
					actionResult2 = Problem("The student could not be registered.", null, 500, "Student registration failed.");
				}
			}
			result = actionResult2;
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

	private static SqlParameter[] StudentParameters(StudentRequest request, int schoolId)
	{
		return new SqlParameter[6]
		{
			new SqlParameter("@FirstName", request.FirstName.Trim()),
			new SqlParameter("@MiddleName", ((object)request.MiddleName?.Trim()) ?? ((object)DBNull.Value)),
			new SqlParameter("@LastName", request.LastName.Trim()),
			new SqlParameter("@Gender", request.StudentGender.Trim()),
			new SqlParameter("@DateOfBirth", request.DateOfBirth.Date),
			new SqlParameter("@SchoolId", schoolId)
		};
	}

	private static SqlParameter[] ParentParameters(StudentRequest request)
	{
		return new SqlParameter[8]
		{
			new SqlParameter("@FullName", request.ParentFullName.Trim()),
			new SqlParameter("@Gender", request.ParentGender.Trim()),
			new SqlParameter("@Phone", request.ParentPhone.Trim()),
			new SqlParameter("@AlternatePhone", ((object)request.ParentAlternatePhone?.Trim()) ?? ((object)DBNull.Value)),
			new SqlParameter("@Email", ((object)request.ParentEmail?.Trim()) ?? ((object)DBNull.Value)),
			new SqlParameter("@Address", ((object)request.ParentAddress?.Trim()) ?? ((object)DBNull.Value)),
			new SqlParameter("@Relationship", request.ParentRelationship.Trim()),
			new SqlParameter("@Occupation", ((object)request.ParentOccupation?.Trim()) ?? ((object)DBNull.Value))
		};
	}

	private static string SchoolPrefix(string schoolName)
	{
		if (schoolName.Length < 3)
		{
			return schoolName;
		}
		return schoolName.Substring(0, 2) + schoolName[schoolName.Length - 1];
	}
}
