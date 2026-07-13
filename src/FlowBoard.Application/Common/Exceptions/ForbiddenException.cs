namespace FlowBoard.Application.Common.Exceptions;

public class ForbiddenException : Exception
{
    public ForbiddenException()
        : base("Você não tem permissão para realizar esta ação.") { }

    public ForbiddenException(string message)
        : base(message) { }
}
