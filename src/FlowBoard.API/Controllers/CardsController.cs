using FlowBoard.Application.Features.Cards.Commands.AssignUser;
using FlowBoard.Application.Features.Cards.Commands.CreateCard;
using FlowBoard.Application.Features.Cards.Commands.MoveCard;
using FlowBoard.Application.Features.Cards.Commands.ToggleChecklistItem;
using FlowBoard.Application.Features.Cards.Commands.UpdateCard;
using FlowBoard.Application.Features.Cards.Queries.GetCardById;
using FlowBoard.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.API.Controllers;

[Route("api/cards")]
public class CardsController(ISender sender) : ApiController(sender)
{
    /// <summary>Retorna os detalhes completos de um card.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetCardByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Cria um card em uma coluna.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCardCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Atualiza título, descrição, prioridade e data de vencimento.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCardRequest request,
        CancellationToken cancellationToken)
    {
        await Sender.Send(
            new UpdateCardCommand(id, request.Title, request.Description, request.Priority, request.DueDate),
            cancellationToken);
        return NoContent();
    }

    /// <summary>Move um card para outra coluna e/ou posição (drag-and-drop).</summary>
    [HttpPatch("{id:guid}/move")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Move(
        Guid id,
        [FromBody] MoveCardRequest request,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new MoveCardCommand(id, request.TargetColumnId, request.NewPosition), cancellationToken);
        return NoContent();
    }

    /// <summary>Atribui ou remove um responsável do card.</summary>
    [HttpPatch("{id:guid}/assignees")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(
        Guid id,
        [FromBody] AssignUserRequest request,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new AssignUserCommand(id, request.UserId, request.Assign), cancellationToken);
        return NoContent();
    }

    /// <summary>Marca ou desmarca um item de checklist.</summary>
    [HttpPatch("{id:guid}/checklist/{itemId:guid}/toggle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleChecklist(
        Guid id,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new ToggleChecklistItemCommand(id, itemId), cancellationToken);
        return NoContent();
    }
}

public record UpdateCardRequest(string Title, string? Description, Priority Priority, DateTime? DueDate);
public record MoveCardRequest(Guid TargetColumnId, int NewPosition);
public record AssignUserRequest(Guid UserId, bool Assign);
