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
[Route("api/classes")]
public sealed class ClassesController(SqlDatabase database) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		return Ok(await database.QueryAsync("SELECT ClassID, ClassName, Level, Section, SchoolId, CreatedBy FROM Classes WHERE SchoolId = @SchoolId ORDER BY ClassName", cancellationToken, new SqlParameter("@SchoolId", schoolId)));
	}

	[HttpPost]
	public async Task<IActionResult> Create(ClassRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (string.IsNullOrWhiteSpace(request.ClassName) || string.IsNullOrWhiteSpace(request.Level) || string.IsNullOrWhiteSpace(request.Section))
		{
			return BadRequest(new
			{
				message = "Class name, level, and section are required."
			});
		}
		try
		{
			if ((await database.QueryAsync("SELECT ClassID FROM Classes WHERE ClassName = @ClassName AND Level = @Level AND Section = @Section AND SchoolId = @SchoolId", cancellationToken, Parameters(request, schoolId))).Count > 0)
			{
				return Conflict(new
				{
					message = "This class has already been registered."
				});
			}
			SqlDatabase sqlDatabase = database;
			SqlParameter[] array = Parameters(request, schoolId);
			int num = 0;
			SqlParameter[] array2 = new SqlParameter[1 + array.Length];
			ReadOnlySpan<SqlParameter> readOnlySpan = new ReadOnlySpan<SqlParameter>(array);
			readOnlySpan.CopyTo(new Span<SqlParameter>(array2).Slice(num, readOnlySpan.Length));
			num += readOnlySpan.Length;
			array2[num] = new SqlParameter("@CreatedBy", base.User.Identity?.Name ?? string.Empty);
			int classId = Convert.ToInt32((await sqlDatabase.QueryAsync("INSERT INTO Classes (ClassName, Level, Section, SchoolId, CreatedBy) OUTPUT INSERTED.ClassID AS ClassId VALUES (@ClassName, @Level, @Section, @SchoolId, @CreatedBy)", cancellationToken, array2))[0]["ClassId"]);
			return StatusCode(201, new
			{
				success = true,
				message = "Class registered successfully.",
				classId = classId
			});
		}
		catch (SqlException ex) when (((Func<bool>)delegate
		{
			// Could not convert BlockContainer to single expression
			int num = ex.Number;
			return ((num == 2601 || num == 2627) ? 1 : 0) != 0;
		}).Invoke())
		{
			return Conflict(new
			{
				message = "This class has already been registered."
			});
		}
		catch (SqlException)
		{
			return Problem("The class could not be registered.", null, 500, "Class registration failed.");
		}
	}

	[HttpPut("{classId:int}")]
	public async Task<IActionResult> Update(int classId, ClassRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (string.IsNullOrWhiteSpace(request.ClassName) || string.IsNullOrWhiteSpace(request.Level) || string.IsNullOrWhiteSpace(request.Section))
		{
			return BadRequest(new
			{
				message = "Class name, level, and section are required."
			});
		}
		try
		{
			SqlDatabase sqlDatabase = database;
			SqlParameter[] array = Parameters(request, schoolId);
			int num = 0;
			SqlParameter[] array2 = new SqlParameter[1 + array.Length];
			ReadOnlySpan<SqlParameter> readOnlySpan = new ReadOnlySpan<SqlParameter>(array);
			readOnlySpan.CopyTo(new Span<SqlParameter>(array2).Slice(num, readOnlySpan.Length));
			num += readOnlySpan.Length;
			array2[num] = new SqlParameter("@ClassId", classId);
			if ((await sqlDatabase.QueryAsync("SELECT ClassID FROM Classes WHERE ClassName = @ClassName AND Level = @Level AND Section = @Section AND SchoolId = @SchoolId AND ClassID <> @ClassId", cancellationToken, array2)).Count > 0)
			{
				return Conflict(new
				{
					message = "Another class with these details already exists."
				});
			}
			SqlDatabase sqlDatabase2 = database;
			array2 = Parameters(request, schoolId);
			num = 0;
			array = new SqlParameter[1 + array2.Length];
			readOnlySpan = new ReadOnlySpan<SqlParameter>(array2);
			readOnlySpan.CopyTo(new Span<SqlParameter>(array).Slice(num, readOnlySpan.Length));
			num += readOnlySpan.Length;
			array[num] = new SqlParameter("@ClassId", classId);
			IActionResult result;
			if (await sqlDatabase2.ExecuteAsync("UPDATE Classes SET ClassName = @ClassName, Level = @Level, Section = @Section WHERE ClassID = @ClassId AND SchoolId = @SchoolId", cancellationToken, array) != 0)
			{
				IActionResult actionResult = Ok(new
				{
					success = true,
					message = "Class updated successfully.",
					classId = classId
				});
				result = actionResult;
			}
			else
			{
				IActionResult actionResult = NotFound(new
				{
					message = "Class not found."
				});
				result = actionResult;
			}
			return result;
		}
		catch (SqlException ex) when (((Func<bool>)delegate
		{
			// Could not convert BlockContainer to single expression
			int num = ex.Number;
			return ((num == 2601 || num == 2627) ? 1 : 0) != 0;
		}).Invoke())
		{
			return Conflict(new
			{
				message = "Another class with these details already exists."
			});
		}
		catch (SqlException)
		{
			return Problem("The class could not be updated.", null, 500, "Class update failed.");
		}
	}

	[HttpDelete("{classId:int}")]
	public async Task<IActionResult> Delete(int classId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		try
		{
			IActionResult result;
			if (await database.ExecuteAsync("DELETE FROM Classes WHERE ClassID = @ClassId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@ClassId", classId), new SqlParameter("@SchoolId", schoolId)) != 0)
			{
				IActionResult actionResult = Ok(new
				{
					success = true,
					message = "Class deleted successfully.",
					classId = classId
				});
				result = actionResult;
			}
			else
			{
				IActionResult actionResult = NotFound(new
				{
					message = "Class not found."
				});
				result = actionResult;
			}
			return result;
		}
		catch (SqlException ex) when (ex.Number == 547)
		{
			return Conflict(new
			{
				message = "This class cannot be deleted because it is used by other records."
			});
		}
		catch (SqlException)
		{
			return Problem("The class could not be deleted.", null, 500, "Class deletion failed.");
		}
	}

	private bool TryGetSchoolId(out int schoolId)
	{
		if (int.TryParse(base.User.FindFirst("schoolId")?.Value, out schoolId))
		{
			return schoolId > 0;
		}
		return false;
	}

	private static SqlParameter[] Parameters(ClassRequest request, int schoolId)
	{
		return new SqlParameter[4]
		{
			new SqlParameter("@ClassName", request.ClassName.Trim()),
			new SqlParameter("@Level", request.Level.Trim()),
			new SqlParameter("@Section", request.Section.Trim()),
			new SqlParameter("@SchoolId", schoolId)
		};
	}
}
