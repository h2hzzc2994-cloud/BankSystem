namespace WebApplication1.Domain.Entities;

public class Transaction
{
	public int Id { get; set; }

	public decimal Amount { get; set; }

	public string Type { get; set; } = string.Empty;

	public int FromAccountId { get; set; }

	public int ToAccountId { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public string Description { get; set; } = string.Empty;

	public Account FromAccount { get; set; } = null!;

	public Account ToAccount { get; set; } = null!;
}