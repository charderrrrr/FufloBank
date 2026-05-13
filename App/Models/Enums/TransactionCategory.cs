// Категории для начисления кэшбэка
// Restaurants - рестораны (5%)
// Taxi - такси (3%)
// Other - остальные покупки (1%)
// Transfer - P2P переводы между пользователями
// Conversion - конвертация валют

namespace App.Models.Enums
{
    public enum TransactionCategory
    {
        Restaurants = 1,
        Taxi = 2,
        Other = 3,
        Transfer = 4,
        Conversion = 5
    }
}
