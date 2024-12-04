using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class SpiceLevelRepository : GenericRepository<SpiceLevel>, ISpiceLevelRepository
    {
        public SpiceLevelRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
