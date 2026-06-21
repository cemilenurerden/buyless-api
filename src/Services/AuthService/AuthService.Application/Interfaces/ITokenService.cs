using AuthService.Domain;

namespace AuthService.Application.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}