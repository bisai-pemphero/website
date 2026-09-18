namespace Myschools.Api.Models;

public sealed record TeacherAssignmentRequest(int ClassId, int TeacherId, string TeacherName);
