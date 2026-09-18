namespace Myschools.Api.Models;

public sealed record AcademicTermRequest(string TermName, string StartDate, string EndDate, int AcademicYearId, string IsActive);
