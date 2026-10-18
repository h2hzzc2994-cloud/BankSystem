namespace WebApplication1.Application.DTOs.Transactions;

public class TransactionResponse
{
	public int Id { get; set; }

	public decimal Amount { get; set; }

	public string Type { get; set; } = string.Empty;

	public string FromIBAN { get; set; } = string.Empty;

	public string ToIBAN { get; set; } = string.Empty;

	public DateTime CreatedAt { get; set; }

	public string Description { get; set; } = string.Empty;
}