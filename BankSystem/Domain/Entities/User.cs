using Microsoft.Identity.Client.NativeInterop;

namespace WebApplication1.Domain.Entities;

public class User
{
	public int Id { get; set; }

	public string FirstName { get; set; } = string.Empty;

	public string LastName { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public string PasswordHash { get; set; } = string.Empty;

	public string Role { get; set; } = "User";

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	public ICollection<Account> Accounts { get; set; } = new List<Account>();

	public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}