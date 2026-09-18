using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
[Route("api/academic")]
public sealed class AcademicController(SqlDatabase database) : ControllerBase
{
	[HttpPost("years")]
	public async Task<IActionResult> CreateYear(AcademicYearRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		string text = ValidateYear(request, out var startYear, out var endYear);
		if (text != null)
		{
			return BadRequest(new
			{
				message = text
			});
		}
		if ((await database.QueryAsync("SELECT AcademicyearId FROM Academic_Year WHERE SchoolId = @SchoolId AND Start_year = @StartYear AND End_year = @EndYear", cancellationToken, YearParameters(startYear, endYear, schoolId))).Count > 0)
		{
			return Conflict(new
			{
				message = "This academic year already exists."
			});
		}
		return StatusCode(201, new
		{
			success = true,
			message = "Academic year created successfully.",
			academicYearId = (await database.QueryAsync("INSERT INTO Academic_Year (Start_year, End_year, SchoolId) OUTPUT INSERTED.AcademicyearId AS AcademicYearId VALUES (@StartYear, @EndYear, @SchoolId)", cancellationToken, YearParameters(startYear, endYear, schoolId)))[0]["AcademicYearId"]
		});
	}

	[HttpPut("years/{academicYearId:int}")]
	public async Task<IActionResult> UpdateYear(int academicYearId, AcademicYearRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (academicYearId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid academic year ID is required."
			});
		}
		string text = ValidateYear(request, out var startYear, out var endYear);
		if (text != null)
		{
			return BadRequest(new
			{
				message = text
			});
		}
		SqlDatabase sqlDatabase = database;
		SqlParameter[] array = YearParameters(startYear, endYear, schoolId);
		int num = 0;
		SqlParameter[] array2 = new SqlParameter[1 + array.Length];
		ReadOnlySpan<SqlParameter> readOnlySpan = new ReadOnlySpan<SqlParameter>(array);
		readOnlySpan.CopyTo(new Span<SqlParameter>(array2).Slice(num, readOnlySpan.Length));
		num += readOnlySpan.Length;
		array2[num] = new SqlParameter("@AcademicYearId", academicYearId);
		if ((await sqlDatabase.QueryAsync("SELECT AcademicyearId FROM Academic_Year WHERE SchoolId = @SchoolId AND Start_year = @StartYear AND End_year = @EndYear AND AcademicyearId <> @AcademicYearId", cancellationToken, array2)).Count > 0)
		{
			return Conflict(new
			{
				message = "This academic year already exists."
			});
		}
		SqlDatabase sqlDatabase2 = database;
		array2 = YearParameters(startYear, endYear, schoolId);
		num = 0;
		array = new SqlParameter[1 + array2.Length];
		readOnlySpan = new ReadOnlySpan<SqlParameter>(array2);
		readOnlySpan.CopyTo(new Span<SqlParameter>(array).Slice(num, readOnlySpan.Length));
		num += readOnlySpan.Length;
		array[num] = new SqlParameter("@AcademicYearId", academicYearId);
		IActionResult result;
		if (await sqlDatabase2.ExecuteAsync("UPDATE Academic_Year SET Start_year = @StartYear, End_year = @EndYear WHERE AcademicyearId = @AcademicYearId AND SchoolId = @SchoolId", cancellationToken, array) != 0)
		{
			IActionResult actionResult = Ok(new
			{
				success = true,
				message = "Academic year updated successfully.",
				academicYearId = academicYearId
			});
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound(new
			{
				message = "Academic year not found."
			});
			result = actionResult;
		}
		return result;
	}

	[HttpDelete("years/{academicYearId:int}")]
	public async Task<IActionResult> DeleteYear(int academicYearId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (academicYearId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid academic year ID is required."
			});
		}
		try
		{
			IActionResult result;
			if (await database.ExecuteAsync("DELETE FROM Academic_Year WHERE AcademicyearId = @AcademicYearId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@AcademicYearId", academicYearId), new SqlParameter("@SchoolId", schoolId)) != 0)
			{
				IActionResult actionResult = Ok(new
				{
					success = true,
					message = "Academic year deleted successfully.",
					academicYearId = academicYearId
				});
				result = actionResult;
			}
			else
			{
				IActionResult actionResult = NotFound(new
				{
					message = "Academic year not found."
				});
				result = actionResult;
			}
			return result;
		}
		catch (SqlException ex) when (ex.Number == 547)
		{
			return Conflict(new
			{
				message = "This academic year cannot be deleted because it is used by terms or other records."
			});
		}
	}

	[HttpGet("years")]
	public async Task<IActionResult> Years(CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		return Ok(await database.QueryAsync("SELECT AcademicyearId, CONCAT(DATENAME(YEAR, Start_year), '-', YEAR(End_year)) AS AcademicYear, Start_year, End_year FROM Academic_Year WHERE SchoolId = @SchoolId ORDER BY Start_year DESC", cancellationToken, new SqlParameter("@SchoolId", schoolId)));
	}

	[HttpGet("terms")]
	public async Task<IActionResult> Terms([FromQuery] int academicYearId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (academicYearId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid academic year ID is required."
			});
		}
		return Ok(await database.QueryAsync("SELECT TermId, TermName, CONCAT(DATENAME(MONTH, Startdate), ' ', YEAR(Enddate)) AS Period FROM SchoolTerm WHERE SchoolId = @SchoolId AND AcademicYearId = @AcademicYearId", cancellationToken, new SqlParameter("@SchoolId", schoolId), new SqlParameter("@AcademicYearId", academicYearId)));
	}

	[HttpPost("terms")]
	public async Task<IActionResult> CreateTerm(AcademicTermRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		string text = ValidateTerm(request, out var startDate, out var endDate);
		if (text != null)
		{
			return BadRequest(new
			{
				message = text
			});
		}
		if (!(await AcademicYearBelongsToSchool(request.AcademicYearId, schoolId, cancellationToken)))
		{
			return NotFound(new
			{
				message = "Academic year not found."
			});
		}
		if ((await database.QueryAsync("SELECT TermId FROM SchoolTerm WHERE SchoolId = @SchoolId AND AcademicYearId = @AcademicYearId AND TermName = @TermName", cancellationToken, TermParameters(request, startDate, endDate, schoolId))).Count > 0)
		{
			return Conflict(new
			{
				message = "This academic term already exists."
			});
		}
		return StatusCode(201, new
		{
			success = true,
			message = "Academic term created successfully.",
			termId = (await database.QueryAsync("INSERT INTO SchoolTerm (TermName, Startdate, Enddate, AcademicYearId, SchoolId, IsActive) OUTPUT INSERTED.TermId AS TermId VALUES (@TermName, @StartDate, @EndDate, @AcademicYearId, @SchoolId, @IsActive)", cancellationToken, TermParameters(request, startDate, endDate, schoolId)))[0]["TermId"]
		});
	}

	[HttpPut("terms/{termId:int}")]
	public async Task<IActionResult> UpdateTerm(int termId, AcademicTermRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (termId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid term ID is required."
			});
		}
		string text = ValidateTerm(request, out var startDate, out var endDate);
		if (text != null)
		{
			return BadRequest(new
			{
				message = text
			});
		}
		if (!(await AcademicYearBelongsToSchool(request.AcademicYearId, schoolId, cancellationToken)))
		{
			return NotFound(new
			{
				message = "Academic year not found."
			});
		}
		SqlDatabase sqlDatabase = database;
		SqlParameter[] array = TermParameters(request, startDate, endDate, schoolId);
		int num = 0;
		SqlParameter[] array2 = new SqlParameter[1 + array.Length];
		ReadOnlySpan<SqlParameter> readOnlySpan = new ReadOnlySpan<SqlParameter>(array);
		readOnlySpan.CopyTo(new Span<SqlParameter>(array2).Slice(num, readOnlySpan.Length));
		num += readOnlySpan.Length;
		array2[num] = new SqlParameter("@TermId", termId);
		if ((await sqlDatabase.QueryAsync("SELECT TermId FROM SchoolTerm WHERE SchoolId = @SchoolId AND AcademicYearId = @AcademicYearId AND TermName = @TermName AND TermId <> @TermId", cancellationToken, array2)).Count > 0)
		{
			return Conflict(new
			{
				message = "This academic term already exists."
			});
		}
		SqlDatabase sqlDatabase2 = database;
		array2 = TermParameters(request, startDate, endDate, schoolId);
		num = 0;
		array = new SqlParameter[1 + array2.Length];
		readOnlySpan = new ReadOnlySpan<SqlParameter>(array2);
		readOnlySpan.CopyTo(new Span<SqlParameter>(array).Slice(num, readOnlySpan.Length));
		num += readOnlySpan.Length;
		array[num] = new SqlParameter("@TermId", termId);
		IActionResult result;
		if (await sqlDatabase2.ExecuteAsync("UPDATE SchoolTerm SET TermName = @TermName, Startdate = @StartDate, Enddate = @EndDate, AcademicYearId = @AcademicYearId, IsActive = @IsActive WHERE TermId = @TermId AND SchoolId = @SchoolId", cancellationToken, array) != 0)
		{
			IActionResult actionResult = Ok(new
			{
				success = true,
				message = "Academic term updated successfully.",
				termId = termId
			});
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound(new
			{
				message = "Academic term not found."
			});
			result = actionResult;
		}
		return result;
	}

	[HttpDelete("terms/{termId:int}")]
	public async Task<IActionResult> DeleteTerm(int termId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (termId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid term ID is required."
			});
		}
		try
		{
			IActionResult result;
			if (await database.ExecuteAsync("DELETE FROM SchoolTerm WHERE TermId = @TermId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@TermId", termId), new SqlParameter("@SchoolId", schoolId)) != 0)
			{
				IActionResult actionResult = Ok(new
				{
					success = true,
					message = "Academic term deleted successfully.",
					termId = termId
				});
				result = actionResult;
			}
			else
			{
				IActionResult actionResult = NotFound(new
				{
					message = "Academic term not found."
				});
				result = actionResult;
			}
			return result;
		}
		catch (SqlException ex) when (ex.Number == 547)
		{
			return Conflict(new
			{
				message = "This academic term cannot be deleted because it is used by other records."
			});
		}
	}

	[HttpGet("subjects")]
	public async Task<IActionResult> Subjects([FromQuery] int? classId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (classId.HasValue && classId.GetValueOrDefault() <= 0)
		{
			return BadRequest(new
			{
				message = "The class ID must be greater than zero."
			});
		}
		string text = (classId.HasValue ? " AND S.ClassId = @ClassId" : string.Empty);
		SqlParameter[] parameters = ((!classId.HasValue) ? new SqlParameter[1]
		{
			new SqlParameter("@SchoolId", schoolId)
		} : new SqlParameter[2]
		{
			new SqlParameter("@SchoolId", schoolId),
			new SqlParameter("@ClassId", classId.Value)
		});
		return Ok(await database.QueryAsync("SELECT S.SubjectId, S.SubjectName, S.ClassId, C.ClassName FROM Subjects S JOIN Classes C ON C.ClassID = S.ClassId WHERE C.SchoolId = @SchoolId" + text + " ORDER BY C.ClassName, S.SubjectName", cancellationToken, parameters));
	}

	[HttpPost("subjects")]
	public async Task<IActionResult> CreateSubject(SubjectRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (string.IsNullOrWhiteSpace(request.SubjectName) || request.ClassId <= 0)
		{
			return BadRequest(new
			{
				message = "Subject name and class are required."
			});
		}
		if (!(await ClassBelongsToSchool(request.ClassId, schoolId, cancellationToken)))
		{
			return NotFound(new
			{
				message = "Class not found."
			});
		}
		if ((await database.QueryAsync("SELECT SubjectId FROM Subjects WHERE SubjectName = @SubjectName AND ClassId = @ClassId", cancellationToken, new SqlParameter("@SubjectName", request.SubjectName.Trim()), new SqlParameter("@ClassId", request.ClassId))).Count > 0)
		{
			return Conflict(new
			{
				message = "Subject already added please Update."
			});
		}
		await database.ExecuteAsync("INSERT INTO Subjects (SubjectName, ClassId, CreatedBy, DateCreated) VALUES (@SubjectName, @ClassId, @CreatedBy, GETDATE())", cancellationToken, new SqlParameter("@SubjectName", request.SubjectName.Trim()), new SqlParameter("@ClassId", request.ClassId), new SqlParameter("@CreatedBy", base.User.Identity?.Name ?? string.Empty));
		return Ok(new
		{
			message = "Subject added Successfully."
		});
	}

	[HttpPut("subjects/{subjectId:int}")]
	public async Task<IActionResult> UpdateSubject(int subjectId, SubjectRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (subjectId <= 0 || string.IsNullOrWhiteSpace(request.SubjectName) || request.ClassId <= 0)
		{
			return BadRequest(new
			{
				message = "Subject name and class are required."
			});
		}
		if (!(await ClassBelongsToSchool(request.ClassId, schoolId, cancellationToken)))
		{
			return NotFound(new
			{
				message = "Class not found."
			});
		}
		if ((await database.QueryAsync("SELECT S.SubjectId FROM Subjects S JOIN Classes C ON C.ClassID = S.ClassId WHERE S.SubjectName = @SubjectName AND S.ClassId = @ClassId AND S.SubjectId <> @SubjectId AND C.SchoolId = @SchoolId", cancellationToken, new SqlParameter("@SubjectName", request.SubjectName.Trim()), new SqlParameter("@ClassId", request.ClassId), new SqlParameter("@SubjectId", subjectId), new SqlParameter("@SchoolId", schoolId))).Count > 0)
		{
			return Conflict(new
			{
				message = "Another subject with these details already exists."
			});
		}
		IActionResult result;
		if (await database.ExecuteAsync("UPDATE S SET SubjectName = @SubjectName, ClassId = @ClassId FROM Subjects S JOIN Classes C ON C.ClassID = S.ClassId WHERE S.SubjectId = @SubjectId AND C.SchoolId = @SchoolId", cancellationToken, new SqlParameter("@SubjectName", request.SubjectName.Trim()), new SqlParameter("@ClassId", request.ClassId), new SqlParameter("@SubjectId", subjectId), new SqlParameter("@SchoolId", schoolId)) != 0)
		{
			IActionResult actionResult = Ok(new
			{
				success = true,
				message = "Subject updated successfully.",
				subjectId = subjectId
			});
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound(new
			{
				message = "Subject not found."
			});
			result = actionResult;
		}
		return result;
	}

	[HttpDelete("subjects/{subjectId:int}")]
	public async Task<IActionResult> DeleteSubject(int subjectId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (subjectId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid subject ID is required."
			});
		}
		try
		{
			IActionResult result;
			if (await database.ExecuteAsync("DELETE S FROM Subjects S JOIN Classes C ON C.ClassID = S.ClassId WHERE S.SubjectId = @SubjectId AND C.SchoolId = @SchoolId", cancellationToken, new SqlParameter("@SubjectId", subjectId), new SqlParameter("@SchoolId", schoolId)) != 0)
			{
				IActionResult actionResult = Ok(new
				{
					success = true,
					message = "Subject deleted successfully.",
					subjectId = subjectId
				});
				result = actionResult;
			}
			else
			{
				IActionResult actionResult = NotFound(new
				{
					message = "Subject not found."
				});
				result = actionResult;
			}
			return result;
		}
		catch (SqlException ex) when (ex.Number == 547)
		{
			return Conflict(new
			{
				message = "This subject cannot be deleted because it is used by other records."
			});
		}
	}

	[HttpGet("exams")]
	public async Task<IActionResult> Exams(CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		return Ok(await database.QueryAsync("SELECT ExamsId, Exam_name, Exam_start, Exam_End, AcademicYear, TermId, SchoolId FROM Exams WHERE SchoolId = @SchoolId ORDER BY Exam_start DESC", cancellationToken, new SqlParameter("@SchoolId", schoolId)));
	}

	[HttpGet("exams/{examId:int}")]
	public async Task<IActionResult> Exam(int examId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (examId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid exam ID is required."
			});
		}
		IReadOnlyList<Dictionary<string, object>> readOnlyList = await database.QueryAsync("SELECT ExamsId, Exam_name, Exam_start, Exam_End, AcademicYear, TermId, SchoolId FROM Exams WHERE ExamsId = @ExamId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@ExamId", examId), new SqlParameter("@SchoolId", schoolId));
		IActionResult result;
		if (readOnlyList.Count != 0)
		{
			IActionResult actionResult = Ok(readOnlyList[0]);
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound(new
			{
				message = "Exam not found."
			});
			result = actionResult;
		}
		return result;
	}

	[HttpPost("exams")]
	public async Task<IActionResult> CreateExam(ExamRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		string text = ValidateExam(request, out var examStart, out var examEnd);
		if (text != null)
		{
			return BadRequest(new
			{
				message = text
			});
		}
		SqlParameter[] parameters = new SqlParameter[6]
		{
			new SqlParameter("@Name", request.ExamName.Trim()),
			new SqlParameter("@Start", examStart),
			new SqlParameter("@End", examEnd),
			new SqlParameter("@Year", request.AcademicYearId),
			new SqlParameter("@Term", request.TermId),
			new SqlParameter("@SchoolId", schoolId)
		};
		if ((await database.QueryAsync("SELECT ExamsId FROM Exams WHERE Exam_name = @Name AND Exam_start = @Start AND Exam_End = @End AND AcademicYear = @Year AND TermId = @Term AND SchoolId = @SchoolId", cancellationToken, parameters)).Count > 0)
		{
			return Conflict(new
			{
				message = "Exam already set please Update."
			});
		}
		await database.ExecuteAsync("INSERT INTO Exams (Exam_name, Exam_start, Exam_End, AcademicYear, TermId, SchoolId, Createdby, DateCreated) VALUES (@Name, @Start, @End, @Year, @Term, @SchoolId, @CreatedBy, GETDATE())", cancellationToken, new SqlParameter("@Name", request.ExamName.Trim()), new SqlParameter("@Start", examStart), new SqlParameter("@End", examEnd), new SqlParameter("@Year", request.AcademicYearId), new SqlParameter("@Term", request.TermId), new SqlParameter("@SchoolId", schoolId), new SqlParameter("@CreatedBy", base.User.Identity?.Name ?? string.Empty));
		return Ok(new
		{
			message = "Exam has been set Successfully."
		});
	}

	[HttpPut("exams/{examId:int}")]
	public async Task<IActionResult> UpdateExam(int examId, ExamRequest request, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (examId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid exam ID is required."
			});
		}
		string text = ValidateExam(request, out var examStart, out var examEnd);
		if (text != null)
		{
			return BadRequest(new
			{
				message = text
			});
		}
		SqlParameter[] parameters = new SqlParameter[7]
		{
			new SqlParameter("@Name", request.ExamName.Trim()),
			new SqlParameter("@Start", examStart),
			new SqlParameter("@End", examEnd),
			new SqlParameter("@Year", request.AcademicYearId),
			new SqlParameter("@Term", request.TermId),
			new SqlParameter("@SchoolId", schoolId),
			new SqlParameter("@ExamId", examId)
		};
		if ((await database.QueryAsync("SELECT ExamsId FROM Exams WHERE Exam_name = @Name AND Exam_start = @Start AND Exam_End = @End AND AcademicYear = @Year AND TermId = @Term AND SchoolId = @SchoolId AND ExamsId <> @ExamId", cancellationToken, parameters)).Count > 0)
		{
			return Conflict(new
			{
				message = "Another exam with these details already exists."
			});
		}
		IActionResult result;
		if (await database.ExecuteAsync("UPDATE Exams SET Exam_name = @Name, Exam_start = @Start, Exam_End = @End, AcademicYear = @Year, TermId = @Term WHERE ExamsId = @ExamId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@Name", request.ExamName.Trim()), new SqlParameter("@Start", examStart), new SqlParameter("@End", examEnd), new SqlParameter("@Year", request.AcademicYearId), new SqlParameter("@Term", request.TermId), new SqlParameter("@SchoolId", schoolId), new SqlParameter("@ExamId", examId)) != 0)
		{
			IActionResult actionResult = Ok(new
			{
				success = true,
				message = "Exam updated successfully.",
				examId = examId
			});
			result = actionResult;
		}
		else
		{
			IActionResult actionResult = NotFound(new
			{
				message = "Exam not found."
			});
			result = actionResult;
		}
		return result;
	}

	[HttpDelete("exams/{examId:int}")]
	public async Task<IActionResult> DeleteExam(int examId, CancellationToken cancellationToken)
	{
		if (!TryGetSchoolId(out var schoolId))
		{
			return Unauthorized(new
			{
				message = "The authenticated user does not have a valid school."
			});
		}
		if (examId <= 0)
		{
			return BadRequest(new
			{
				message = "A valid exam ID is required."
			});
		}
		try
		{
			IActionResult result;
			if (await database.ExecuteAsync("DELETE FROM Exams WHERE ExamsId = @ExamId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@ExamId", examId), new SqlParameter("@SchoolId", schoolId)) != 0)
			{
				IActionResult actionResult = Ok(new
				{
					success = true,
					message = "Exam deleted successfully.",
					examId = examId
				});
				result = actionResult;
			}
			else
			{
				IActionResult actionResult = NotFound(new
				{
					message = "Exam not found."
				});
				result = actionResult;
			}
			return result;
		}
		catch (SqlException ex) when (ex.Number == 547)
		{
			return Conflict(new
			{
				message = "This exam cannot be deleted because it is used by other records."
			});
		}
	}

	private async Task<bool> AcademicYearBelongsToSchool(int academicYearId, int schoolId, CancellationToken cancellationToken)
	{
		return (await database.QueryAsync("SELECT AcademicyearId FROM Academic_Year WHERE AcademicyearId = @AcademicYearId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@AcademicYearId", academicYearId), new SqlParameter("@SchoolId", schoolId))).Count > 0;
	}

	private async Task<bool> ClassBelongsToSchool(int classId, int schoolId, CancellationToken cancellationToken)
	{
		return (await database.QueryAsync("SELECT ClassID FROM Classes WHERE ClassID = @ClassId AND SchoolId = @SchoolId", cancellationToken, new SqlParameter("@ClassId", classId), new SqlParameter("@SchoolId", schoolId))).Count > 0;
	}

	private bool TryGetSchoolId(out int schoolId)
	{
		if (int.TryParse(base.User.FindFirst("schoolId")?.Value, out schoolId))
		{
			return schoolId > 0;
		}
		return false;
	}

	private static string? ValidateYear(AcademicYearRequest request, out DateTime startYear, out DateTime endYear)
	{
		startYear = default(DateTime);
		endYear = default(DateTime);
		if (string.IsNullOrWhiteSpace(request.StartYear) || !DateTime.TryParseExact(request.StartYear.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startYear))
		{
			return "Academic year start date is required and must use the format yyyy-MM-dd.";
		}
		if (string.IsNullOrWhiteSpace(request.EndYear) || !DateTime.TryParseExact(request.EndYear.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endYear))
		{
			return "Academic year end date is required and must use the format yyyy-MM-dd.";
		}
		if (startYear >= endYear)
		{
			return "Academic year start date must be before the end date.";
		}
		return null;
	}

	private static string? ValidateTerm(AcademicTermRequest request, out DateTime startDate, out DateTime endDate)
	{
		startDate = default(DateTime);
		endDate = default(DateTime);
		if (string.IsNullOrWhiteSpace(request.TermName))
		{
			return "Term name is required.";
		}
		if (request.AcademicYearId <= 0)
		{
			return "A valid academic year ID is required.";
		}
		string isActive = request.IsActive;
		if ((!(isActive == "True") && !(isActive == "False")) || 1 == 0)
		{
			return "IsActive must be exactly True or False.";
		}
		if (string.IsNullOrWhiteSpace(request.StartDate) || !DateTime.TryParseExact(request.StartDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
		{
			return "Term start date is required and must use the format yyyy-MM-dd.";
		}
		if (string.IsNullOrWhiteSpace(request.EndDate) || !DateTime.TryParseExact(request.EndDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
		{
			return "Term end date is required and must use the format yyyy-MM-dd.";
		}
		if (startDate >= endDate)
		{
			return "Term start date must be before the end date.";
		}
		return null;
	}

	private static string? ValidateExam(ExamRequest request, out DateTime examStart, out DateTime examEnd)
	{
		examStart = default(DateTime);
		examEnd = default(DateTime);
		if (string.IsNullOrWhiteSpace(request.ExamName))
		{
			return "Exam name is required.";
		}
		if (string.IsNullOrWhiteSpace(request.ExamStart) || !DateTime.TryParse(request.ExamStart.Trim(), out examStart))
		{
			return "A valid exam start date is required.";
		}
		if (string.IsNullOrWhiteSpace(request.ExamEnd) || !DateTime.TryParse(request.ExamEnd.Trim(), out examEnd))
		{
			return "A valid exam end date is required.";
		}
		if (examStart >= examEnd)
		{
			return "Exam start date must be before the end date.";
		}
		if (request.AcademicYearId <= 0)
		{
			return "A valid academic year ID is required.";
		}
		if (request.TermId <= 0)
		{
			return "A valid term ID is required.";
		}
		return null;
	}

	private static SqlParameter[] YearParameters(DateTime startYear, DateTime endYear, int schoolId)
	{
		return new SqlParameter[3]
		{
			DateParameter("@StartYear", startYear),
			DateParameter("@EndYear", endYear),
			new SqlParameter("@SchoolId", schoolId)
		};
	}

	private static SqlParameter[] TermParameters(AcademicTermRequest request, DateTime startDate, DateTime endDate, int schoolId)
	{
		return new SqlParameter[6]
		{
			new SqlParameter("@TermName", request.TermName.Trim()),
			DateParameter("@StartDate", startDate),
			DateParameter("@EndDate", endDate),
			new SqlParameter("@AcademicYearId", request.AcademicYearId),
			new SqlParameter("@SchoolId", schoolId),
			new SqlParameter("@IsActive", request.IsActive)
		};
	}

	private static SqlParameter DateParameter(string name, DateTime value)
	{
		return new SqlParameter(name, SqlDbType.Date)
		{
			Value = value.Date
		};
	}
}
