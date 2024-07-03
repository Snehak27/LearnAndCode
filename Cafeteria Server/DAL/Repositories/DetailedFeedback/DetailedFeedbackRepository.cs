using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class DetailedFeedbackRepository : GenericRepository<DetailedFeedback>, IDetailedFeedbackRepository
    {
        public DetailedFeedbackRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
