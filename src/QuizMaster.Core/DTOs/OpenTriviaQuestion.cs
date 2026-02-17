using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QuizMaster.Core.DTOs
{
    public class OpenTriviaResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }
        
        [JsonPropertyName("results")]
        public List<OpenTriviaQuestion> Results { get; set; }
    }

    public class OpenTriviaQuestion
    {
        [JsonPropertyName("category")]
        public string Category { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; }
        
        [JsonPropertyName("question")]
        public string Question { get; set; }
        
        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; set; }
        
        [JsonPropertyName("incorrect_answers")]
        public List<string> IncorrectAnswers { get; set; }
    }
}