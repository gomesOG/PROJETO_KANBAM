using FlowBoard.Application.DTOs;
using MediatR;

namespace FlowBoard.Application.Features.Projects.Queries.GetProjects;

public record GetProjectsQuery : IRequest<IReadOnlyList<ProjectDto>>;
