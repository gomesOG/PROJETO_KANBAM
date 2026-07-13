using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Domain.Entities;
using FlowBoard.Application.DTOs;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Projects.Queries.GetProjectById;

public class GetProjectByIdQueryHandler(
    IProjectRepository projectRepository,
    ICurrentUserService currentUser
) : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetWithBoardsAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        var isMember = await projectRepository.IsUserMemberAsync(request.ProjectId, currentUser.UserId, cancellationToken);
        if (!isMember) throw new ForbiddenException();

        return new ProjectDto(
            project.Id, project.Name, project.Description, project.Color,
            project.IsArchived, project.OwnerId,
            project.Members.Count, project.Boards.Count, project.CreatedAt);
    }
}
