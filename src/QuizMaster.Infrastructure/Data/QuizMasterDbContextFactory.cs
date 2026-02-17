using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace QuizMaster.Infrastructure.Data
{
    public class QuizMasterDbContextFactory : IDesignTimeDbContextFactory<QuizMasterDbContext>
    {
        public QuizMasterDbContext CreateDbContext(string[] args)
        {
            // Get the path to the API project where appsettings.json is located
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "QuizMaster.API");
            
            // If running from solution root, adjust the path
            if (!Directory.Exists(basePath))
            {
                basePath = Path.Combine(Directory.GetCurrentDirectory(), "src", "QuizMaster.API");
            }

            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Create DbContextOptionsBuilder
            var optionsBuilder = new DbContextOptionsBuilder<QuizMasterDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString, 
                sqlOptions => sqlOptions.MigrationsAssembly("QuizMaster.Infrastructure"));

            return new QuizMasterDbContext(optionsBuilder.Options);
        }
    }
}