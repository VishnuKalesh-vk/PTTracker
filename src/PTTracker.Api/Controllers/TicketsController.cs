using System.Security.Claims;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTTracker.Application.Tickets.Commands.CreateManualTicket;
using PTTracker.Application.Tickets.Commands.CreateTicket;
using PTTracker.Application.Tickets.Commands.DeleteTicket;
using PTTracker.Application.Tickets.Commands.IngestEmail;
using PTTracker.Application.Tickets.Commands.SetJiraId;
using PTTracker.Application.Tickets.Commands.UpdateTicket;
using PTTracker.Application.Tickets.Queries.GetTicketById;
using PTTracker.Application.Tickets.Queries.ListTickets;
using PTTracker.Contracts.Common;
using PTTracker.Contracts.Tickets;

namespace PTTracker.Api.Controllers;

[Route("api/tickets")]
[Authorize]
public class TicketsController(ISender mediator) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateTicketRequest request)
    {
        var command = new CreateTicketCommand(
            request.CustomerId,
            request.Subject,
            request.Description,
            request.Priority,
            request.AssignedUserId,
            request.DescriptionHtml);

        var result = await mediator.Send(command);

        return result.Match(
            ticket => CreatedAtAction(
                nameof(GetById),
                new { id = ticket.Id },
                ApiResponse<TicketResponse>.Ok(ticket, "Ticket created.")),
            errors => Problem(errors));
    }

    [HttpPost("manual")]
    public async Task<IActionResult> CreateManual(CreateManualTicketRequest request)
    {
        var command = new CreateManualTicketCommand(
            request.CustomerName,
            request.CustomerEmail,
            request.Subject,
            request.Description,
            request.Priority,
            request.DescriptionHtml);

        var result = await mediator.Send(command);

        return result.Match(
            ticket => CreatedAtAction(
                nameof(GetById),
                new { id = ticket.Id },
                ApiResponse<TicketResponse>.Ok(ticket, "Ticket created.")),
            errors => Problem(errors));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await mediator.Send(new GetTicketByIdQuery(id));

        return result.Match(
            ticket => Ok(ApiResponse<TicketResponse>.Ok(ticket)),
            errors => Problem(errors));
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] TicketQueryParameters parameters)
    {
        var query = new ListTicketsQuery(
            parameters.Page,
            parameters.PageSize,
            parameters.Status,
            parameters.Priority,
            parameters.CustomerId,
            parameters.AssignedUserId,
            parameters.Search,
            parameters.SortBy,
            parameters.SortDescending);

        var result = await mediator.Send(query);

        return result.Match(
            page => Ok(ApiResponse<PagedResponse<TicketSummaryResponse>>.Ok(page)),
            errors => Problem(errors));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTicketRequest request)
    {
        var command = new UpdateTicketCommand(
            id,
            request.Subject,
            request.Description,
            request.Status,
            request.Priority,
            request.AssignedUserId,
            GetCurrentUserId(),
            request.DescriptionHtml);

        var result = await mediator.Send(command);

        return result.Match(
            ticket => Ok(ApiResponse<TicketResponse>.Ok(ticket, "Ticket updated.")),
            errors => Problem(errors));
    }

    [HttpPost("ingest-eml")]
    public async Task<IActionResult> IngestEml(IFormFile file)
    {
        if (file is null || file.Length == 0)
            return Problem(new List<Error> { Error.Validation("Email.Empty", "No file was uploaded.") });

        using var memory = new MemoryStream();
        await file.CopyToAsync(memory);

        var command = new IngestEmailCommand(memory.ToArray(), file.FileName);
        var result = await mediator.Send(command);

        return result.Match(
            ingest => Ok(ApiResponse<EmailIngestResponse>.Ok(ingest, ingest.Outcome)),
            errors => Problem(errors));
    }

    [HttpPut("{id:guid}/jira")]
    public async Task<IActionResult> SetJiraId(Guid id, SetJiraIdRequest request)
    {
        var result = await mediator.Send(new SetJiraIdCommand(id, request.JiraId));

        return result.Match(
            ticket => Ok(ApiResponse<TicketResponse>.Ok(ticket, "JIRA ID updated.")),
            errors => Problem(errors));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteTicketCommand(id));

        return result.Match(
            _ => NoContent(),
            errors => Problem(errors));
    }

    private Guid? GetCurrentUserId()
    {
        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
                      ?? User.FindFirstValue("sub");

        return Guid.TryParse(subject, out var userId) ? userId : null;
    }
}
