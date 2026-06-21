using MediatR;

namespace AuthService.Application.Commands;

public class RefreshTokenCommand : IRequest<LoginResult>
{
    public string RefreshToken { get; set; } = string.Empty;
}