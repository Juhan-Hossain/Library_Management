namespace LibraryManagement.Application.Features.Books;

public sealed record GetBooksQuery : IRequest<PagedResult<BookDto>>, IPagedQuery
{
    //Matches title, genre or author name.
    public string? Search { get; init; }
    //Exact ISBN (hyphens and spaces allowed)
    public string? Isbn { get; init; }
    public Guid? AuthorId { get; init; }
    public Guid? LibraryId { get; init; }
    public bool? Available { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = Paging.DefaultPageSize;
}

public sealed class GetBooksQueryValidator : PagedQueryValidator<GetBooksQuery>
{
    public GetBooksQueryValidator()
    {
        RuleFor(q => q.Isbn)
            .Must(Isbn.IsValid).WithMessage("'{PropertyValue}' is not a valid ISBN-10 or ISBN-13.")
            .When(q => !string.IsNullOrWhiteSpace(q.Isbn));
    }
}

public sealed class GetBooksQueryHandler(IReadDbContext db)
    : IRequestHandler<GetBooksQuery, PagedResult<BookDto>>
{
    public Task<PagedResult<BookDto>> Handle(GetBooksQuery request, CancellationToken cancellationToken)
    {
        var books = db.Books;

        var search = request.Search?.Trim();
        if (!string.IsNullOrEmpty(search))
            books = books.Where(b =>
                b.Title.Contains(search) ||
                b.Genre.Contains(search) ||
                b.Author!.FirstName.Contains(search) ||
                b.Author!.LastName.Contains(search));

        if (!string.IsNullOrWhiteSpace(request.Isbn))
        {
            var isbn = Isbn.Create(request.Isbn);
            books = books.Where(b => b.Isbn == isbn);
        }

        if (request.AuthorId is { } authorId)
            books = books.Where(b => b.AuthorId == authorId);

        if (request.LibraryId is { } libraryId)
            books = books.Where(b => b.LibraryId == libraryId);

        if (request.Available is { } available)
            books = available
                ? books.Where(b => b.AvailableCopies > 0)
                : books.Where(b => b.AvailableCopies == 0);

        return books
            .OrderBy(b => b.Title).ThenBy(b => b.Id)
            .ToPagedResultAsync(BookProjections.ToDto, request, cancellationToken);
    }
}