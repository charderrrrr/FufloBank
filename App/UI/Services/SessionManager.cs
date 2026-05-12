using System;
using App.Services;
using App.Models;
using App.Data.Repositories;
using App.Data;
using App.Validators;

namespace App.UI.Services
{
    public class SessionManager : IDisposable
    {
        private readonly DatabaseInitializer _initializer;
        private System.Data.IDbConnection _connection;
        
        public FufloBankModule BankModule { get; private set; }
        public UserProfile CurrentUser { get; set; }
        public bool IsAuthenticated => CurrentUser != null;

        public SessionManager(string connectionString)
        {
            _initializer = new DatabaseInitializer(connectionString);
            _initializer.Initialize();
            _connection = _initializer.CreateConnection();
            _connection.Open();
            
            BankModule = new FufloBankModule(_connection);
            BankModule.RateUpdaterService.UpdateRates("rates.json");
        }

        public bool Login(string phone)
        {
            var user = BankModule.UserRepository.GetByPhone(phone);
            if (user == null)
                return false;

            CurrentUser = user;
            return true;
        }

        public bool Register(string fullName, string phone)
        {
            try
            {
                CurrentUser = BankModule.RegisterUser(fullName, phone);
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