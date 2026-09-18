namespace Myschools.Api.Models;

public sealed record UserUpdateRequest(string Username, string Fullname, int RoleId, string? Password);
