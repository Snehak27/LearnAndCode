using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class RecommendationRepository : GenericRepository<Recommendation>, IRecommendationRepository
    {
        public RecommendationRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
