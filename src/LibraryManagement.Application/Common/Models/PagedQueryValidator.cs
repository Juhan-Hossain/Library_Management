namespace LibraryManagement.Application.Common.Models;

public abstract class PagedQueryValidator<T> : AbstractValidator<T> where T : IPagedQuery
{
    protected PagedQueryValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, Paging.MaxPageSize);
    }
}