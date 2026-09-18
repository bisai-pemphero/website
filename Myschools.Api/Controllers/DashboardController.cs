using System;
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
[Route("api/dashboard")]
public sealed class DashboardController(SqlDatabase database) : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
	{
		int roleId = base.User.GetRoleId();
		int schoolId = base.User.GetSchoolId();
		if (roleId == 1)
		{
			IReadOnlyList<Dictionary<string, object?>> totalSchools = await database.QueryAsync("SELECT COUNT(*) AS Total FROM AllSchools", cancellationToken);
			IReadOnlyList<Dictionary<string, object?>> totalStudents = await database.QueryAsync("SELECT COUNT(*) AS Total FROM Students WHERE IsDeleted = 0 AND Status = 'Active'", cancellationToken);
			IReadOnlyList<Dictionary<string, object?>> totalUsers = await database.QueryAsync("SELECT COUNT(*) AS Total FROM Users", cancellationToken);
			IReadOnlyList<Dictionary<string, object>> rows = await database.QueryAsync("SELECT COUNT(*) AS Total FROM School_Licence WHERE Status = 'Active'", cancellationToken);
			return Ok(new
			{
				roleId = roleId,
				schoolId = schoolId,
				totalSchools = ToInt(totalSchools, "Total"),
				totalRegisteredStudents = ToInt(totalStudents, "Total"),
				totalSystemUsers = ToInt(totalUsers, "Total"),
				activeSchools = ToInt(rows, "Total")
			});
		}
		if (schoolId <= 0)
		{
			return Forbid();
		}
		string termActiveSql = "CAST(t.IsActive AS NVARCHAR(50)) IN ('True', 'true', '1')";
		IReadOnlyList<Dictionary<string, object?>> totalExpected = await database.QueryAsync("SELECT COALESCE(SUM(f.totalFees), 0) AS TotalExpected FROM Fees f INNER JOIN SchoolTerm t ON f.TermId = t.TermId WHERE f.SchoolId = @SchoolId AND " + termActiveSql, cancellationToken, new SqlParameter("@SchoolId", schoolId));
		IReadOnlyList<Dictionary<string, object?>> totalCollected = await database.QueryAsync("SELECT COALESCE(SUM(f.amountPaid), 0) AS TotalCollected FROM Fees f INNER JOIN SchoolTerm t ON f.TermId = t.TermId WHERE f.SchoolId = @SchoolId AND " + termActiveSql, cancellationToken, new SqlParameter("@SchoolId", schoolId));
		IReadOnlyList<Dictionary<string, object?>> totalDue = await database.QueryAsync("SELECT COALESCE(SUM(f.balance), 0) AS TotalDue FROM Fees f INNER JOIN SchoolTerm t ON f.TermId = t.TermId WHERE f.SchoolId = @SchoolId AND " + termActiveSql, cancellationToken, new SqlParameter("@SchoolId", schoolId));
		IReadOnlyList<Dictionary<string, object?>> collectionByClass = await database.QueryAsync("SELECT c.ClassName, COALESCE(SUM(f.amountPaid), 0) AS TotalCollected FROM Fees f INNER JOIN Students s ON f.studentId = s.StudentID INNER JOIN Classes c ON s.CurrentClassID = c.ClassID INNER JOIN SchoolTerm t ON f.TermId = t.TermId WHERE f.SchoolId = @SchoolId AND " + termActiveSql + " GROUP BY c.ClassName ORDER BY c.ClassName", cancellationToken, new SqlParameter("@SchoolId", schoolId));
		IReadOnlyList<Dictionary<string, object?>> collectionByCategory = await database.QueryAsync("SELECT fc.CategoryName, COALESCE(SUM(f.amountPaid), 0) AS TotalCollected FROM Fees f INNER JOIN FeesCategory fc ON f.fees_Category = fc.CategoryId INNER JOIN SchoolTerm t ON f.TermId = t.TermId WHERE f.SchoolId = @SchoolId AND " + termActiveSql + " GROUP BY fc.CategoryName ORDER BY fc.CategoryName", cancellationToken, new SqlParameter("@SchoolId", schoolId));
		IReadOnlyList<Dictionary<string, object?>> collectedToday = await database.QueryAsync("SELECT COALESCE(SUM(p.Paid), 0) AS TotalCollectedToday FROM Payments p INNER JOIN Fees f ON p.FeesId = f.FeesId WHERE CAST(p.datePaid AS DATE) = CAST(GETDATE() AS DATE) AND p.IsDeleted = 0 AND f.SchoolId = @SchoolId", cancellationToken, new SqlParameter("@SchoolId", schoolId));
		IReadOnlyList<Dictionary<string, object?>> collectedTodayByClass = await database.QueryAsync("SELECT c.ClassName, COALESCE(SUM(p.Paid), 0) AS TotalCollected FROM Payments p INNER JOIN Fees f ON p.FeesId = f.FeesId INNER JOIN Students s ON f.studentId = s.StudentID INNER JOIN Classes c ON s.CurrentClassID = c.ClassID WHERE CAST(p.datePaid AS DATE) = CAST(GETDATE() AS DATE) AND p.IsDeleted = 0 AND f.SchoolId = @SchoolId GROUP BY c.ClassName ORDER BY c.ClassName", cancellationToken, new SqlParameter("@SchoolId", schoolId));
		IReadOnlyList<Dictionary<string, object?>> collectedTodayByCategory = await database.QueryAsync("SELECT fc.CategoryName, COALESCE(SUM(p.Paid), 0) AS TotalCollected FROM Payments p INNER JOIN Fees f ON p.FeesId = f.FeesId INNER JOIN FeesCategory fc ON f.fees_Category = fc.CategoryId WHERE CAST(p.datePaid AS DATE) = CAST(GETDATE() AS DATE) AND p.IsDeleted = 0 AND f.SchoolId = @SchoolId GROUP BY fc.CategoryName ORDER BY fc.CategoryName", cancellationToken, new SqlParameter("@SchoolId", schoolId));
		IReadOnlyList<Dictionary<string, object?>> totalSchoolStudents = await database.QueryAsync("SELECT COUNT(*) AS Total FROM Students WHERE IsDeleted = 0 AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@SchoolId", schoolId));
		IReadOnlyList<Dictionary<string, object?>> genderBreakdown = await database.QueryAsync("SELECT Gender, COUNT(StudentID) AS Total FROM Students WHERE IsDeleted = 0 AND SchoolId = @SchoolId GROUP BY Gender ORDER BY Gender", cancellationToken, new SqlParameter("@SchoolId", schoolId));
		IReadOnlyList<Dictionary<string, object>> rows2 = await database.QueryAsync("SELECT c.ClassName, COUNT(s.StudentID) AS Total FROM Students s JOIN Classes c ON s.CurrentClassID = c.ClassID WHERE s.IsDeleted = 0 AND s.SchoolId = @SchoolId GROUP BY c.ClassName ORDER BY c.ClassName", cancellationToken, new SqlParameter("@SchoolId", schoolId));
		var value = new
		{
			roleId = roleId,
			schoolId = schoolId,
			totalExpectedForTerm = ToDecimal(totalExpected, "TotalExpected"),
			totalFeesCollected = ToDecimal(totalCollected, "TotalCollected"),
			totalDueForTerm = ToDecimal(totalDue, "TotalDue"),
			collectionByClass = ToCollectionRows(collectionByClass),
			collectionByCategory = ToCollectionRows(collectionByCategory),
			totalCollectedToday = ToDecimal(collectedToday, "TotalCollectedToday"),
			collectedTodayByClass = ToCollectionRows(collectedTodayByClass),
			collectedTodayByCategory = ToCollectionRows(collectedTodayByCategory),
			totalStudents = ToInt(totalSchoolStudents, "Total"),
			genderBreakdown = ToGroupRows(genderBreakdown),
			classBreakdown = ToGroupRows(rows2)
		};
		switch (roleId)
		{
		case 2:
			return Ok(value);
		case 3:
		case 6:
			return Ok(new
			{
				roleId = roleId,
				schoolId = schoolId,
				totalStudents = ToInt(totalSchoolStudents, "Total"),
				genderBreakdown = ToGroupRows(genderBreakdown),
				classBreakdown = ToGroupRows(rows2)
			});
		case 5:
			return Ok(new
			{
				roleId = roleId,
				schoolId = schoolId,
				totalCollectedToday = ToDecimal(collectedToday, "TotalCollectedToday"),
				collectedTodayByClass = ToCollectionRows(collectedTodayByClass),
				collectedTodayByCategory = ToCollectionRows(collectedTodayByCategory)
			});
		default:
			return Ok(new
			{
				roleId = roleId,
				schoolId = schoolId,
				totalStudents = ToInt(totalSchoolStudents, "Total"),
				genderBreakdown = ToGroupRows(genderBreakdown),
				classBreakdown = ToGroupRows(rows2)
			});
		}
	}

	private static int ToInt(IReadOnlyList<Dictionary<string, object?>> rows, string key)
	{
		if (rows.Count == 0)
		{
			return 0;
		}
		object obj = (rows[0].TryGetValue(key, out object value) ? value : ((object)0));
		if (obj != null)
		{
			return Convert.ToInt32(obj);
		}
		return 0;
	}

	private static decimal ToDecimal(IReadOnlyList<Dictionary<string, object?>> rows, string key)
	{
		if (rows.Count == 0)
		{
			return 0m;
		}
		object obj = (rows[0].TryGetValue(key, out object value) ? value : ((object)0m));
		if (obj != null)
		{
			return Convert.ToDecimal(obj);
		}
		return 0m;
	}

	private static object[] ToCollectionRows(IReadOnlyList<Dictionary<string, object?>> rows)
	{
		List<object> list = new List<object>();
		foreach (Dictionary<string, object> row in rows)
		{
			object name = (row.ContainsKey("ClassName") ? row["ClassName"] : (row.ContainsKey("CategoryName") ? row["CategoryName"] : "Unknown"));
			list.Add(new
			{
				name = name,
				totalCollected = (row.ContainsKey("TotalCollected") ? Convert.ToDecimal(row["TotalCollected"]) : 0m)
			});
		}
		return list.ToArray();
	}

	private static object[] ToGroupRows(IReadOnlyList<Dictionary<string, object?>> rows)
	{
		List<object> list = new List<object>();
		foreach (Dictionary<string, object> row in rows)
		{
			object name = (row.ContainsKey("Gender") ? row["Gender"] : (row.ContainsKey("ClassName") ? row["ClassName"] : "Unknown"));
			list.Add(new
			{
				name = name,
				total = (row.ContainsKey("Total") ? Convert.ToInt32(row["Total"]) : 0)
			});
		}
		return list.ToArray();
	}
}
