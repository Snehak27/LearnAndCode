using CafeteriaServer.DTO;
using System;

namespace CafeteriaServer.Service
{
    public interface IRecommendationService
    {
        Task<List<MealTypeRecommendations>> GetRecommendations();
    }
}
