namespace Myschools.Api.Models;

public sealed class UserCreateRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Fullname { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int SchoolId { get; set; }
    public string? Location { get; set; }
    public string? Position { get; set; }
}
