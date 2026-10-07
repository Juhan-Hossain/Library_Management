using LibraryManagement.Api.Contracts;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.Features.Authors;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

public sealed class AuthorsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<AuthorDto>>> GetAll(
        [FromQuery] GetAuthorsQuery query, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuthorDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(new GetAuthorByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<AuthorDto>> Create(CreateAuthorCommand command, CancellationToken cancellationToken)
    {
        var id = await Sender.Send(command, cancellationToken);
        var author = await Sender.Send(new GetAuthorByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, author);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAuthorRequest request, CancellationToken cancellationToken)
    {
        await Sender.Send(new UpdateAuthorCommand(id, request.FirstName, request.LastName, request.Biography), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteAuthorCommand(id), cancellationToken);
        return NoContent();
    }
}