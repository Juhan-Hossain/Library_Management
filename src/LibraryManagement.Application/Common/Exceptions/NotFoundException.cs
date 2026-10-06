namespace LibraryManagement.Application.Common.Exceptions;

public sealed class NotFoundException(string resource, Guid id)
    : Exception($"{resource} with id '{id}' was not found.");