using FlowBoard.Application.DTOs;
using MediatR;

namespace FlowBoard.Application.Features.Auth.Commands.RegisterUser;

public record RegisterUserCommand(
    string Name,
    string Email,
    string Password
) : IRequest<UserDto>;
