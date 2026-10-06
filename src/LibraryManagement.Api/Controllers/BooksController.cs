using LibraryManagement.Api.Contracts;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.Features.Books;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

public sealed class BooksController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    [ProducesResponseType<PagedResult<BookDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<BookDto>>> GetAll(
        [FromQuery] GetBooksQuery query, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<BookDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(new GetBookByIdQuery(id), cancellationToken));

    [HttpPost]
    [ProducesResponseType<BookDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookDto>> Create(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var id = await Sender.Send(command, cancellationToken);
        var book = await Sender.Send(new GetBookByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, book);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, UpdateBookRequest request, CancellationToken cancellationToken)
    {
        await Sender.Send(new UpdateBookCommand(id, request.Title, request.Isbn, request.PublishedYear,
            request.Genre, request.TotalCopies, request.LibraryId, request.AuthorId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteBookCommand(id), cancellationToken);
        return NoContent();
    }
}