namespace Myschools.Api.Models;

public sealed record ExamRemarkRequest(int StudentId, int ExamId, string Remark, bool IsHeadTeacher);
