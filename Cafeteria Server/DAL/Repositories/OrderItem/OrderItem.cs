using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class OrderItemRepository : GenericRepository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
