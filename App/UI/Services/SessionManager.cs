// SessionManager - управляет пользовательской сессией: вход, регистрация, выход, хранение текущего пользователя.
// Инициализирует базу данных и подключает банковский модуль. Реализует IDisposable для освобождения соединения с БД.
using System;
using System.Data;
using App.Services;
using App.Models;
using App.Data.Repositories;
using App.Data;

namespace App.UI.Services
{
    public class SessionManager : IDisposable
    {
        private readonly DatabaseInitializer _initializer;
        private IDbConnection _connection;
        private readonly PasswordService _passwordService;
        
        public FufloBankModule BankModule { get; private set; }
        public UserProfile? CurrentUser { get; set; }
        public bool IsAuthenticated => CurrentUser != null;

        public SessionManager(string connectionString)
        {
            _initializer = new DatabaseInitializer(connectionString);
            _initializer.Initialize();
            _connection = _initializer.CreateConnection();
            _connection.Open();
            
            _passwordService = new PasswordService();
            BankModule = new FufloBankModule(_connection);
            BankModule.RateUpdaterService.UpdateRates("rates.json");
        }

        public bool Login(string phone, string password)
        {
            var user = BankModule.UserRepository.GetByPhone(phone);
            if (user == null)
                return false;

            if (!_passwordService.VerifyPassword(password, user.PasswordHash))
                return false;

            CurrentUser = user;
            return true;
        }

        public bool Register(string fullName, string phone, string password)
        {
            try
            {
                var passwordHash = _passwordService.HashPassword(password);
                CurrentUser = BankModule.RegisterUser(fullName, phone, passwordHash);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }
}