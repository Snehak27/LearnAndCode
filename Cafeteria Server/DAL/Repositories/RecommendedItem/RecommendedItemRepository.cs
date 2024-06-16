using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;

namespace CafeteriaServer.DAL.Repositories
{
    public class RecommendedItemRepository : GenericRepository<RecommendedItem>, IRecommendedItemRepository
    {
        public RecommendedItemRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
