using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Myschools.Api.Models;

namespace Myschools.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/sms")]
public sealed class SmsController(IHttpClientFactory clientFactory, IConfiguration configuration) : ControllerBase
{
	[HttpPost]
	public async Task<IActionResult> Send(SmsRequest request, CancellationToken cancellationToken)
	{
		string text = configuration["Sms:ApiKey"];
		string text2 = configuration["Sms:SenderId"];
		string value = configuration["Sms:Endpoint"];
		if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(text2) || string.IsNullOrWhiteSpace(value))
		{
			return Problem("SMS provider configuration is missing.", null, 503);
		}
		string requestUri = $"{value}?APIKEY={Uri.EscapeDataString(text)}&senderid={Uri.EscapeDataString(text2)}&channel=promo&DCS=0&flashsms=0&number={Uri.EscapeDataString(request.Number)}&text={Uri.EscapeDataString(request.Message)}&route=15";
		HttpResponseMessage response = await clientFactory.CreateClient().GetAsync(requestUri, cancellationToken);
		string response2 = await response.Content.ReadAsStringAsync(cancellationToken);
		return StatusCode((int)response.StatusCode, new
		{
			sent = response.IsSuccessStatusCode,
			response = response2
		});
	}
}
