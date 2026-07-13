namespace FlowBoard.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"'{name}' com identificador '{key}' não foi encontrado.") { }

    public NotFoundException(string message)
        : base(message) { }
}
