using System.Transactions;

namespace GreentubePatformBackend.Models
{
    public class PlayerWallet
    {
        private readonly object _lock = new object();
        public Guid playerId { get; set; }
        public decimal Balance { get; set; }
        public List<Transaction> Transactions { get; set; }


        public bool AddTransaction(Transaction transaction)
        {
            lock (_lock)
            {
                //Check for negative amount
                if (transaction.Amount < 0)
                {
                    transaction.State = TransactionState.Rejected;
                    return false;
                }

                //Add transaction to list
                Transactions.Add(transaction);

                //Update balance and transaction state
                switch (transaction.Type)
                {
                    case TransactionType.Deposit:
                        Balance += transaction.Amount;
                        break;
                    case TransactionType.Stake:
                        if (Balance < transaction.Amount)
                        {
                            transaction.State = TransactionState.Rejected;
                            return false;
                        }
                        Balance -= transaction.Amount;
                        break;
                    case TransactionType.Win:
                        Balance += transaction.Amount;
                        break;
                    default:
                        return false;
                }
                transaction.State = TransactionState.Accepted;
                return true;
            }
        }

    }
}
