using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using CafeteriaServer.UnitofWork;

namespace CafeteriaServer.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> ProvideFeedback(FeedbackRequest feedbackRequest)
        {
            var feedback = new Feedback
            {
                MenuItemId = feedbackRequest.MenuItemId,
                UserId = feedbackRequest.UserId,
                Comment = feedbackRequest.Comment,
                Rating = feedbackRequest.Rating,
                FeedbackDate = DateTime.UtcNow,
                MealTypeId = feedbackRequest.MealTypeId,
            };

            await _unitOfWork.Feedbacks.Add(feedback);
            _unitOfWork.Save();

            return true;
        }
    }
}
