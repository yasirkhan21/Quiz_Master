using QuizMaster.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuizMaster.Application.Algorithms
{
    public class RecommendationEngine
    {
        private readonly double _decayFactor = 0.9;

        // Build user weakness vector from profiles
        public Dictionary<string, double> BuildWeaknessVector(
            IEnumerable<UserWeaknessProfile> profiles)
        {
            var vector = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            foreach (var profile in profiles)
            {
                // Apply exponential decay based on days since last update
                var daysSinceUpdate = (DateTime.UtcNow - profile.LastUpdated).TotalDays;
                var decayedWeight = (double)profile.WeightScore * 
                    Math.Pow(_decayFactor, daysSinceUpdate);

                vector[profile.Topic] = decayedWeight;
            }

            return NormalizeVector(vector);
        }

        // Build question tag vector
        public Dictionary<string, double> BuildQuestionVector(List<string> tags)
        {
            var vector = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            if (tags == null || !tags.Any())
                return vector;

            // Equal weight for each tag
            double weight = 1.0 / tags.Count;

            foreach (var tag in tags)
            {
                if (vector.ContainsKey(tag))
                    vector[tag] += weight;
                else
                    vector[tag] = weight;
            }

            return NormalizeVector(vector);
        }

        // Calculate cosine similarity between two vectors
        public double CalculateCosineSimilarity(
            Dictionary<string, double> vectorA,
            Dictionary<string, double> vectorB)
        {
            if (!vectorA.Any() || !vectorB.Any())
                return 0.0;

            // Dot product
            double dotProduct = vectorA
                .Where(kv => vectorB.ContainsKey(kv.Key))
                .Sum(kv => kv.Value * vectorB[kv.Key]);

            // Magnitudes
            double magnitudeA = Math.Sqrt(vectorA.Values.Sum(v => v * v));
            double magnitudeB = Math.Sqrt(vectorB.Values.Sum(v => v * v));

            if (magnitudeA == 0 || magnitudeB == 0)
                return 0.0;

            return dotProduct / (magnitudeA * magnitudeB);
        }

        // Rank questions by similarity to user weakness profile
        public List<(Question Question, double Score)> RankQuestionsByWeakness(
            IEnumerable<Question> questions,
            Dictionary<string, double> weaknessVector,
            IEnumerable<Guid> recentlyAnsweredIds,
            int count = 10)
        {
            var recentIds = new HashSet<Guid>(recentlyAnsweredIds);

            var scoredQuestions = questions
                .Where(q => !recentIds.Contains(q.QuestionId)) // Exclude recently answered
                .Select(q => new
                {
                    Question = q,
                    Vector = BuildQuestionVector(q.Tags ?? new List<string> { q.Category }),
                })
                .Select(q => (
                    Question: q.Question,
                    Score: CalculateCosineSimilarity(weaknessVector, q.Vector)
                ))
                .OrderByDescending(x => x.Score)
                .ToList();

            // Ensure diversity by grouping and picking from different topics
            return EnsureDiversity(scoredQuestions, count);
        }

        // Ensure question diversity in recommendations
        private List<(Question Question, double Score)> EnsureDiversity(
            List<(Question Question, double Score)> rankedQuestions,
            int count)
        {
            var result = new List<(Question Question, double Score)>();
            var usedTopics = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // First pass: add highest scoring unique topics
            foreach (var item in rankedQuestions)
            {
                if (result.Count >= count) break;

                var primaryTopic = item.Question.Tags?.FirstOrDefault() 
                    ?? item.Question.Category;

                if (!usedTopics.Contains(primaryTopic))
                {
                    result.Add(item);
                    usedTopics.Add(primaryTopic);
                }
            }

            // Second pass: fill remaining slots with next best questions
            foreach (var item in rankedQuestions)
            {
                if (result.Count >= count) break;

                if (!result.Any(r => r.Question.QuestionId == item.Question.QuestionId))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        private Dictionary<string, double> NormalizeVector(Dictionary<string, double> vector)
        {
            double magnitude = Math.Sqrt(vector.Values.Sum(v => v * v));

            if (magnitude == 0) return vector;

            return vector.ToDictionary(
                kv => kv.Key,
                kv => kv.Value / magnitude,
                StringComparer.OrdinalIgnoreCase);
        }
    }
}