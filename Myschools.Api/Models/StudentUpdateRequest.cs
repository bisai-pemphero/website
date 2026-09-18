using System;

namespace Myschools.Api.Models;

public sealed record StudentUpdateRequest(string FirstName, string? MiddleName, string LastName, string Gender, DateTime DateOfBirth, DateTime AdmissionDate, int CurrentClassId, int PreviousClassId, string? PreviousSchool, string? SpecialNeeds, int ParentId, string ParentFullName, string ParentGender, string ParentPhone, string? ParentAlternatePhone, string? ParentEmail, string? ParentAddress, string ParentRelationship, string? ParentOccupation);
