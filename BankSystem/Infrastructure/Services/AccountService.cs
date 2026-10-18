using Microsoft.EntityFrameworkCore;
using WebApplication1.Application.DTOs.Accounts;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using WebApplication1.Infrastructure.Data;

namespace WebApplication1.Application.Services;

public class AccountService : IAccountService
{
	private readonly BankDbContext _context;

	public AccountService(BankDbContext context)
	{
		_context = context;
	}

	public async Task<AccountResponse> CreateAccountAsync(
		int userId,
		CreateAccountRequest request)
	{
		var currency = request.Currency.ToUpper();

		var allowedCurrencies = new[]
		{
			"GEL",
			"USD",
			"EUR"
		};

		if (!allowedCurrencies.Contains(currency))
			throw new Exception("Unsupported currency.");

		var iban = GenerateIban();

		var account = new Account
		{
			UserId = userId,
			IBAN = iban,
			Currency = currency,
			Balance = 0,
			IsBlocked = false
		};

		_context.Accounts.Add(account);

		await _context.SaveChangesAsync();

		return Map(account);
	}

	public async Task<List<AccountResponse>> GetMyAccountsAsync(
		int userId)
	{
		var accounts = await _context.Accounts
			.Where(x => x.UserId == userId)
			.ToListAsync();

		return accounts.Select(Map).ToList();
	}

	private static AccountResponse Map(Account account)
	{
		return new AccountResponse
		{
			Id = account.Id,
			IBAN = account.IBAN,
			Currency = account.Currency,
			Balance = account.Balance,
			IsBlocked = account.IsBlocked
		};
	}

	private static string GenerateIban()
	{
		var number =
			Random.Shared.NextInt64(
				1000000000000000,
				9999999999999999);

		return $"GE{number}";
	}
}