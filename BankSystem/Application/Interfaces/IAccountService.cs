using WebApplication1.Application.DTOs.Accounts;

namespace WebApplication1.Application.Interfaces;

public interface IAccountService
{
	Task<AccountResponse> CreateAccountAsync(
		int userId,
		CreateAccountRequest request);

	Task<List<AccountResponse>> GetMyAccountsAsync(
		int userId);
}