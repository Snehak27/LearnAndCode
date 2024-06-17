using System;

namespace CafeteriaServer.Service
{
    public class SentimentAnalyzer
    {
        private readonly Dictionary<string, int> _sentimentLexicon;

        public SentimentAnalyzer()
        {
            _sentimentLexicon = new Dictionary<string, int>
        {
            { "good", 1 }, { "excellent", 2 }, { "amazing", 2 },
            { "bad", -1 }, { "poor", -2 }, { "terrible", -2 }
        };
        }

        public double AnalyzeSentiment(string comment)
        {
            var words = comment.Split(' ');
            double sentimentScore = 0;
            foreach (var word in words)
            {
                if (_sentimentLexicon.TryGetValue(word, out var score))
                {
                    sentimentScore += score;
                }
            }
            return sentimentScore / words.Length;
        }
    }
}
