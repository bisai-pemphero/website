namespace Myschools.Api.Models;

public sealed record GradeRequest(int StudentId, int ExamId, int SubjectId, decimal Marks, int TeacherId);
