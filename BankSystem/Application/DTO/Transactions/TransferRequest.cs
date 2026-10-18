namespace WebApplication1.Application.DTOs.Transactions;

public class TransferRequest
{
	public string FromIBAN { get; set; } = string.Empty;

	public string ToIBAN { get; set; } = string.Empty;

	public decimal Amount { get; set; }

	public string Description { get; set; } = string.Empty;
}