using App.Models.Enums;

namespace App.Models
{
    public class MccCode
    {
        public int Code { get; set; }
        public TransactionCategory Category { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}