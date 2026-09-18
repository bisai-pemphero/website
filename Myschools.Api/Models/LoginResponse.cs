namespace Myschools.Api.Models;

public sealed record LoginResponse(string AccessToken, string Username, int RoleId, int SchoolId);
