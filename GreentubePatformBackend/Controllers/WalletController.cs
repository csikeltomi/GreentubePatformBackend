using GreentubePatformBackend.Data;
using GreentubePatformBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GreentubePatformBackend.Controllers
{
    [Route("wallet")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        //initialize player wallet singleton
        private readonly IPlayerWalletRepository _playerWalletRepository;

        public WalletController(IPlayerWalletRepository playerWalletRepository)
        {
            _playerWalletRepository = playerWalletRepository;
        }

        [HttpPut("registerWallet/{playerId}")]
        public async Task<IActionResult> RegisterWallet(Guid playerId)
        {
            var playerWallet = new PlayerWallet
            {
                playerId = playerId,
                Balance = 0,
                Transactions = new List<Transaction>()
            };

            var result = await _playerWalletRepository.AddPlayerWalletAsync(playerWallet);

            if (!result)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error registering wallet: Player wallet already exists");
            }

            return Ok();
        }

        [HttpGet("getBalance/{playerId}")]
        public async Task<IActionResult> GetBalance(Guid playerId)
        {
            var playerWallet = await _playerWalletRepository.GetPlayerWalletAsync(playerId);

            if (playerWallet == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error getting balance: Player wallet does not exist");
            }

            return Ok(playerWallet.Balance);
        }

        //idempotent deposit function
        [HttpPut("deposit/{playerId}/{amount}")]
        public async Task<IActionResult> Deposit(Guid playerId, decimal amount)
        {
            var playerWallet = await _playerWalletRepository.GetPlayerWalletAsync(playerId);

            if (playerWallet == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error depositing: Player wallet does not exist");
            }

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Type = TransactionType.Deposit,
                State = TransactionState.Pending
            };

            var result = await _playerWalletRepository.AddTransactionAsync(playerId, transaction);

            return result ? Ok() : BadRequest();
        }

        //idempotent stake function
        [HttpPut("stake/{playerId}/{amount}")]
        public async Task<IActionResult> Stake(Guid playerId, decimal amount)
        {
            var playerWallet = await _playerWalletRepository.GetPlayerWalletAsync(playerId);

            if (playerWallet == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error depositing: Player wallet does not exist");
            }

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Type = TransactionType.Stake,
                State = TransactionState.Pending
            };

            var result = await _playerWalletRepository.AddTransactionAsync(playerId, transaction);

            return result ? Ok() : BadRequest("Insufficient funds");
        }

        //idempotent win function
        [HttpPut("win/{playerId}/{amount}")]
        public async Task<IActionResult> Win(Guid playerId, decimal amount)
        {
            var playerWallet = await _playerWalletRepository.GetPlayerWalletAsync(playerId);

            if (playerWallet == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error depositing: Player wallet does not exist");
            }

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Type = TransactionType.Win,
                State = TransactionState.Pending
            };

            var result = await _playerWalletRepository.AddTransactionAsync(playerId, transaction);

            return result ? Ok() : BadRequest();
        }

        //get transactions with filter of type supporting multiple types
        [HttpGet("getTransactions/{playerId}/{s_transactionTypes}")]
        public async Task<IActionResult> GetTransactions(Guid playerId, string s_transactionTypes)
        {
            var transactionTypes = s_transactionTypes.Split(',').Select(t => (TransactionType)Enum.Parse(typeof(TransactionType), t)).ToArray();
            var transactions = await _playerWalletRepository.GetTransactionsAsync(playerId,transactionTypes);
            if (transactionTypes != null && transactionTypes.Length > 0)
            {
                transactions = transactions.Where(t => transactionTypes.Contains(t.Type));
            }

            return Ok(transactions);
        }
    }
}
