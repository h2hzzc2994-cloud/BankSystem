using WebApplication1.Application.DTOs.Auth;

namespace WebApplication1.Application.Interfaces;

public interface IAuthService
{
	Task<LoginResponse> RegisterAsync(RegisterRequest request);

	Task<LoginResponse> LoginAsync(LoginRequest request);

	Task<LoginResponse> RefreshAsync(string refreshToken);
}