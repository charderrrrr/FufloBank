using System;
using Spectre.Console;
using App.UI.Services;

namespace App.UI.Pages
{
    public class AuthPage
    {
        private readonly SessionManager _sessionManager;

        public AuthPage(SessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void Show()
        {
            AnsiConsole.Clear();
            
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold yellow]Добро пожаловать в FufloBank :)[/]")
                    .AddChoices("Войти", "Регистрация", "Выход"));

            switch (choice)
            {
                case "Войти":
                    Login();
                    break;
                case "Регистрация":
                    Register();
                    break;
                case "Выход":
                    Environment.Exit(0);
                    break;
            }
        }

        private void Login()
        {
            var phone = AnsiConsole.Ask<string>("Введите телефон (+7XXXXXXXXXX):");
            
            if (_sessionManager.Login(phone))
            {
                AnsiConsole.MarkupLine("[green]Авторизация успешна! :)[/]");
                AnsiConsole.Confirm("Нажмите любую клавишу для продолжения...");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Пользователь не найден :([/]");
                AnsiConsole.Confirm("Нажмите любую клавишу для продолжения...");
            }
        }

        private void Register()
        {
            var fullName = AnsiConsole.Ask<string>("Введите полное имя:");
            var phone = AnsiConsole.Ask<string>("Введите телефон (+7XXXXXXXXXX):");

            if (_sessionManager.Register(fullName, phone))
            {
                AnsiConsole.MarkupLine("[green]Регистрация успешна! :)[/]");
                AnsiConsole.Confirm("Нажмите любую клавишу для продолжения...");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Регистрация не пройдена.. :([/]");
                AnsiConsole.Confirm("Нажмите любую клавишу для продолжения...");
            }
        }
    }
}