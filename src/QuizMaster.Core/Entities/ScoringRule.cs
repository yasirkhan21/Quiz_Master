using System;

namespace QuizMaster.Core.Entities
{
    public class ScoringRule
    {
        public Guid RuleId { get; set; }
        public string RuleName { get; set; }
        public string Description { get; set; }
        public string ConditionsJson { get; set; } // Store rule conditions as JSON
        public string ActionsJson { get; set; } // Store actions as JSON
        public bool IsActive { get; set; }
        public string Scope { get; set; } // "SinglePlayer" or "Multiplayer"
        public int Priority { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}