using System;

namespace QuizMaster.Core.Entities
{
    public class UserWeaknessProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Topic { get; set; }
        public decimal WeightScore { get; set; }
        public DateTime LastUpdated { get; set; }
        
        // Navigation property
        public virtual User User { get; set; }
    }
}