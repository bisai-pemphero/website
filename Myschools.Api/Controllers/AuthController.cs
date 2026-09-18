using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Myschools.Api.Data;
using Myschools.Api.Models;

namespace Myschools.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(SqlDatabase database, LegacyPasswordCipher passwordCipher, IConfiguration configuration) : ControllerBase
{
	private const int MaxFailedLoginAttempts = 5;

	private const int LockoutMinutes = 15;

	[HttpPost("login")]
	public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
	{
		string username = request.Username.Trim();
		string password = passwordCipher.EncryptForLegacyLogin(request.Password.Trim());
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT LockoutUntil FROM FailedLoginAttempts WHERE Username = @Username", cancellationToken, new SqlParameter("@Username", username));
		if (readOnlyList.Count > 0 && readOnlyList[0]["LockoutUntil"] is DateTime dateTime && dateTime > DateTime.UtcNow)
		{
			return Unauthorized(new
			{
				message = GetLockoutMessage(dateTime)
			});
		}
		IReadOnlyList<Dictionary<string, object?>> users = await database.QueryAsync("SELECT RoleId, SchoolId FROM Users WHERE Username = @Username AND Password = @Password", cancellationToken, new SqlParameter("@Username", username), new SqlParameter("@Password", password));
		if (users.Count == 0)
		{
			await database.ExecuteAsync("MERGE dbo.FailedLoginAttempts WITH (HOLDLOCK) AS target USING (SELECT @Username AS Username) AS source ON target.Username = source.Username WHEN MATCHED THEN UPDATE SET FailedAttempts = CASE WHEN target.LockoutUntil IS NOT NULL AND target.LockoutUntil <= SYSUTCDATETIME() THEN 1 ELSE target.FailedAttempts + 1 END, LockoutUntil = CASE WHEN (CASE WHEN target.LockoutUntil IS NOT NULL AND target.LockoutUntil <= SYSUTCDATETIME() THEN 1 ELSE target.FailedAttempts + 1 END) >= @MaxFailedAttempts THEN DATEADD(MINUTE, @LockoutMinutes, SYSUTCDATETIME()) ELSE NULL END, LastFailedAt = SYSUTCDATETIME() WHEN NOT MATCHED THEN INSERT (Username, FailedAttempts, LockoutUntil, LastFailedAt) VALUES (source.Username, 1, NULL, SYSUTCDATETIME());", cancellationToken, new SqlParameter("@Username", username), new SqlParameter("@MaxFailedAttempts", 5), new SqlParameter("@LockoutMinutes", 15));
			IReadOnlyList<Dictionary<string, object>> readOnlyList2 = await database.QueryAsync("SELECT LockoutUntil FROM FailedLoginAttempts WHERE Username = @Username", cancellationToken, new SqlParameter("@Username", username));
			if (readOnlyList2.Count > 0 && readOnlyList2[0]["LockoutUntil"] is DateTime dateTime2 && dateTime2 > DateTime.UtcNow)
			{
				return Unauthorized(new
				{
					message = GetLockoutMessage(dateTime2)
				});
			}
			return Unauthorized(new
			{
				message = "Incorrect login details."
			});
		}
		await database.ExecuteAsync("DELETE FROM FailedLoginAttempts WHERE Username = @Username", cancellationToken, new SqlParameter("@Username", username));
		Dictionary<string, object> dictionary = users[0];
		int roleId = Convert.ToInt32(dictionary["RoleId"]);
		int schoolId = Convert.ToInt32(dictionary["SchoolId"]);
		string machineName = Environment.MachineName;
		string value = base.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
		await database.ExecuteAsync("INSERT INTO T_Sys_Operate_Log (Operator_No, Operator_Name, Operate_Type, Module_Name, User_No, Meter_No, Log_Remark, IP_Address, Computer_Name, Station_No, Update_Flag, Update_Date, Create_Date, School_Id) VALUES (@OperatorNo, @OperatorName, 'Login', 'User Login', 'none', 'None', 'Login Successfull', @IpAddress, @ComputerName, 'NULL', '0', GETDATE(), GETDATE(), @SchoolId)", cancellationToken, new SqlParameter("@OperatorNo", username), new SqlParameter("@OperatorName", username), new SqlParameter("@IpAddress", value), new SqlParameter("@ComputerName", machineName), new SqlParameter("@SchoolId", schoolId));
		Claim[] claims = new Claim[4]
		{
			new Claim("sub", username),
			new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", username),
			new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", roleId.ToString()),
			new Claim("schoolId", schoolId.ToString())
		};
		SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
		SigningCredentials signingCredentials = new SigningCredentials(key, "HS256");
		DateTime? expires = DateTime.UtcNow.AddHours(8.0);
		SigningCredentials signingCredentials2 = signingCredentials;
		JwtSecurityToken token = new JwtSecurityToken(null, null, claims, null, expires, signingCredentials2);
		return Ok(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token), username, roleId, schoolId));
	}

	private static string GetLockoutMessage(DateTime lockoutUntil)
	{
		int num = Math.Max(1, (int)Math.Ceiling((lockoutUntil - DateTime.UtcNow).TotalMinutes));
		return $"Too many failed login attempts. Try again in {num} minute{((num == 1) ? string.Empty : "s")}.";
	}
}
