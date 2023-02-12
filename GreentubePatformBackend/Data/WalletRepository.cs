using GreentubePatformBackend.Models;
using System.Transactions;
using Transaction = GreentubePatformBackend.Models.Transaction;

namespace GreentubePatformBackend.Data
{
    public interface IPlayerWalletRepository
    {
        Task<PlayerWallet> GetPlayerWalletAsync(Guid Id);
        Task<bool> AddPlayerWalletAsync(PlayerWallet playerWallet);
        Task<bool> AddTransactionAsync(Guid Id, Transaction transaction);
        Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid Id, IEnumerable<TransactionType> types = null);
    }

    public class InMemoryPlayerWalletRepository : IPlayerWalletRepository
    {
        private readonly Dictionary<Guid, PlayerWallet> _playerWallets = new Dictionary<Guid, PlayerWallet>();

        public Task<PlayerWallet> GetPlayerWalletAsync(Guid Id)
        {
            if (_playerWallets.TryGetValue(Id, out var playerWallet))
            {
                return Task.FromResult(playerWallet);
            }

            return Task.FromResult<PlayerWallet>(null);
        }

        public Task<bool> AddPlayerWalletAsync(PlayerWallet playerWallet)
        {
            if (_playerWallets.ContainsKey(playerWallet.playerId))
            {
                return Task.FromResult(false);
            }

            _playerWallets.Add(playerWallet.playerId, playerWallet);
            return Task.FromResult(true);
        }

        public Task<bool> AddTransactionAsync(Guid Id, Transaction transaction)
        {
            if (!_playerWallets.TryGetValue(Id, out var playerWallet))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(playerWallet.AddTransaction(transaction));
        }

        public Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid Id, IEnumerable<TransactionType> types = null)
        {
            if (!_playerWallets.TryGetValue(Id, out var playerWallet))
            {
                return Task.FromResult(Enumerable.Empty<Transaction>());
            }

            if (types == null)
            {
                return Task.FromResult(playerWallet.Transactions.AsEnumerable());
            }

            return Task.FromResult(playerWallet.Transactions.Where(t => types.Contains(t.Type)));
        }
    }
}
