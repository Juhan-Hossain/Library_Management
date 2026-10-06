using LibraryManagement.Api.Contracts;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.Features.Libraries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

public sealed class LibrariesController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    [ProducesResponseType<PagedResult<LibraryDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<LibraryDto>>> GetAll(
        [FromQuery] GetLibrariesQuery query, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<LibraryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LibraryDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(new GetLibraryByIdQuery(id), cancellationToken));

    [HttpPost]
    [ProducesResponseType<LibraryDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LibraryDto>> Create(CreateLibraryCommand command, CancellationToken cancellationToken)
    {
        var id = await Sender.Send(command, cancellationToken);
        var library = await Sender.Send(new GetLibraryByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, library);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateLibraryRequest request, CancellationToken cancellationToken)
    {
        await Sender.Send(new UpdateLibraryCommand(id, request.Name, request.Address, request.Phone), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteLibraryCommand(id), cancellationToken);
        return NoContent();
    }
}