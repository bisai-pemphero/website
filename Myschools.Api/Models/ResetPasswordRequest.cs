namespace Myschools.Api.Models;

public sealed record ResetPasswordRequest(string Username, string ResetToken, string NewPassword);
