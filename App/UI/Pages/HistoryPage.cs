using System;
using System.Linq;
using Spectre.Console;
using App.Models.Enums;
using App.UI.Services;

namespace App.UI.Pages
{
    public class HistoryPage
    {
        private readonly SessionManager _sessionManager;

        public HistoryPage(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void Show()
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold yellow]Итория транзакций :)[/]");

            var fromDate = AnsiConsole.Ask<DateTime>("С (yyyy-MM-dd):");
            var toDate = AnsiConsole.Ask<DateTime>("По (yyyy-MM-dd):");

            var filterChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Фильтровать:")
                    .AddChoices("All", "Входящие", "Исходящие", "По категории", "По объёму"));

            bool? isIncoming = null;
            int? category = null;
            decimal? minAmount = null;
            decimal? maxAmount = null;

            switch (filterChoice)
            {
                case "Входящие":
                    isIncoming = true;
                    break;
                case "Исходящие":
                    isIncoming = false;
                    break;
                case "По категории":
                    category = AnsiConsole.Prompt(
                        new SelectionPrompt<int>()
                            .Title("Выбрать категорию:")
                            .AddChoices(1, 2, 3, 4, 5));
                    break;
                case "По объёму":
                    minAmount = AnsiConsole.Ask<decimal?>("Минимальное количество (или оставьте пустым):", null);
                    maxAmount = AnsiConsole.Ask<decimal?>("Максимальное количество (или оставьте пустым):", null);
                    break;
            }

            var transactions = _sessionManager.BankModule.StatementService.GetStatement(
                _sessionManager.CurrentUser.Id, fromDate, toDate, minAmount, maxAmount, category, isIncoming);

            var table = new Table();
            table.AddColumn("Дата");
            table.AddColumn("Тип");
            table.AddColumn("Сумма");
            table.AddColumn("Валюта");
            table.AddColumn("Категории");
            table.AddColumn("Статус");
            table.AddColumn("Кэшбек");

            foreach (var transaction in transactions)
            {
                table.AddRow(
                    transaction.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                    transaction.FromUserId == _sessionManager.CurrentUser.Id ? "Outgoing" : "Incoming",
                    transaction.Amount.ToString("N2"),
                    transaction.Currency.ToString(),
                    transaction.Category.ToString(),
                    transaction.Status.ToString(),
                    transaction.CashbackAmount?.ToString("N2") ?? "-");
            }

            AnsiConsole.Write(table);
            AnsiConsole.Confirm("Нажмите любую клавишу для продолжения...");
        }
    }
}