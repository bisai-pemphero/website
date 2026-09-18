using System;

namespace Myschools.Api.Models;

public sealed record LicenceRequest(string Category, DateTime StartDate, DateTime ExpiryDate);
