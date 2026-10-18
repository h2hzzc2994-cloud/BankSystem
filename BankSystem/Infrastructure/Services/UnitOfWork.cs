using Microsoft.EntityFrameworkCore.Storage;
using WebApplication1.Application.Interfaces;
using WebApplication1.Infrastructure.Data;

namespace WebApplication1.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
	private readonly BankDbContext _context;

	private IDbContextTransaction? _transaction;

	public UnitOfWork(BankDbContext context)
	{
		_context = context;
	}

	public async Task<int> SaveChangesAsync()
	{
		return await _context.SaveChangesAsync();
	}

	public async Task BeginTransactionAsync()
	{
		_transaction =
			await _context.Database.BeginTransactionAsync();
	}

	public async Task CommitTransactionAsync()
	{
		if (_transaction != null)
		{
			await _transaction.CommitAsync();
			await _transaction.DisposeAsync();
		}
	}

	public async Task RollbackTransactionAsync()
	{
		if (_transaction != null)
		{
			await _transaction.RollbackAsync();
			await _transaction.DisposeAsync();
		}
	}
}