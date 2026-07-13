using FlowBoard.Application.Features.Boards.Commands.AddColumn;
using FlowBoard.Application.Features.Boards.Commands.CreateBoard;
using FlowBoard.Application.Features.Boards.Commands.ReorderColumns;
using FlowBoard.Application.Features.Boards.Queries.GetBoardById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.API.Controllers;

[Route("api/boards")]
public class BoardsController(ISender sender) : ApiController(sender)
{
    /// <summary>Retorna o board completo com colunas e cards.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetBoardByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Cria um novo board em um projeto.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateBoardCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Adiciona uma coluna ao board.</summary>
    [HttpPost("{id:guid}/columns")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddColumn(
        Guid id,
        [FromBody] AddColumnRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(
            new AddColumnCommand(id, request.Name, request.Color, request.CardLimit),
            cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>Reordena as colunas do board (drag-and-drop).</summary>
    [HttpPatch("{id:guid}/columns/reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ReorderColumns(
        Guid id,
        [FromBody] ReorderColumnsRequest request,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new ReorderColumnsCommand(id, request.OrderedColumnIds), cancellationToken);
        return NoContent();
    }
}

public record AddColumnRequest(string Name, string? Color, int? CardLimit);
public record ReorderColumnsRequest(IReadOnlyList<Guid> OrderedColumnIds);
