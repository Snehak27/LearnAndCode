using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;
using System.Threading.Tasks;

namespace CafeteriaServer.DAL.Repositories
{
    public class MealTypeRepository : GenericRepository<MealType>, IMealTypeRepository
    {
        public MealTypeRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
