using System;
using Spectre.Console;
using App.Models.Enums;
using App.UI.Services;

namespace App.UI.Pages
{
    public class BalancePage
    {
        private readonly SessionManager _sessionManager;

        public BalancePage(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Баланс аккаунта :0[/]");

            var table = new Table();
            table.AddColumn("Валюта");
            table.AddColumn("Баланс");

            foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
            {
                var account = _sessionManager.BankModule.AccountRepository.GetByUserIdAndCurrency(
                    _sessionManager.CurrentUser.Id, currency);
                
                if (account != null)
                {
                    table.AddRow(currency.ToString(), account.Balance.ToString("N2"));
                }
            }

            AnsiConsole.Write(table);
            AnsiConsole.Confirm("Нажмите любую клавишу для продолжения...");
        }
    }
}
