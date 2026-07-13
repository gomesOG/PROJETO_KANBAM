using FlowBoard.Application.DTOs;
using MediatR;

namespace FlowBoard.Application.Features.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery(Guid ProjectId) : IRequest<ProjectDto>;
