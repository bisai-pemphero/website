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
[Route("api/teacher-assignments")]
public sealed class TeacherAssignmentsController(SqlDatabase database) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		int num = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		return Ok(await database.QueryAsync("SELECT FormTeacherId, TeacherId, TeacherName, ClassId, SchoolId FROM FormTeachers WHERE SchoolId = @SchoolId", cancellationToken, new SqlParameter("@SchoolId", num)));
	}

	[HttpPost]
	public async Task<IActionResult> Assign(TeacherAssignmentRequest request, CancellationToken cancellationToken)
	{
		int schoolId = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		if ((await database.QueryAsync("SELECT FormTeacherId FROM FormTeachers WHERE ClassId = @ClassId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@ClassId", request.ClassId), new SqlParameter("@SchoolId", schoolId))).Count > 0)
		{
			return Conflict(new
			{
				message = "Class Teacher already assigned to this class, please update."
			});
		}
		await database.ExecuteAsync("INSERT INTO FormTeachers (TeacherId, TeacherName, ClassId, SchoolId) VALUES (@TeacherId, @TeacherName, @ClassId, @SchoolId)", cancellationToken, new SqlParameter("@TeacherId", request.TeacherId), new SqlParameter("@TeacherName", request.TeacherName.Trim()), new SqlParameter("@ClassId", request.ClassId), new SqlParameter("@SchoolId", schoolId));
		return Ok(new
		{
			message = "Class Teacher Assigned Successfully."
		});
	}

	[HttpPut("{assignmentId:int}")]
	public async Task<IActionResult> Update(int assignmentId, TeacherAssignmentRequest request, CancellationToken cancellationToken)
	{
		int num = int.Parse(base.User.FindFirst("schoolId")?.Value ?? "0");
		IActionResult result;
		if (await database.ExecuteAsync("UPDATE FormTeachers SET TeacherId = @TeacherId, TeacherName = @TeacherName, ClassId = @ClassId WHERE FormTeacherId = @AssignmentId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@TeacherId", request.TeacherId), new SqlParameter("@TeacherName", request.TeacherName.Trim()), new SqlParameter("@ClassId", request.ClassId), new SqlParameter("@AssignmentId", assignmentId), new SqlParameter("@SchoolId", num)) != 0)
		{
			IActionResult actionResult = Ok();
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound();
			result = actionResult;
		}
		return result;
	}
}
