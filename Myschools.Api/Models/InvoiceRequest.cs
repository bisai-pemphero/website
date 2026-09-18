namespace Myschools.Api.Models;

public sealed record InvoiceRequest(int SchoolId, string Description, decimal Total);
