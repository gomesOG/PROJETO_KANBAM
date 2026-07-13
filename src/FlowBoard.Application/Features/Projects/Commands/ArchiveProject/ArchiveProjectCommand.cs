using MediatR;

namespace FlowBoard.Application.Features.Projects.Commands.ArchiveProject;

public record ArchiveProjectCommand(Guid ProjectId) : IRequest;
