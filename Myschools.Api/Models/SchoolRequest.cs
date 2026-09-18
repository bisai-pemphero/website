namespace Myschools.Api.Models;

public sealed record SchoolRequest(string SchoolName, string? SchoolEmail, string? PhoneNumber, string? SchoolAddress, string? Slogan, string? AdminName, string? AdminEmail, string? AdminPhone);
