using System;

namespace CafeteriaServer.DTO
{
    public class MonthlyFeedbackReportResponse
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public MonthlyFeedbackReport Report { get; set; }
    }

    public class MonthlyFeedbackReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public List<FeedbackSummary> FeedbackSummaries { get; set; }
    }

    public class FeedbackSummary
    {
        public string MenuItemName { get; set; }
        public double AverageRating { get; set; }
        public int FeedbackCount { get; set; }
        public List<string> Comments { get; set; }
    }
}
