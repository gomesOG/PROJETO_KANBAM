using FlowBoard.Domain.Common;
using FlowBoard.Domain.ValueObjects;

namespace FlowBoard.Domain.Entities;

public class User : AggregateRoot
{
    public string Name { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string? AvatarUrl { get; private set; }
    public bool IsActive { get; private set; }

    private User() { Name = null!; Email = null!; PasswordHash = null!; }

    public static User Create(string name, string email, string passwordHash, string? avatarUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(name));

        return new User
        {
            Name = name.Trim(),
            Email = Email.Create(email),
            PasswordHash = passwordHash,
            AvatarUrl = avatarUrl,
            IsActive = true
        };
    }

    public void UpdateProfile(string name, string? avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(name));

        Name = name.Trim();
        AvatarUrl = avatarUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
