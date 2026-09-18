using System;

namespace Myschools.Api.Models;

public sealed record FeePaymentRequest(int FeesId, decimal Amount, string PaymentMode, string PaidBy, string? PaymentReference, DateTime PaymentDate);
