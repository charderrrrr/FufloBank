using System;
using Spectre.Console;
using App.Models.Enums;
using App.UI.Services;

namespace App.UI.Pages
{
    public class ConversionPage
    {
        private readonly SessionManager _sessionManager;

        public ConversionPage(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Конвертация валюты[/]");

            var fromCurrency = AnsiConsole.Prompt(
                new SelectionPrompt<CurrencyType>()
                    .Title("From currency:")
                    .AddChoices(CurrencyType.RUB, CurrencyType.USD, CurrencyType.CRYPTO));

            var toCurrency = AnsiConsole.Prompt(
                new SelectionPrompt<CurrencyType>()
                    .Title("To currency:")
                    .AddChoices(CurrencyType.RUB, CurrencyType.USD, CurrencyType.CRYPTO));

            var amount = AnsiConsole.Ask<decimal>("Enter amount:");

            try
            {
                var convertedAmount = _sessionManager.BankModule.CurrencyConversionService.Convert(
                    amount, fromCurrency, toCurrency);

                AnsiConsole.MarkupLine($"{amount:N2} {fromCurrency} = {convertedAmount:N6} {toCurrency}");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]{ex.Message}[/]");
            }

            AnsiConsole.Confirm("Нажмите любую клавишу для продолжения...");
        }
    }
}