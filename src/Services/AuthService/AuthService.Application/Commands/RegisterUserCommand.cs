using MediatR;

namespace AuthService.Application.Commands;

public class RegisterUserCommand : IRequest<Guid>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}