using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
