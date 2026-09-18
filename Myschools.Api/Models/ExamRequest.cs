namespace Myschools.Api.Models;

public sealed record ExamRequest(string ExamName, string ExamStart, string ExamEnd, int AcademicYearId, int TermId);
