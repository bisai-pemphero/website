using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Myschools.Api.Data;

public sealed class SqlDatabase(IConfiguration configuration)
{
	private readonly string _connectionString = configuration.GetConnectionString("Myschools") ?? throw new InvalidOperationException("ConnectionStrings:Myschools is required.");

	public SqlConnection CreateConnection()
	{
		return new SqlConnection(_connectionString);
	}

	public async Task EnsurePasswordResetTokensTableAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		await ExecuteAsync("IF OBJECT_ID(N'dbo.PasswordResetTokens', N'U') IS NULL\r\nBEGIN\r\n    CREATE TABLE dbo.PasswordResetTokens (\r\n        PasswordResetTokenId int IDENTITY(1,1) NOT NULL PRIMARY KEY,\r\n        UserId int NOT NULL,\r\n        TokenHash varchar(64) NOT NULL,\r\n        ExpiresAt datetime2 NOT NULL,\r\n        CreatedAt datetime2 NOT NULL,\r\n        CONSTRAINT FK_PasswordResetTokens_Users FOREIGN KEY (UserId) REFERENCES dbo.Users(UserId)\r\n    );\r\nEND;\r\n\r\nIF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_PasswordResetTokens_UserId' AND object_id = OBJECT_ID(N'dbo.PasswordResetTokens'))\r\n    CREATE UNIQUE INDEX UX_PasswordResetTokens_UserId ON dbo.PasswordResetTokens(UserId);\r\n\r\nIF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_PasswordResetTokens_TokenHash' AND object_id = OBJECT_ID(N'dbo.PasswordResetTokens'))\r\n    CREATE INDEX IX_PasswordResetTokens_TokenHash ON dbo.PasswordResetTokens(TokenHash);", cancellationToken);
	}

	public async Task EnsureFailedLoginAttemptsTableAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		await ExecuteAsync("IF OBJECT_ID(N'dbo.FailedLoginAttempts', N'U') IS NULL\r\nBEGIN\r\n    CREATE TABLE dbo.FailedLoginAttempts (\r\n        Username nvarchar(256) NOT NULL PRIMARY KEY,\r\n        FailedAttempts int NOT NULL,\r\n        LockoutUntil datetime2 NULL,\r\n        LastFailedAt datetime2 NOT NULL\r\n    );\r\nEND;", cancellationToken);
	}

	public async Task<IReadOnlyList<Dictionary<string, object?>>> QueryAsync(string sql, CancellationToken cancellationToken, params SqlParameter[] parameters)
	{
		IReadOnlyList<Dictionary<string, object?>> result;
		await using (SqlConnection connection = CreateConnection())
		{
			await connection.OpenAsync(cancellationToken);
			IReadOnlyList<Dictionary<string, object?>> readOnlyList2;
			await using (SqlCommand command = new SqlCommand(sql, connection))
			{
				command.Parameters.AddRange(parameters);
				IReadOnlyList<Dictionary<string, object?>> readOnlyList;
				await using (SqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken))
				{
					List<Dictionary<string, object?>> rows = new List<Dictionary<string, object>>();
					while (await reader.ReadAsync(cancellationToken))
					{
						Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
						for (int i = 0; i < reader.FieldCount; i++)
						{
							dictionary[reader.GetName(i)] = (reader.IsDBNull(i) ? null : reader.GetValue(i));
						}
						rows.Add(dictionary);
					}
					readOnlyList = rows;
				}
				readOnlyList2 = readOnlyList;
			}
			result = readOnlyList2;
		}
		return result;
	}

	public async Task<int> ExecuteAsync(string sql, CancellationToken cancellationToken, params SqlParameter[] parameters)
	{
		int result;
		await using (SqlConnection connection = CreateConnection())
		{
			await connection.OpenAsync(cancellationToken);
			int num;
			await using (SqlCommand command = new SqlCommand(sql, connection))
			{
				command.Parameters.AddRange(parameters);
				num = await command.ExecuteNonQueryAsync(cancellationToken);
			}
			result = num;
		}
		return result;
	}

	public static async Task<int> ExecuteAsync(SqlConnection connection, SqlTransaction transaction, string sql, CancellationToken cancellationToken, params SqlParameter[] parameters)
	{
		int result;
		await using (SqlCommand command = new SqlCommand(sql, connection, transaction))
		{
			command.Parameters.AddRange(parameters);
			result = await command.ExecuteNonQueryAsync(cancellationToken);
		}
		return result;
	}
}
