namespace WebApplication1.Application.DTOs.Accounts;

public class AccountResponse
{
	public int Id { get; set; }

	public string IBAN { get; set; } = string.Empty;

	public string Currency { get; set; } = string.Empty;

	public decimal Balance { get; set; }

	public bool IsBlocked { get; set; }
}