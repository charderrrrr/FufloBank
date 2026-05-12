using System;
using Spectre.Console;
using App.UI.Services;

namespace App.UI.Pages
{
    public class MainMenuPage
    {
        private readonly SessionManager _sessionManager;
        private readonly BalancePage _balancePage;
        private readonly TransferPage _transferPage;
        private readonly PaymentPage _paymentPage;
        private readonly HistoryPage _historyPage;
        private readonly ConversionPage _conversionPage;

        public MainMenuPage(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _balancePage = new BalancePage(sessionManager);
            _transferPage = new TransferPage(sessionManager);
            _paymentPage = new PaymentPage(sessionManager);
            _historyPage = new HistoryPage(sessionManager);
            _conversionPage = new ConversionPage(sessionManager);
        }

        public void Show()
        {
            while (_sessionManager.IsAuthenticated)
            {
                AnsiConsole.Clear();
                AnsiConsole.MarkupLine($"[bold yellow]Welcome, {_sessionManager.CurrentUser.FullName}[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold]Главное меню :)[/]")
                        .AddChoices("Баланс", "Оплатить", "P2P перевод", "Конвертация валюты", "История транзакций", "Выйти"));

                switch (choice)
                {
                    case "Баланс":
                        _balancePage.Show();
                        break;
                    case "Оплатить":
                        _paymentPage.Show();
                        break;
                    case "P2P перевод":
                        _transferPage.Show();
                        break;
                    case "Конвертация валюты":
                        _conversionPage.Show();
                        break;
                    case "История транзакций":
                        _historyPage.Show();
                        break;
                    case "Выйти":
                        _sessionManager.Logout();
                        break;
                }
            }
        }
    }
}