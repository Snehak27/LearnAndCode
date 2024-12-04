using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class DiscardMenuItemLogRepository : GenericRepository<DiscardedMenuItem>, IDiscardedMenuItemRepository
    {
        public DiscardMenuItemLogRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
