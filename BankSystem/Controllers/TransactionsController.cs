using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Application.DTOs.Transactions;
using WebApplication1.Application.Interfaces;

namespace WebApplication1.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
	private readonly ITransactionService _transactionService;

	public TransactionsController(
		ITransactionService transactionService)
	{
		_transactionService = transactionService;
	}

	[HttpPost("transfer")]
	public async Task<IActionResult> Transfer(
		TransferRequest request)
	{
		var userId = GetUserId();

		await _transactionService.TransferAsync(
			userId,
			request);

		return Ok(new
		{
			message = "Transfer completed successfully."
		});
	}

	[HttpGet]
	public async Task<IActionResult> GetTransactions(
		[FromQuery] TransactionFilterRequest filter)
	{
		var userId = GetUserId();

		var transactions =
			await _transactionService.GetTransactionsAsync(
				userId,
				filter);

		return Ok(transactions);
	}

	private int GetUserId()
	{
		var value =
			User.FindFirstValue(
				ClaimTypes.NameIdentifier);

		if (!int.TryParse(value, out var userId))
			throw new UnauthorizedAccessException();

		return userId;
	}
}