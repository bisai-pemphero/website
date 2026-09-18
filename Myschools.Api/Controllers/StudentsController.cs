using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Myschools.Api.Data;

namespace Myschools.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/students")]
public sealed class StudentsController(SqlDatabase database) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> Get([FromQuery] bool includeDeleted, CancellationToken cancellationToken)
	{
		int schoolId = base.User.GetSchoolId();
		string sql = (includeDeleted ? "SELECT S.StudentID, S.AdmissionNo, S.FirstName, S.Middlename, S.LastName, S.Gender, C.ClassName FROM Students S JOIN Classes C ON C.ClassID = S.CurrentClassID WHERE S.SchoolId = @SchoolId ORDER BY S.FirstName, S.LastName" : "SELECT S.StudentID, S.AdmissionNo, S.FirstName, S.Middlename, S.LastName, S.Gender, C.ClassName FROM Students S JOIN Classes C ON C.ClassID = S.CurrentClassID WHERE S.SchoolId = @SchoolId AND S.IsDeleted = 0 ORDER BY S.FirstName, S.LastName");
		return Ok(await database.QueryAsync(sql, cancellationToken, new SqlParameter("@SchoolId", schoolId)));
	}

	[HttpGet("{studentId:int}")]
	public async Task<IActionResult> GetById(int studentId, [FromQuery] bool includeDeleted, CancellationToken cancellationToken)
	{
		int schoolId = base.User.GetSchoolId();
		string sql = (includeDeleted ? "SELECT S.StudentID, S.AdmissionNo, S.FirstName, S.Middlename AS MiddleName, S.LastName, S.Gender, S.DateOfBirth, S.AdmissionDate, S.CurrentClassID, C.ClassName, S.ParentID, SP.Fullname AS ParentFullName, SP.PhoneNumber AS ParentPhone, SP.AlternatePhone AS ParentAlternatePhone, SP.Email AS ParentEmail, SP.Address AS ParentAddress, SP.Gender AS ParentGender, SP.Relationship AS ParentRelationship, SP.Occupation AS ParentOccupation, S.SpecialNeeds, S.Status, S.IsDeleted FROM Students S LEFT JOIN Classes C ON C.ClassID = S.CurrentClassID LEFT JOIN StudentParent SP ON SP.ParentID = S.ParentID WHERE S.StudentID = @StudentId AND S.SchoolId = @SchoolId" : "SELECT S.StudentID, S.AdmissionNo, S.FirstName, S.Middlename AS MiddleName, S.LastName, S.Gender, S.DateOfBirth, S.AdmissionDate, S.CurrentClassID, C.ClassName, S.ParentID, SP.Fullname AS ParentFullName, SP.PhoneNumber AS ParentPhone, SP.AlternatePhone AS ParentAlternatePhone, SP.Email AS ParentEmail, SP.Address AS ParentAddress, SP.Gender AS ParentGender, SP.Relationship AS ParentRelationship, SP.Occupation AS ParentOccupation, S.SpecialNeeds, S.Status, S.IsDeleted FROM Students S LEFT JOIN Classes C ON C.ClassID = S.CurrentClassID LEFT JOIN StudentParent SP ON SP.ParentID = S.ParentID WHERE S.StudentID = @StudentId AND S.SchoolId = @SchoolId AND S.IsDeleted = 0");
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync(sql, cancellationToken, new SqlParameter("@StudentId", studentId), new SqlParameter("@SchoolId", schoolId));
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
}
