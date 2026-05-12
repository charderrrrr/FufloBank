namespace App.Models.Enums
{
    public enum TransactionStatus
    {
        Pending = 1,
        Completed = 2,
        Failed = 3,
        Reversed = 4,
        PendingConfirmation = 5
    }
}