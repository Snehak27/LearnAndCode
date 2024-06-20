using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Service
{
    public class SentimentAnalyzer : ISentimentAnalyzer
    {
        private readonly Dictionary<string, int> _sentimentLexicon;

        public SentimentAnalyzer()
        {
            var lexiconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Service", "sentiment_lexicon.json");
            _sentimentLexicon = LoadSentimentLexicon(lexiconPath);
        }

        private Dictionary<string, int> LoadSentimentLexicon(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("Sentiment lexicon file not found.", path);
            }

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
        }

        public double AnalyzeSentiment(string comment)
        {
            var words = comment.Split(' ');
            double sentimentScore = 0;
            foreach (var word in words)
            {
                if (_sentimentLexicon.TryGetValue(word.ToLower(), out var score))
                {
                    sentimentScore += score;
                }
            }
            return words.Length > 0 ? sentimentScore / words.Length : 0;
        }

        public string GetSentimentLabel(double sentimentScore)
        {
            if (sentimentScore > 0.5) return "Positive";
            if (sentimentScore < -0.5) return "Negative";
            return "Neutral";
        }
    }
}
