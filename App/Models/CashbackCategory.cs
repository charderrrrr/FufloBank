using App.Models.Enums;

namespace App.Models
{
    public class CashbackCategory
    {
        public int Id { get; set; }
        public TransactionCategory Category { get; set; }
        public decimal Percentage { get; set; }
        public bool IsActive { get; set; }
    }
}