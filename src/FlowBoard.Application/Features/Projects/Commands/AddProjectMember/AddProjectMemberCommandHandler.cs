using FlowBoard.Application.Common.Exceptions;
using FlowBoard.Domain.Entities;
using FlowBoard.Application.Interfaces;
using FlowBoard.Domain.Interfaces;
using FlowBoard.Domain.Interfaces.Repositories;
using MediatR;

namespace FlowBoard.Application.Features.Projects.Commands.AddProjectMember;

public class AddProjectMemberCommandHandler(
    IProjectRepository projectRepository,
    IUserRepository userRepository,
    ICurrentUserService currentUser,
    IUnitOfWork unitOfWork
) : IRequestHandler<AddProjectMemberCommand>
{
    public async Task Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Project), request.ProjectId);

        if (project.OwnerId != currentUser.UserId)
            throw new ForbiddenException("Apenas o proprietário pode adicionar membros.");

        var userExists = await userRepository.ExistsAsync(request.UserId, cancellationToken);
        if (!userExists)
            throw new NotFoundException(nameof(User), request.UserId);

        project.AddMember(request.UserId, request.Role);
        projectRepository.Update(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
