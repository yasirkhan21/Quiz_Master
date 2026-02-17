using Microsoft.EntityFrameworkCore;
using QuizMaster.Core.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace QuizMaster.Core.Interfaces
{
    public interface IQuizMasterDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Quiz> Quizzes { get; set; }
        DbSet<Question> Questions { get; set; }
        DbSet<UserWeaknessProfile> UserWeaknessProfiles { get; set; }
        DbSet<ScoringRule> ScoringRules { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}