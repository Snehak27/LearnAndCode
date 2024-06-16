using CafeteriaServer.Context;
using CafeteriaServer.Repositories.Generic;
using CafeteriaServer.DAL.Models;

namespace CafeteriaServer.DAL.Repositories
{
    public class FeedbackRepository : GenericRepository<Feedback>, IFeedbackRepository
    {
        public FeedbackRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
