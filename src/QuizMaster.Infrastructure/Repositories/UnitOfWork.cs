using Microsoft.EntityFrameworkCore.Storage;
using QuizMaster.Core.Entities;
using QuizMaster.Core.Interfaces;
using QuizMaster.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace QuizMaster.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly QuizMasterDbContext _context;
        private IDbContextTransaction _transaction;

        public IRepository<User> Users { get; }
        public IRepository<Quiz> Quizzes { get; }
        public IRepository<Question> Questions { get; }
        public IRepository<UserWeaknessProfile> WeaknessProfiles { get; }
        public IRepository<ScoringRule> ScoringRules { get; }

        public UnitOfWork(QuizMasterDbContext context)
        {
            _context = context;
            Users = new Repository<User>(_context);
            Quizzes = new Repository<Quiz>(_context);
            Questions = new Repository<Question>(_context);
            WeaknessProfiles = new Repository<UserWeaknessProfile>(_context);
            ScoringRules = new Repository<ScoringRule>(_context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}