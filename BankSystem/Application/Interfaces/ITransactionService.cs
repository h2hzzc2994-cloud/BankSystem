using WebApplication1.Application.DTOs.Transactions;

namespace WebApplication1.Application.Interfaces;

public interface ITransactionService
{
	Task TransferAsync(
		int userId,
		TransferRequest request);

	Task<List<TransactionResponse>> GetTransactionsAsync(
		int userId,
		TransactionFilterRequest filter);
}