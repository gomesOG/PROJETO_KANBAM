namespace FlowBoard.Application.DTOs;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    string? AvatarUrl,
    bool IsActive,
    DateTime CreatedAt
);
