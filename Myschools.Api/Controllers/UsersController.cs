using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
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
[Route("api/users")]
public sealed class UsersController(SqlDatabase database, LegacyPasswordCipher passwordCipher) : ControllerBase
{
		[HttpGet]
		public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		{
			// SystemAdmin can see all users, others see only their school's users
			int roleId = User.GetRoleId();
			if (roleId == 1) // SystemAdmin
			{
				return Ok(await database.QueryAsync("SELECT U.UserId, U.Username, U.Fullname, R.RoleName, AL.School_name AS SchoolName, U.SchoolId, U.RoleId FROM Users U JOIN AllSchools AL ON U.SchoolId = AL.SchoolId JOIN Roles R ON R.RoleId = U.RoleId ORDER BY U.SchoolId, U.Username", cancellationToken));
			}
			else
			{
				if (!TryGetSchoolId(out var schoolId))
				{
					return Unauthorized(new { message = "The authenticated user does not have a valid school." });
				}
				return Ok(await database.QueryAsync("SELECT U.UserId, U.Username, U.Fullname, R.RoleName, AL.School_name AS SchoolName, U.SchoolId, U.RoleId FROM Users U JOIN AllSchools AL ON U.SchoolId = AL.SchoolId JOIN Roles R ON R.RoleId = U.RoleId WHERE U.SchoolId = @SchoolId ORDER BY U.Username", cancellationToken, new SqlParameter("@SchoolId", schoolId)));
			}
		}

	[HttpGet("me")]
	public async Task<IActionResult> Me(CancellationToken cancellationToken)
	{
		string text = base.User.Identity?.Name;
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT UserId, Username, Fullname, RoleId, SchoolId FROM Users WHERE Username = @Username", cancellationToken, new SqlParameter("@Username", text ?? string.Empty));
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

	[HttpPut("{userId:int}")]
	public async Task<IActionResult> Update(int userId, UserUpdateRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (userId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid user ID is required."
			});
		}
		if (string.IsNullOrWhiteSpace(request.Username))
		{
			return BadRequest(new
			{
				message = "Username is required."
			});
		}
		if (string.IsNullOrWhiteSpace(request.Fullname))
		{
			return BadRequest(new
			{
				message = "Full name is required."
			});
		}
		if (request.RoleId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid role ID is required."
			});
		}
		if (request.Password != null && string.IsNullOrWhiteSpace(request.Password))
		{
			return BadRequest(new
			{
				message = "Password cannot be empty when provided."
			});
		}
		string username = request.Username.Trim();
		string fullname = request.Fullname.Trim();
		string password = (string.IsNullOrWhiteSpace(request.Password) ? null : passwordCipher.EncryptForLegacyLogin(request.Password.Trim()));
		if ((await database.QueryAsync("SELECT UserId FROM Users WHERE Username = @Username AND UserId <> @UserId", cancellationToken, new SqlParameter("@Username", username), new SqlParameter("@UserId", userId))).Count > 0)
		{
			return Conflict(new
			{
				message = "Another user already has this username."
			});
		}
		try
		{
			IActionResult result;
			if (await database.ExecuteAsync("UPDATE Users SET Username = @Username, Fullname = @Fullname, RoleId = @RoleId, Password = COALESCE(@Password, Password) WHERE UserId = @UserId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@Username", username), new SqlParameter("@Fullname", fullname), new SqlParameter("@RoleId", request.RoleId), new SqlParameter("@Password", ((object)password) ?? ((object)DBNull.Value)), new SqlParameter("@UserId", userId), new SqlParameter("@SchoolId", schoolId)) != 0)
			{
				IActionResult actionResult = Ok(new
				{
					success = true,
					message = "User updated successfully.",
					userId = userId
				});
				result = actionResult;
			}
			else
			{
				IActionResult actionResult = NotFound(new
				{
					message = "User not found."
				});
				result = actionResult;
			}
			return result;
		}
		catch (SqlException ex) when (((Func<bool>)delegate
		{
			// Could not convert BlockContainer to single expression
			int number = ex.Number;
			return ((number == 2601 || number == 2627) ? 1 : 0) != 0;
		}).Invoke())
		{
			return Conflict(new
			{
				message = "Another user already has this username."
			});
		}
	}

	[HttpDelete("{userId:int}")]
	public async Task<IActionResult> Delete(int userId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (userId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid user ID is required."
			});
		}
		try
		{
			IActionResult result;
			if (await database.ExecuteAsync("DELETE FROM Users WHERE UserId = @UserId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@UserId", userId), new SqlParameter("@SchoolId", schoolId)) != 0)
			{
				IActionResult actionResult = Ok(new
				{
					success = true,
					message = "User deleted successfully.",
					userId = userId
				});
				result = actionResult;
			}
			else
			{
				IActionResult actionResult = NotFound(new
				{
					message = "User not found."
				});
				result = actionResult;
			}
			return result;
		}
		catch (SqlException ex) when (ex.Number == 547)
		{
			return Conflict(new
			{
				message = "This user cannot be deleted because it is referenced by other records."
			});
		}
	}

	[HttpPost("change-password")]
	public async Task<IActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
	{
		string value = base.User.Identity?.Name;
		if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
		{
			return BadRequest(new
			{
				message = "Current and new passwords are required."
			});
		}
		if (request.NewPassword.Trim() == request.CurrentPassword.Trim())
		{
			return BadRequest(new
			{
				message = "The new password must be different from the current password."
			});
		}
		string value2 = passwordCipher.EncryptForLegacyLogin(request.CurrentPassword.Trim());
		string value3 = passwordCipher.EncryptForLegacyLogin(request.NewPassword.Trim());
		IActionResult result;
		if (await database.ExecuteAsync("UPDATE Users SET Password = @NewPassword WHERE Username = @Username AND Password = @CurrentPassword", cancellationToken, new SqlParameter("@NewPassword", value3), new SqlParameter("@Username", value), new SqlParameter("@CurrentPassword", value2)) != 0)
		{
			IActionResult actionResult = Ok(new
			{
				message = "Password changed successfully."
			});
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = Unauthorized(new
			{
				message = "The current password is incorrect."
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

	[AllowAnonymous]
	[HttpPost("forgot-password")]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.Username))
		{
			return BadRequest(new
			{
				message = "Username is required."
			});
		}
		string value = request.Username.Trim();
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT UserId FROM Users WHERE Username = @Username", cancellationToken, new SqlParameter("@Username", value));
		if (readOnlyList.Count == 0)
		{
			return Ok(new
			{
				message = "If the account exists, a password reset token has been issued."
			});
		}
		string resetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
		string value2 = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(resetToken)));
		await database.ExecuteAsync("DELETE FROM PasswordResetTokens WHERE UserId = @UserId; INSERT INTO PasswordResetTokens (UserId, TokenHash, ExpiresAt, CreatedAt) VALUES (@UserId, @TokenHash, DATEADD(MINUTE, 15, SYSUTCDATETIME()), SYSUTCDATETIME())", cancellationToken, new SqlParameter("@UserId", readOnlyList[0]["UserId"]), new SqlParameter("@TokenHash", value2));
		return Ok(new
		{
			message = "If the account exists, a password reset token has been issued.",
			resetToken = resetToken
		});
	}

	[AllowAnonymous]
	[HttpPost("reset-password")]
	public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.ResetToken) || string.IsNullOrWhiteSpace(request.NewPassword))
		{
			return BadRequest(new
			{
				message = "Username, reset token, and new password are required."
			});
		}
		string value = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(request.ResetToken.Trim())));
		string value2 = passwordCipher.EncryptForLegacyLogin(request.NewPassword.Trim());
		IActionResult result;
		if (await database.ExecuteAsync("UPDATE U SET U.Password = @NewPassword FROM Users U JOIN PasswordResetTokens T ON T.UserId = U.UserId WHERE U.Username = @Username AND T.TokenHash = @TokenHash AND T.ExpiresAt > SYSUTCDATETIME(); DELETE T FROM PasswordResetTokens T JOIN Users U ON U.UserId = T.UserId WHERE U.Username = @Username AND T.TokenHash = @TokenHash", cancellationToken, new SqlParameter("@NewPassword", value2), new SqlParameter("@Username", request.Username.Trim()), new SqlParameter("@TokenHash", value)) != 0)
		{
			IActionResult actionResult = Ok(new
			{
				message = "Password reset successfully."
			});
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = BadRequest(new
			{
				message = "The reset token is invalid or expired."
			});
			result = actionResult;
		}
		return result;
	}
}
