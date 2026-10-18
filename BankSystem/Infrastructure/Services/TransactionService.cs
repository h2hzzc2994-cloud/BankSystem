using Microsoft.EntityFrameworkCore;
using WebApplication1.Application.DTOs.Transactions;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using WebApplication1.Infrastructure.Data;

namespace WebApplication1.Application.Services;

public class TransactionService : ITransactionService
{
	private readonly BankDbContext _context;
	private readonly IEmailService _emailService;

	private const decimal DailyLimit = 10000m;

	public TransactionService(
		BankDbContext context,
		IEmailService emailService)
	{
		_context = context;
		_emailService = emailService;
	}

	public async Task TransferAsync(
		int userId,
		TransferRequest request)
	{
		if (request.Amount <= 0)
			throw new Exception("Amount must be greater than zero.");

		if (request.FromIBAN == request.ToIBAN)
			throw new Exception(
				"Sender and receiver cannot be the same.");

		await using var transaction =
			await _context.Database.BeginTransactionAsync();

		try
		{
			var fromAccount =
				await _context.Accounts
					.FirstOrDefaultAsync(x =>
						x.IBAN == request.FromIBAN);

			var toAccount =
				await _context.Accounts
					.FirstOrDefaultAsync(x =>
						x.IBAN == request.ToIBAN);

			if (fromAccount == null)
				throw new Exception("Sender account not found.");

			if (toAccount == null)
				throw new Exception("Receiver account not found.");

			if (fromAccount.UserId != userId)
				throw new UnauthorizedAccessException(
					"You can only transfer from your own account.");

			if (fromAccount.IsBlocked)
				throw new Exception(
					"Blocked account cannot make transfers.");

			if (toAccount.IsBlocked)
				throw new Exception(
					"Receiver account is blocked.");

			if (fromAccount.Currency != toAccount.Currency)
				throw new Exception(
					"Accounts must have the same currency.");

			if (fromAccount.Balance < request.Amount)
				throw new Exception(
					"Insufficient balance.");

			var today = DateTime.UtcNow.Date;

			var dailyTotal =
				await _context.Transactions
					.Where(x =>
						x.FromAccountId == fromAccount.Id &&
						x.CreatedAt >= today &&
						x.CreatedAt < today.AddDays(1))
					.SumAsync(x => (decimal?)x.Amount) ?? 0;

			if (dailyTotal + request.Amount > DailyLimit)
				throw new Exception(
					"Daily transfer limit exceeded.");

			fromAccount.Balance -= request.Amount;

			toAccount.Balance += request.Amount;

			var transactionEntity = new Transaction
			{
				Amount = request.Amount,
				Type = "Transfer",
				FromAccountId = fromAccount.Id,
				ToAccountId = toAccount.Id,
				CreatedAt = DateTime.UtcNow,
				Description = request.Description
			};

			_context.Transactions.Add(transactionEntity);

			await _context.SaveChangesAsync();

			await transaction.CommitAsync();

			await _emailService.SendEmailAsync(
				"YOUR_EMAIL_HERE",
				"Bank transaction",
				$"Transfer of {request.Amount} {fromAccount.Currency} completed.");
		}
		catch
		{
			await transaction.RollbackAsync();

			throw;
		}
	}

	public async Task<List<TransactionResponse>>
		GetTransactionsAsync(
			int userId,
			TransactionFilterRequest filter)
	{
		var query = _context.Transactions
			.Include(x => x.FromAccount)
			.Include(x => x.ToAccount)
			.Where(x =>
				x.FromAccount.UserId == userId ||
				x.ToAccount.UserId == userId)
			.AsQueryable();

		if (filter.FromDate.HasValue)
		{
			query = query.Where(x =>
				x.CreatedAt >= filter.FromDate.Value);
		}

		if (filter.ToDate.HasValue)
		{
			var endDate =
				filter.ToDate.Value.Date.AddDays(1);

			query = query.Where(x =>
				x.CreatedAt < endDate);
		}

		if (!string.IsNullOrWhiteSpace(filter.Type))
		{
			query = query.Where(x =>
				x.Type == filter.Type);
		}

		if (filter.MinAmount.HasValue)
		{
			query = query.Where(x =>
				x.Amount >= filter.MinAmount.Value);
		}

		if (filter.MaxAmount.HasValue)
		{
			query = query.Where(x =>
				x.Amount <= filter.MaxAmount.Value);
		}

		if (!string.IsNullOrWhiteSpace(filter.IBAN))
		{
			query = query.Where(x =>
				x.FromAccount.IBAN == filter.IBAN ||
				x.ToAccount.IBAN == filter.IBAN);
		}

		var transactions = await query
			.OrderByDescending(x => x.CreatedAt)
			.ToListAsync();

		return transactions.Select(x =>
			new TransactionResponse
			{
				Id = x.Id,
				Amount = x.Amount,
				Type = x.Type,
				FromIBAN = x.FromAccount.IBAN,
				ToIBAN = x.ToAccount.IBAN,
				CreatedAt = x.CreatedAt,
				Description = x.Description
			}).ToList();
	}
}