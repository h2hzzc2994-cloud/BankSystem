namespace WebApplication1.Application.Interfaces;

public interface IUnitOfWork
{
	Task<int> SaveChangesAsync();

	Task BeginTransactionAsync();

	Task CommitTransactionAsync();

	Task RollbackTransactionAsync();
}