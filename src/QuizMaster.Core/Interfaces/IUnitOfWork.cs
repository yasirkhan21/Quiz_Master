using QuizMaster.Core.Entities;
using System;
using System.Threading.Tasks;

namespace QuizMaster.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Quiz> Quizzes { get; }
        IRepository<Question> Questions { get; }
        IRepository<UserWeaknessProfile> WeaknessProfiles { get; }
        IRepository<ScoringRule> ScoringRules { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}