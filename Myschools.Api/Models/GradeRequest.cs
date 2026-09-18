namespace Myschools.Api.Models;

public sealed record GradeRequest(int StudentId, int ExamId, int SubjectId, decimal Marks, int TeacherId);

public sealed record GradingSystemRequest(string Grade, decimal MinimumMark, decimal MaximumMark, string Remark, string? Level);
