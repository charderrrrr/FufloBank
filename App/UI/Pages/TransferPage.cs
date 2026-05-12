using System;
using Spectre.Console;
using App.Models.Enums;
using App.UI.Services;

namespace App.UI.Pages
{
    public class TransferPage
    {
        private readonly SessionManager _sessionManager;

        public TransferPage(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]P2P Transfer[/]");

            var phone = AnsiConsole.Ask<string>("Входит телефон получателя (+7XXXXXXXXXX):");
            var currency = AnsiConsole.Prompt(
                new SelectionPrompt<CurrencyType>()
                    .Title("Select currency:")
                    .AddChoices(CurrencyType.RUB, CurrencyType.USD, CurrencyType.CRYPTO));

            var amount = AnsiConsole.Ask<decimal>("Enter amount:");

            string confirmationCode = null;
            if (_sessionManager.BankModule.SecurityService.RequiresConfirmation(amount))
            {
                confirmationCode = AnsiConsole.Ask<string>("Введите код подтверждения:");
            }

            try
            {
                var transaction = _sessionManager.BankModule.P2PTransferService.InitiateTransfer(
                    _sessionManager.CurrentUser.Id, phone, amount, currency, confirmationCode);

                AnsiConsole.MarkupLine("[green]Перевод успешен! :)[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
            }

            AnsiConsole.Confirm("Нажмите любую клавишу для продолжения...");
        }
    }
}