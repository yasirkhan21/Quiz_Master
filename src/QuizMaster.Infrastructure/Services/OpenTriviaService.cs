using QuizMaster.Core.DTOs;
using QuizMaster.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace QuizMaster.Infrastructure.Services
{
    public class OpenTriviaService : IOpenTriviaService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://opentdb.com/api.php";

        // Complete Open Trivia DB Category Map
        private static readonly Dictionary<string, int> CategoryMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { "General Knowledge",              9  },
            { "Books",                          10 },
            { "Film",                           11 },
            { "Music",                          12 },
            { "Musicals & Theatres",            13 },
            { "Television",                     14 },
            { "Video Games",                    15 },
            { "Board Games",                    16 },
            { "Science & Nature",               17 },
            { "Computers",                      18 },
            { "Mathematics",                    19 },
            { "Mythology",                      20 },
            { "Sports",                         21 },
            { "Geography",                      22 },
            { "History",                        23 },
            { "Politics",                       24 },
            { "Art",                            25 },
            { "Celebrities",                    26 },
            { "Animals",                        27 },
            { "Vehicles",                       28 },
            { "Comics",                         29 },
            { "Gadgets",                        30 },
            { "Anime & Manga",                  31 },
            { "Cartoon & Animations",           32 }
        };

        public OpenTriviaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OpenTriviaQuestion>> GetQuestionsAsync(
            int amount = 10,
            string? category = null,
            string? difficulty = null,
            string? type = null)
        {
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["amount"] = amount.ToString();

            // Convert category name to numeric ID
            if (!string.IsNullOrEmpty(category))
            {
                if (CategoryMap.TryGetValue(category, out int categoryId))
                {
                    queryParams["category"] = categoryId.ToString();
                }
                else if (int.TryParse(category, out int numericId))
                {
                    // Already a numeric ID passed directly
                    queryParams["category"] = numericId.ToString();
                }
                // If category not found in map, omit it (returns all categories)
            }

            if (!string.IsNullOrEmpty(difficulty))
                queryParams["difficulty"] = difficulty.ToLower();

            if (!string.IsNullOrEmpty(type))
                queryParams["type"] = type.ToLower();

            var url = $"{BaseUrl}?{queryParams}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var triviaResponse = JsonSerializer.Deserialize<OpenTriviaResponse>(content);

                if (triviaResponse?.ResponseCode == 0 && triviaResponse.Results != null)
                {
                    foreach (var question in triviaResponse.Results)
                    {
                        question.Question = HttpUtility.HtmlDecode(question.Question);
                        question.CorrectAnswer = HttpUtility.HtmlDecode(question.CorrectAnswer);

                        if (question.IncorrectAnswers != null)
                        {
                            for (int i = 0; i < question.IncorrectAnswers.Count; i++)
                            {
                                question.IncorrectAnswers[i] =
                                    HttpUtility.HtmlDecode(question.IncorrectAnswers[i]);
                            }
                        }
                    }

                    return triviaResponse.Results;
                }

                // Handle Open Trivia DB response codes
                var errorMessage = triviaResponse?.ResponseCode switch
                {
                    1 => "No results found for the given parameters",
                    2 => "Invalid parameter passed to Open Trivia DB",
                    3 => "Token not found",
                    4 => "Token empty - all questions exhausted",
                    _ => "Unknown error from Open Trivia DB"
                };

                throw new Exception(errorMessage);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch questions: {ex.Message}", ex);
            }
        }

        public Task<List<string>> GetCategoriesAsync()
        {
            // Return all available category names
            var categories = new List<string>(CategoryMap.Keys);
            return Task.FromResult(categories);
        }

        public int? GetCategoryId(string categoryName)
        {
            return CategoryMap.TryGetValue(categoryName, out int id) ? id : null;
        }
    }
}