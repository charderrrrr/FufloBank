using System;
using System.Collections.Generic;
using App.Data.Repositories;
using App.Models;

namespace App.Services
{
    public class StatementService
    {
        private readonly TransactionRepository _transactionRepository;

        public StatementService(TransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public IEnumerable<Transaction> GetStatement(
            Guid userId, 
            DateTime from, 
            DateTime to,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            int? category = null,
            bool? isIncoming = null)
        {
            return _transactionRepository.GetStatement(userId, from, to, minAmount, maxAmount, category, isIncoming);
        }
    }
}