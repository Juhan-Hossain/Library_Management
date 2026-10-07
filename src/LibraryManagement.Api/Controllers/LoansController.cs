using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.Features.Loans;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

public sealed class LoansController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<LoanDto>>> GetAll(
        [FromQuery] GetLoansQuery query, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LoanDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(new GetLoanByIdQuery(id), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<LoanDto>> Borrow(BorrowBookCommand command, CancellationToken cancellationToken)
    {
        var id = await Sender.Send(command, cancellationToken);
        var loan = await Sender.Send(new GetLoanByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, loan);
    }

    [HttpPost("{id:guid}/return")]
    public async Task<ActionResult<LoanDto>> Return(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new ReturnBookCommand(id), cancellationToken);
        return Ok(await Sender.Send(new GetLoanByIdQuery(id), cancellationToken));
    }
}