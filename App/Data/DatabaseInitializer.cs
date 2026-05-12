using System.Data;
using Dapper;
using Npgsql;

namespace App.Data
{
    public class DatabaseInitializer
    {
        private readonly string _connectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            return connection;
        }

        public void Initialize()
        {
            using var connection = CreateConnection();
            connection.Open();

            var sql = @"
                CREATE TABLE IF NOT EXISTS users (
                    id UUID PRIMARY KEY,
                    full_name VARCHAR(200) NOT NULL,
                    phone VARCHAR(12) UNIQUE NOT NULL,
                    status INT NOT NULL DEFAULT 0,
                    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
                    monthly_cashback_limit DECIMAL(18,2) NOT NULL DEFAULT 5000.00,
                    current_monthly_cashback DECIMAL(18,2) NOT NULL DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS accounts (
                    id UUID PRIMARY KEY,
                    user_id UUID NOT NULL REFERENCES users(id),
                    currency INT NOT NULL,
                    balance DECIMAL(18,2) NOT NULL DEFAULT 0,
                    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
                    is_active BOOLEAN NOT NULL DEFAULT TRUE
                );

                CREATE TABLE IF NOT EXISTS exchange_rates (
                    id SERIAL PRIMARY KEY,
                    from_currency INT NOT NULL,
                    to_currency INT NOT NULL,
                    rate DECIMAL(18,6) NOT NULL,
                    updated_at TIMESTAMP NOT NULL DEFAULT NOW(),
                    CONSTRAINT uq_exchange_rates_from_to UNIQUE (from_currency, to_currency)
                );

                CREATE TABLE IF NOT EXISTS mcc_codes (
                    code INT PRIMARY KEY,
                    category INT NOT NULL,
                    description VARCHAR(200) NOT NULL
                );

                CREATE TABLE IF NOT EXISTS cashback_categories (
                    id SERIAL PRIMARY KEY,
                    category INT NOT NULL UNIQUE,
                    percentage DECIMAL(5,2) NOT NULL,
                    is_active BOOLEAN NOT NULL DEFAULT TRUE
                );

                CREATE TABLE IF NOT EXISTS transactions (
                    id UUID PRIMARY KEY,
                    from_account_id UUID REFERENCES accounts(id),
                    to_account_id UUID REFERENCES accounts(id),
                    from_user_id UUID REFERENCES users(id),
                    to_user_id UUID REFERENCES users(id),
                    amount DECIMAL(18,2) NOT NULL,
                    currency INT NOT NULL,
                    converted_amount DECIMAL(18,2),
                    category INT NOT NULL,
                    status INT NOT NULL DEFAULT 1,
                    created_at TIMESTAMP NOT NULL DEFAULT NOW(),
                    completed_at TIMESTAMP,
                    description VARCHAR(500),
                    cashback_amount DECIMAL(18,2),
                    mcc_code INT
                );

                CREATE INDEX IF NOT EXISTS idx_accounts_user_id ON accounts(user_id);
                CREATE INDEX IF NOT EXISTS idx_transactions_from_user ON transactions(from_user_id);
                CREATE INDEX IF NOT EXISTS idx_transactions_to_user ON transactions(to_user_id);
                CREATE INDEX IF NOT EXISTS idx_transactions_created ON transactions(created_at);

                INSERT INTO cashback_categories (category, percentage, is_active) VALUES 
                    (1, 5.00, TRUE),
                    (2, 3.00, TRUE),
                    (3, 1.00, TRUE)
                ON CONFLICT (category) DO NOTHING;

                INSERT INTO mcc_codes (code, category, description) VALUES
                    (5812, 1, 'Restaurant'),
                    (5813, 1, 'Bar'),
                    (5814, 1, 'Fast Food'),
                    (4121, 2, 'Taxi'),
                    (4111, 2, 'Transportation'),
                    (5999, 3, 'Other Retail')
                ON CONFLICT (code) DO NOTHING;
            ";

            connection.Execute(sql);
        }
    }
}