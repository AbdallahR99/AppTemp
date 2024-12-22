using AppTemp.Core.Identity.Tokens.Features.Generate;
using AppTemp.Core.Identity.Tokens.Features.Refresh;
using AppTemp.Core.Identity.Tokens.Models;

namespace AppTemp.Core.Identity.Tokens;
public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(TokenGenerationCommand request, string ipAddress, CancellationToken cancellationToken);
    Task<TokenResponse> RefreshTokenAsync(RefreshTokenCommand request, string ipAddress, CancellationToken cancellationToken);

}
