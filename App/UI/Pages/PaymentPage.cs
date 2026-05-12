using System;
using Spectre.Console;
using App.Models.Enums;
using App.UI.Services;

namespace App.UI.Pages
{
    public class PaymentPage
    {
        private readonly SessionManager _sessionManager;

        public PaymentPage(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Перевод :)[/]");

            var currency = AnsiConsole.Prompt(
                new SelectionPrompt<CurrencyType>()
                    .Title("Select currency:")
                    .AddChoices(CurrencyType.RUB, CurrencyType.USD, CurrencyType.CRYPTO));

            var amount = AnsiConsole.Ask<decimal>("Введите сумму:");
            var mccCode = AnsiConsole.Ask<int>("Ввести MCC код:");
            var description = AnsiConsole.Ask<string>("Введите описание:");

            try
            {
                var transaction = _sessionManager.BankModule.TransactionService.ProcessPayment(
                    _sessionManager.CurrentUser.Id, amount, currency, mccCode, description);

                AnsiConsole.MarkupLine("[green]Payment successful[/]");
                
                if (transaction.CashbackAmount.HasValue && transaction.CashbackAmount.Value > 0)
                {
                    AnsiConsole.MarkupLine($"[green]Кэшбек: {transaction.CashbackAmount.Value:N2}[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
            }

            AnsiConsole.Confirm("Нажмите любую клавишу для продолжения...");
        }
    }
}