using WebApplication1.Domain.Entities;

namespace BankSystem.Application.Interfaces
{
	public interface IJwtService
	{
		string GenerateAccessToken(User user);
		string GenerateRefreshToken();
	}
}
