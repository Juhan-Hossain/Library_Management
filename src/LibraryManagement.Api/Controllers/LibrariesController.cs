using LibraryManagement.Api.Contracts;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.Features.Libraries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

public sealed class LibrariesController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<LibraryDto>>> GetAll(
        [FromQuery] GetLibrariesQuery query, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LibraryDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(new GetLibraryByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<LibraryDto>> Create(CreateLibraryCommand command, CancellationToken cancellationToken)
    {
        var id = await Sender.Send(command, cancellationToken);
        var library = await Sender.Send(new GetLibraryByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, library);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateLibraryRequest request, CancellationToken cancellationToken)
    {
        await Sender.Send(new UpdateLibraryCommand(id, request.Name, request.Address, request.Phone), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteLibraryCommand(id), cancellationToken);
        return NoContent();
    }
}