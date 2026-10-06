using LibraryManagement.Application.Common.Models;
using LibraryManagement.Application.Features.Loans;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Api.Controllers;

public sealed class LoansController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet]
    [ProducesResponseType<PagedResult<LoanDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<LoanDto>>> GetAll(
        [FromQuery] GetLoansQuery query, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(query, cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType<LoanDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoanDto>> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Sender.Send(new GetLoanByIdQuery(id), cancellationToken));

    /// <summary>
    /// Borrows a book for 14 days. 409 if: no copies available, member inactive, member already has
    /// 5 active loans, member already holds this book, or the book belongs to another library.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<LoanDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoanDto>> Borrow(BorrowBookCommand command, CancellationToken cancellationToken)
    {
        var id = await Sender.Send(command, cancellationToken);
        var loan = await Sender.Send(new GetLoanByIdQuery(id), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, loan);
    }

    [HttpPost("{id:guid}/return")]
    [ProducesResponseType<LoanDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoanDto>> Return(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new ReturnBookCommand(id), cancellationToken);
        return Ok(await Sender.Send(new GetLoanByIdQuery(id), cancellationToken));
    }
}