using System;
using System.Collections.Generic;

namespace QuizMaster.Core.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<Quiz> Quizzes { get; set; }
        public virtual ICollection<UserWeaknessProfile> WeaknessProfiles { get; set; }
        
        public User()
        {
            Quizzes = new HashSet<Quiz>();
            WeaknessProfiles = new HashSet<UserWeaknessProfile>();
        }
    }
}