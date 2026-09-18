using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Myschools.Api.Data;
using Myschools.Api.Models;

namespace Myschools.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/school-registration")]
public sealed class SchoolRegistrationController(SqlDatabase database, LegacyPasswordCipher cipher, IConfiguration configuration, IWebHostEnvironment environment) : ControllerBase
{
	[HttpPost]
	[Consumes("multipart/form-data", new string[] { })]
	[RequestSizeLimit(10485760L)]
	public async Task<IActionResult> Register([FromForm] SchoolRegistrationRequest request, CancellationToken cancellationToken)
	{
		if (!new MailAddress(request.AdminEmail).Address.Equals(request.AdminEmail, StringComparison.OrdinalIgnoreCase))
		{
			return BadRequest(new
			{
				message = "Please enter a valid email."
			});
		}
		if (request.Logo == null || request.Letterhead == null)
		{
			return BadRequest(new
			{
				message = "Logo and letterhead files are required."
			});
		}
		if (!AllowedImage(request.Logo.FileName) || !AllowedImage(request.Letterhead.FileName))
		{
			return BadRequest(new
			{
				message = "Only JPG, JPEG, and PNG files are supported."
			});
		}
		string logoName = SafeFileName(request.Logo.FileName);
		string letterheadName = SafeFileName(request.Letterhead.FileName);
		string storagePath = configuration["Storage:SchoolDocsPath"] ?? Path.Combine(environment.ContentRootPath, "..", "img", "Schooldocs");
		Directory.CreateDirectory(storagePath);
		int schoolId = 0;
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
					await using (SqlCommand duplicate = new SqlCommand("SELECT COUNT(*) FROM AllSchools WHERE School_name = @SchoolName AND Admin_name = @AdminName", connection, transaction))
					{
						duplicate.Parameters.Add(new SqlParameter("@SchoolName", request.SchoolName.Trim()));
						duplicate.Parameters.Add(new SqlParameter("@AdminName", request.AdminName.Trim()));
						if (Convert.ToInt32(await duplicate.ExecuteScalarAsync(cancellationToken)) > 0)
						{
							actionResult = Conflict(new
							{
								message = "This school already exists."
							});
							goto IL_04c4;
						}
					}
					actionResult = null;
					await using (SqlCommand duplicate = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection, transaction))
					{
						duplicate.Parameters.Add(new SqlParameter("@Username", request.AdminEmail.Trim()));
						if (Convert.ToInt32(await duplicate.ExecuteScalarAsync(cancellationToken)) > 0)
						{
							actionResult = Conflict(new
							{
								message = "Please use a different email address."
							});
							goto IL_068c;
						}
					}
					await using (SqlCommand duplicate = new SqlCommand("INSERT INTO AllSchools (School_name, School_email, PhoneNumber, School_address, Slogan, Logo, Letterhead, Admin_name, Admin_email, Admin_phone, Admin_password, Date_Registered, RegisteredBy) VALUES (@SchoolName, @SchoolEmail, @PhoneNumber, @SchoolAddress, @Slogan, @Logo, @Letterhead, @AdminName, @AdminEmail, @AdminPhone, @AdminPassword, GETDATE(), @RegisteredBy); SELECT CAST(SCOPE_IDENTITY() AS int);", connection, transaction))
					{
						duplicate.Parameters.AddRange(new SqlParameter[12]
						{
							new SqlParameter("@SchoolName", request.SchoolName.Trim()),
							new SqlParameter("@SchoolEmail", request.SchoolEmail.Trim()),
							new SqlParameter("@PhoneNumber", request.PhoneNumber.Trim()),
							new SqlParameter("@SchoolAddress", request.SchoolAddress.Trim()),
							new SqlParameter("@Slogan", request.Slogan.Trim()),
							new SqlParameter("@Logo", logoName),
							new SqlParameter("@Letterhead", letterheadName),
							new SqlParameter("@AdminName", request.AdminName.Trim()),
							new SqlParameter("@AdminEmail", request.AdminEmail.Trim()),
							new SqlParameter("@AdminPhone", request.AdminPhone.Trim()),
							new SqlParameter("@AdminPassword", request.AdminPassword),
							new SqlParameter("@RegisteredBy", base.User.Identity?.Name ?? string.Empty)
						});
						schoolId = Convert.ToInt32(await duplicate.ExecuteScalarAsync(cancellationToken));
					}
					await SqlDatabase.ExecuteAsync(connection, transaction, "INSERT INTO Users (Username, Password, Fullname, RoleId, Created_On, SchoolId) VALUES (@Username, @Password, @Fullname, 2, GETDATE(), @SchoolId)", cancellationToken, new SqlParameter("@Username", request.AdminEmail.Trim()), new SqlParameter("@Password", cipher.EncryptForLegacyLogin(request.AdminPassword)), new SqlParameter("@Fullname", request.AdminName.Trim()), new SqlParameter("@SchoolId", schoolId));
					DateTime now = DateTime.Now;
					DateTime expiryDate = now.AddDays(100.0);
					string value = cipher.EncryptForLegacyLogin($"{now}{schoolId}{expiryDate}");
					await SqlDatabase.ExecuteAsync(connection, transaction, "INSERT INTO School_Licence (LicenceKey, Status, Mode, SchoolId, Directors_Email, DateUpdated, ExpiryDate, Postedby, DatePosted) VALUES (@LicenceKey, 'Active', 'Freemium', @SchoolId, @Email, @StartDate, @ExpiryDate, @PostedBy, GETDATE())", cancellationToken, new SqlParameter("@LicenceKey", value), new SqlParameter("@SchoolId", schoolId), new SqlParameter("@Email", request.AdminEmail.Trim()), new SqlParameter("@StartDate", now), new SqlParameter("@ExpiryDate", expiryDate), new SqlParameter("@PostedBy", base.User.Identity?.Name ?? string.Empty));
					await SqlDatabase.ExecuteAsync(connection, transaction, "INSERT INTO sms (SchoolId, Purchased, Used, Balance) VALUES (@SchoolId, 0, 0, 0)", cancellationToken, new SqlParameter("@SchoolId", schoolId));
					await transaction.CommitAsync(cancellationToken);
					await SaveFile(request.Logo, Path.Combine(storagePath, logoName), cancellationToken);
					await SaveFile(request.Letterhead, Path.Combine(storagePath, letterheadName), cancellationToken);
					actionResult2 = CreatedAtAction("Get", "Schools", new { schoolId }, new
					{
						schoolId = schoolId,
						username = request.AdminEmail,
						licenceExpiry = expiryDate
					});
					goto end_IL_02f4;
					IL_04c4:
					actionResult2 = actionResult;
					goto end_IL_02f4;
					IL_068c:
					actionResult2 = actionResult;
					end_IL_02f4:;
				}
				catch
				{
					await transaction.RollbackAsync(cancellationToken);
					throw;
				}
			}
			result = actionResult2;
		}
		return result;
	}

	private static bool AllowedImage(string fileName)
	{
		return Enumerable.Contains(new string[3] { ".jpg", ".jpeg", ".png" }, Path.GetExtension(fileName).ToLowerInvariant());
	}

	private static string SafeFileName(string fileName)
	{
		return Path.GetFileName(fileName).Replace(" ", "_");
	}

	private static async Task SaveFile(IFormFile file, string path, CancellationToken cancellationToken)
	{
		await using FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
		await file.CopyToAsync(stream, cancellationToken);
	}
}
