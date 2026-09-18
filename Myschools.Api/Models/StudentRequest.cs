using System;

namespace Myschools.Api.Models;

public sealed record StudentRequest(string FirstName, string? MiddleName, string LastName, string StudentGender, DateTime DateOfBirth, DateTime AdmissionDate, int CurrentClassId, int PreviousClassId, string? PreviousSchool, string? SpecialNeeds, string ParentFullName, string ParentGender, string ParentPhone, string? ParentAlternatePhone, string? ParentEmail, string? ParentAddress, string ParentRelationship, string? ParentOccupation);
