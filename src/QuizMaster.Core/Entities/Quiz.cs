using System;
using System.Collections.Generic;

namespace QuizMaster.Core.Entities
{
    public class Quiz
    {
        public Guid QuizId { get; set; }
        public Guid UserId { get; set; }
        public string Category { get; set; }
        public string Difficulty { get; set; }
        public int TotalScore { get; set; }
        public decimal Accuracy { get; set; }
        public decimal AverageTimePerQuestion { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsCompleted { get; set; }
        
        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        
        public Quiz()
        {
            Questions = new HashSet<Question>();
        }
    }
}