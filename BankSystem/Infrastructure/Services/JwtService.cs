using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BankSystem.Application.Interfaces;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Application.Interfaces;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Infrastructure.Services;

public class JwtService : IJwtService
{
	private readonly IConfiguration _configuration;

	public JwtService(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	public string GenerateAccessToken(User user)
	{
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Role, user.Role)
		};

		var key = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(
				_configuration["Jwt:Key"]!));

		var credentials = new SigningCredentials(
			key,
			SecurityAlgorithms.HmacSha256);

		var minutes = int.Parse(
			_configuration["Jwt:AccessTokenMinutes"] ?? "15");

		var token = new JwtSecurityToken(
			issuer: _configuration["Jwt:Issuer"],
			audience: _configuration["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(minutes),
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler()
			.WriteToken(token);
	}

	public string GenerateRefreshToken()
	{
		var bytes = RandomNumberGenerator.GetBytes(64);

		return Convert.ToBase64String(bytes);
	}
}