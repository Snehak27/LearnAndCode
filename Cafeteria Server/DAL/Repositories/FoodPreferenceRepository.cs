using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class FoodPreferenceRepository : GenericRepository<FoodType>, IFoodPreferenceRepository
    {
        public FoodPreferenceRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
