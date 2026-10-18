using BankSystem.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Application.DTOs.Auth;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;
using WebApplication1.Infrastructure.Data;

namespace WebApplication1.Application.Services;

public class AuthService : IAuthService
{
	private readonly BankDbContext _context;
	private readonly IJwtService _jwtService;
	private readonly PasswordHasher<User> _passwordHasher;

	public AuthService(
		BankDbContext context,
		IJwtService jwtService)
	{
		_context = context;
		_jwtService = jwtService;
		_passwordHasher = new PasswordHasher<User>();
	}

	public async Task<LoginResponse> RegisterAsync(
		RegisterRequest request)
	{
		var email = request.Email.Trim().ToLower();

		var exists = await _context.Users
			.AnyAsync(x => x.Email == email);

		if (exists)
			throw new Exception("Email already exists.");

		var user = new User
		{
			FirstName = request.FirstName,
			LastName = request.LastName,
			Email = email,
			Role = "User"
		};

		user.PasswordHash =
			_passwordHasher.HashPassword(
				user,
				request.Password);

		_context.Users.Add(user);

		await _context.SaveChangesAsync();

		return await CreateTokensAsync(user);
	}

	public async Task<LoginResponse> LoginAsync(
		LoginRequest request)
	{
		var email = request.Email.Trim().ToLower();

		var user = await _context.Users
			.FirstOrDefaultAsync(x => x.Email == email);

		if (user == null)
			throw new Exception("Invalid email or password.");

		var result =
			_passwordHasher.VerifyHashedPassword(
				user,
				user.PasswordHash,
				request.Password);

		if (result == PasswordVerificationResult.Failed)
			throw new Exception("Invalid email or password.");

		return await CreateTokensAsync(user);
	}

	public async Task<LoginResponse> RefreshAsync(
		string refreshToken)
	{
		var token = await _context.RefreshTokens
			.Include(x => x.User)
			.FirstOrDefaultAsync(x =>
				x.Token == refreshToken &&
				!x.IsRevoked);

		if (token == null ||
			token.ExpiresAt <= DateTime.UtcNow)
		{
			throw new Exception("Invalid refresh token.");
		}

		token.IsRevoked = true;

		return await CreateTokensAsync(token.User);
	}

	private async Task<LoginResponse> CreateTokensAsync(User user)
	{
		var accessToken =
			_jwtService.GenerateAccessToken(user);

		var refreshToken =
			_jwtService.GenerateRefreshToken();

		var refreshTokenEntity = new RefreshToken
		{
			Token = refreshToken,
			UserId = user.Id,
			ExpiresAt = DateTime.UtcNow.AddDays(7)
		};

		_context.RefreshTokens.Add(refreshTokenEntity);

		await _context.SaveChangesAsync();

		return new LoginResponse
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken
		};
	}
}