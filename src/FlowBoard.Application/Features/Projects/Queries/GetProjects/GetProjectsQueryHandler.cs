using FlowBoard.Application.DTOs;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler(
    IProjectRepository projectRepository,
    ICurrentUserService currentUser
) : IRequestHandler<GetProjectsQuery, IReadOnlyList<ProjectDto>>
{
    public async Task<IReadOnlyList<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await projectRepository.GetByUserIdAsync(currentUser.UserId, cancellationToken);

        return projects
            .Where(p => !p.IsArchived)
            .Select(p => new ProjectDto(
                p.Id, p.Name, p.Description, p.Color,
                p.IsArchived, p.OwnerId,
                p.Members.Count, p.Boards.Count, p.CreatedAt))
            .ToList()
            .AsReadOnly();
    }
}
