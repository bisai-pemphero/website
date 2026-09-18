using System.Security.Claims;

namespace Myschools.Api.Data;

public static class UserContext
{
	public static int GetSchoolId(this ClaimsPrincipal user)
	{
		string s = user.FindFirst("schoolId")?.Value ?? user.FindFirst("SchoolId")?.Value ?? user.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/groupsid")?.Value;
		if (int.TryParse(s, out var result))
		{
			return result;
		}
		return 0;
	}

	public static int GetRoleId(this ClaimsPrincipal user)
	{
		string s = user.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value ?? user.FindFirst("roleId")?.Value;
		if (!int.TryParse(s, out var result))
		{
			return 0;
		}
		return result;
	}
}
