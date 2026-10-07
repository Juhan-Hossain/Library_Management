using LibraryManagement.Api.Contracts;
using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.Features.Loans;
using LibraryManagement.Application.Features.Members;
using LibraryManagement.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

public sealed class MembersController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<MemberDto>>> GetAll(
        [FromQuery] GetMembersQuery query, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MemberDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(new GetMemberByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<MemberDto>> Create(CreateMemberCommand command, CancellationToken cancellationToken)
    {
        var id = await Sender.Send(command, cancellationToken);
        var member = await Sender.Send(new GetMemberByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, member);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateMemberRequest request, CancellationToken cancellationToken)
    {
        await Sender.Send(new UpdateMemberCommand(id, request.FirstName, request.LastName, request.Email,
            request.Phone, request.LibraryId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new DeleteMemberCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/activate")]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new SetMemberStatusCommand(id, IsActive: true), cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new SetMemberStatusCommand(id, IsActive: false), cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/loans")]
    public async Task<ActionResult<PagedResult<LoanDto>>> GetLoans(
        Guid id,
        [FromQuery] LoanStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = Paging.DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        await Sender.Send(new GetMemberByIdQuery(id), cancellationToken); 
        var loans = await Sender.Send(
            new GetLoansQuery { MemberId = id, Status = status, Page = page, PageSize = pageSize },
            cancellationToken);
        return Ok(loans);
    }
}