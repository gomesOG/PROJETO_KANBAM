using FlowBoard.Application.Features.Projects.Commands.AddProjectMember;
using FlowBoard.Application.Features.Projects.Commands.ArchiveProject;
using FlowBoard.Application.Features.Projects.Commands.CreateProject;
using FlowBoard.Application.Features.Projects.Commands.UpdateProject;
using FlowBoard.Application.Features.Projects.Queries.GetProjectById;
using FlowBoard.Application.Features.Projects.Queries.GetProjects;
using FlowBoard.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlowBoard.API.Controllers;

[Route("api/projects")]
public class ProjectsController(ISender sender) : ApiController(sender)
{
    /// <summary>Lista todos os projetos do usuário autenticado.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetProjectsQuery(), cancellationToken);
        return Ok(result);
    }

    /// <summary>Retorna um projeto pelo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(new GetProjectByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Cria um novo projeto.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProjectCommand command,
        CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>Atualiza nome, descrição e cor de um projeto.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new UpdateProjectCommand(id, request.Name, request.Description, request.Color), cancellationToken);
        return NoContent();
    }

    /// <summary>Arquiva um projeto.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Archive(Guid id, CancellationToken cancellationToken)
    {
        await Sender.Send(new ArchiveProjectCommand(id), cancellationToken);
        return NoContent();
    }

    /// <summary>Adiciona um membro ao projeto.</summary>
    [HttpPost("{id:guid}/members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddMember(
        Guid id,
        [FromBody] AddMemberRequest request,
        CancellationToken cancellationToken)
    {
        await Sender.Send(new AddProjectMemberCommand(id, request.UserId, request.Role), cancellationToken);
        return NoContent();
    }
}

public record UpdateProjectRequest(string Name, string? Description, string? Color);
public record AddMemberRequest(Guid UserId, ProjectRole Role);
