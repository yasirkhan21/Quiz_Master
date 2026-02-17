using QuizMaster.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuizMaster.Core.Interfaces
{
    public interface IOpenTriviaService
    {
        Task<List<OpenTriviaQuestion>> GetQuestionsAsync(
            int amount = 10,
            string? category = null,
            string? difficulty = null,
            string? type = null);

        Task<List<string>> GetCategoriesAsync();

        int? GetCategoryId(string categoryName);
    }
}