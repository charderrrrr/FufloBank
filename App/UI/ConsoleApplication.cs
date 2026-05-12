using System;
using Spectre.Console;
using App.UI.Pages;
using App.UI.Services;

namespace App.UI
{
    public class ConsoleApplication
    {
        private readonly string _connectionString;
        private SessionManager _sessionManager;

        public ConsoleApplication(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Run()
        {
            _sessionManager = new SessionManager(_connectionString);

            while (true)
            {
                if (!_sessionManager.IsAuthenticated)
                {
                    var authPage = new AuthPage(_sessionManager);
                    authPage.Show();
                }
                else
                {
                    var mainMenuPage = new MainMenuPage(_sessionManager);
                    mainMenuPage.Show();
                }
            }
        }
    }
}