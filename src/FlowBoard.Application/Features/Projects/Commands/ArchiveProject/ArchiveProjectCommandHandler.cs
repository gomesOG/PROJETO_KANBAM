using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Domain.Entities;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Projects.Commands.ArchiveProject;

public class ArchiveProjectCommandHandler(
    IProjectRepository projectRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<ArchiveProjectCommand>
{
    public async Task Handle(ArchiveProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        if (project.OwnerId != currentUser.UserId)
            throw new ForbiddenException("Apenas o proprietário pode arquivar o projeto.");

        project.Archive();
        projectRepository.Update(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
