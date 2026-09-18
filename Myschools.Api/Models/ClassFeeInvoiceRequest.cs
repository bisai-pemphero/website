namespace Myschools.Api.Models;

public sealed record ClassFeeInvoiceRequest(int ClassId, int TermId, int FeesCategoryId);
