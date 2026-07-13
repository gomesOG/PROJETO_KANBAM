using FlowBoard.Application.DTOs;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Entities;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler(
    IProjectRepository projectRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = Project.Create(request.Name, currentUser.UserId, request.Description, request.Color);

        await projectRepository.AddAsync(project, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProjectDto(
            project.Id, project.Name, project.Description, project.Color,
            project.IsArchived, project.OwnerId,
            project.Members.Count, project.Boards.Count, project.CreatedAt);
    }
}
