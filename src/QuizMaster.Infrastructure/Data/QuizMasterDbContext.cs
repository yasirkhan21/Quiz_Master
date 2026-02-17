using Microsoft.EntityFrameworkCore;
using QuizMaster.Core.Entities;
using QuizMaster.Core.Interfaces;

namespace QuizMaster.Infrastructure.Data
{
    public class QuizMasterDbContext : DbContext, IQuizMasterDbContext
    {
        public QuizMasterDbContext(DbContextOptions<QuizMasterDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<UserWeaknessProfile> UserWeaknessProfiles { get; set; }
        public DbSet<ScoringRule> ScoringRules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.HasKey(e => e.QuizId);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Difficulty).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Accuracy).HasPrecision(5, 2);
                entity.Property(e => e.AverageTimePerQuestion).HasPrecision(8, 2);
                entity.Property(e => e.Timestamp).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Quizzes)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.Timestamp);
            });

           modelBuilder.Entity<Question>(entity =>
    { 
    entity.HasKey(e => e.QuestionId);
    entity.Property(e => e.QuestionText).IsRequired();
    entity.Property(e => e.CorrectAnswer).IsRequired();
    entity.Property(e => e.UserAnswer).IsRequired(false);
    entity.Property(e => e.IncorrectAnswersJson).IsRequired(false);
    entity.Property(e => e.TagsJson).IsRequired(false);
    entity.Property(e => e.TimeSpent).HasPrecision(8, 2);
    entity.Property(e => e.Category).HasMaxLength(50);
    entity.Property(e => e.Difficulty).HasMaxLength(20);
    entity.Property(e => e.Type).HasMaxLength(20);
    entity.Property(e => e.IsCorrect).HasDefaultValue(false);
    entity.Property(e => e.TimeSpent).HasDefaultValue(0m);
    entity.Ignore(e => e.Tags); // Handled via TagsJson

    entity.HasOne(e => e.Quiz)
        .WithMany(q => q.Questions)
        .HasForeignKey(e => e.QuizId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasIndex(e => e.QuizId);
});
            modelBuilder.Entity<UserWeaknessProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Topic).IsRequired().HasMaxLength(100);
                entity.Property(e => e.WeightScore).HasPrecision(10, 4);
                entity.Property(e => e.LastUpdated).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.User)
                    .WithMany(u => u.WeaknessProfiles)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.Topic }).IsUnique();
            });

            modelBuilder.Entity<ScoringRule>(entity =>
            {
                entity.HasKey(e => e.RuleId);
                entity.Property(e => e.RuleName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ConditionsJson).IsRequired();
                entity.Property(e => e.ActionsJson).IsRequired();
                entity.Property(e => e.Scope).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasIndex(e => e.IsActive);
                entity.HasIndex(e => e.Scope);
            });
        }
    }
}