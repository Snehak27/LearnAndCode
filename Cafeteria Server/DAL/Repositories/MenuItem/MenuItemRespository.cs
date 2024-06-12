using CafeteriaServer.Context;
using CafeteriaServer.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.Repositories
{
    public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
