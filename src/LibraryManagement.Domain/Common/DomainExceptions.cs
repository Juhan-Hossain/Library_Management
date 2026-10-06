namespace LibraryManagement.Domain.Common;

/// <summary>Base type for every rule the domain model enforces.</summary>
public abstract class DomainException(string message) : Exception(message);

/// <summary>Input is structurally invalid (blank name, bad ISBN, ...).</summary>
public sealed class DomainValidationException(string message) : DomainException(message);

/// <summary>Input is valid but the current state forbids the operation.</summary>
public sealed class BusinessRuleViolationException(string message) : DomainException(message);