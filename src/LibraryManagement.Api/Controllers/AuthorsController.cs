using LibraryManagement.Api.Contracts;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.Features.Authors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

public sealed class AuthorsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    [ProducesResponseType<PagedResult<AuthorDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<AuthorDto>>> GetAll(
        [FromQuery] GetAuthorsQuery query, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<AuthorDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuthorDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(new GetAuthorByIdQuery(id), cancellationToken));

    [HttpPost]
    [ProducesResponseType<AuthorDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthorDto>> Create(CreateAuthorCommand command, CancellationToken cancellationToken)
    {
        var id = await Sender.Send(command, cancellationToken);
        var author = await Sender.Send(new GetAuthorByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, author);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateAuthorRequest request, CancellationToken cancellationToken)
    {
        await Sender.Send(new UpdateAuthorCommand(id, request.FirstName, request.LastName, request.Biography), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteAuthorCommand(id), cancellationToken);
        return NoContent();
    }
}