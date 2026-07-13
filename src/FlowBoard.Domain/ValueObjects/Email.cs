using FlowBoard.Domain.Common;

namespace FlowBoard.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail não pode ser vazio.", nameof(email));

        email = email.Trim().ToLowerInvariant();

        if (!email.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("E-mail inválido.", nameof(email));

        return new Email(email);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
