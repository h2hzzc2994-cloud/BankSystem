namespace WebApplication1.Domain.Entities;

public class Account
{
	public int Id { get; set; }

	public int UserId { get; set; }

	public string IBAN { get; set; } = string.Empty;

	public string Currency { get; set; } = "GEL";

	public decimal Balance { get; set; }

	public bool IsBlocked { get; set; }

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public User User { get; set; } = null!;

	public ICollection<Transaction> OutgoingTransactions { get; set; }
		= new List<Transaction>();

	public ICollection<Transaction> IncomingTransactions { get; set; }
		= new List<Transaction>();
}