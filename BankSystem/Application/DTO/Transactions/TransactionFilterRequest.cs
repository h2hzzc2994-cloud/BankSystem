namespace WebApplication1.Application.DTOs.Transactions;

public class TransactionFilterRequest
{
	public DateTime? FromDate { get; set; }

	public DateTime? ToDate { get; set; }

	public string? Type { get; set; }

	public decimal? MinAmount { get; set; }

	public decimal? MaxAmount { get; set; }

	public string? IBAN { get; set; }
}