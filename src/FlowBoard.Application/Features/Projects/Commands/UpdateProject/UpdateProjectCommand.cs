using MediatR;

namespace FlowBoard.Application.Features.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(
    Guid ProjectId,
    string Name,
    string? Description,
    string? Color
) : IRequest;
