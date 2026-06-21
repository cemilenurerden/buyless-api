using AuthService.Application.Interfaces;
using AuthService.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace AuthService.Application.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResult>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<LoginResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (existingToken == null || !existingToken.IsActive)
        {
            throw new UnauthorizedAccessException("Geçersiz veya süresi dolmuş refresh token.");
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId);

        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı veya aktif değil.");
        }

        existingToken.Revoke();

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"]!);
        var newRefreshToken = new RefreshToken(newRefreshTokenValue, user.Id, DateTime.UtcNow.AddDays(refreshTokenExpiryDays));

        await _refreshTokenRepository.AddAsync(newRefreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new LoginResult
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue
        };
    }
}