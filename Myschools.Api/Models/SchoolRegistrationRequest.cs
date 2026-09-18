using Microsoft.AspNetCore.Http;

namespace Myschools.Api.Models;

public sealed class SchoolRegistrationRequest
{
	public string SchoolName { get; set; } = string.Empty;

	public string SchoolEmail { get; set; } = string.Empty;

	public string PhoneNumber { get; set; } = string.Empty;

	public string SchoolAddress { get; set; } = string.Empty;

	public string Slogan { get; set; } = string.Empty;

	public string AdminName { get; set; } = string.Empty;

	public string AdminEmail { get; set; } = string.Empty;

	public string AdminPhone { get; set; } = string.Empty;

	public string AdminPassword { get; set; } = string.Empty;

	public IFormFile? Logo { get; set; }

	public IFormFile? Letterhead { get; set; }
}
