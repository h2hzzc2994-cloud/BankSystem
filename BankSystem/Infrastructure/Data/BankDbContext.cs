using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Infrastructure.Data;

public class BankDbContext : DbContext
{
	public BankDbContext(DbContextOptions<BankDbContext> options)
		: base(options)
	{
	}

	public DbSet<User> Users => Set<User>();

	public DbSet<Account> Accounts => Set<Account>();

	public DbSet<Transaction> Transactions => Set<Transaction>();

	public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<User>()
			.HasIndex(x => x.Email)
			.IsUnique();

		modelBuilder.Entity<Account>()
			.HasIndex(x => x.IBAN)
			.IsUnique();

		modelBuilder.Entity<User>()
			.HasMany(x => x.Accounts)
			.WithOne(x => x.User)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<User>()
			.HasMany(x => x.RefreshTokens)
			.WithOne(x => x.User)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		modelBuilder.Entity<Transaction>()
			.HasOne(x => x.FromAccount)
			.WithMany(x => x.OutgoingTransactions)
			.HasForeignKey(x => x.FromAccountId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Transaction>()
			.HasOne(x => x.ToAccount)
			.WithMany(x => x.IncomingTransactions)
			.HasForeignKey(x => x.ToAccountId)
			.OnDelete(DeleteBehavior.Restrict);

		modelBuilder.Entity<Account>()
			.Property(x => x.Balance)
			.HasPrecision(18, 2);

		modelBuilder.Entity<Transaction>()
			.Property(x => x.Amount)
			.HasPrecision(18, 2);
	}
}