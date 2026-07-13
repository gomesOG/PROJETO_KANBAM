using FlowBoard.Application.DTOs;
using MediatR;

namespace FlowBoard.Application.Features.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    string Name,
    string? Description,
    string? Color
) : IRequest<ProjectDto>;
