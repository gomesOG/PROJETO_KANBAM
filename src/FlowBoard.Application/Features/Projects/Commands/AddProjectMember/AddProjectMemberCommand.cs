using FlowBoard.Domain.Enums;
using MediatR;

namespace FlowBoard.Application.Features.Projects.Commands.AddProjectMember;

public record AddProjectMemberCommand(
    Guid ProjectId,
    Guid UserId,
    ProjectRole Role
) : IRequest;
