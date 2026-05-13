// Репозиторий для работы с таблицей mcc_codes
// GetByCode - определение категории транзакции по коду мерчанта для автоначисления кэшбэка

using System.Data;
using Dapper;
using App.Models;

namespace App.Data.Repositories
{
    public class MccCodeRepository
    {
        private readonly IDbConnection _connection;

        public MccCodeRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public MccCode? GetByCode(int code)
        {
            return _connection.QuerySingleOrDefault<MccCode>(
                "SELECT * FROM mcc_codes WHERE code = @Code", new { Code = code });
        }
    }
}
