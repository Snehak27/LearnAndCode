using System;

namespace CafeteriaServer.Service
{
    public interface ISentimentAnalyzer
    {
        double AnalyzeSentiment(string comment);
        string GetSentimentLabel(double sentimentScore);
    }
}
