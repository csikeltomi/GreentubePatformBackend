namespace GreentubePatformBackend.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public TransactionState State { get; set; }
    }

    public enum TransactionType
    {
        Deposit,
        Stake,
        Win
    }

    public enum TransactionState
    {
        Accepted,
        Rejected,
        Pending
    }
}
