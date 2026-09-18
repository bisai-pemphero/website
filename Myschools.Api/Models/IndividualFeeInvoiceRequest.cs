namespace Myschools.Api.Models;

public sealed record IndividualFeeInvoiceRequest(int StudentId, int TermId, int FeesCategoryId);
