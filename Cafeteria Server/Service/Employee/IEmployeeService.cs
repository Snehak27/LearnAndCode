using CafeteriaServer.DTO;
using System;

namespace CafeteriaServer.Service
{
    public interface IEmployeeService
    {
        Task<bool> ProvideFeedback(FeedbackRequest feedbackRequest);
    }
}
