using MediatR;

namespace FlowBoard.Application.Features.Boards.Commands.ReorderColumns;

public record ReorderColumnsCommand(
    Guid BoardId,
    IReadOnlyList<Guid> OrderedColumnIds
) : IRequest;
