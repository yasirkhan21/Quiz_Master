using QuizMaster.Core.Entities;

namespace QuizMaster.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        Guid? ValidateToken(string token);
    }
}