using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class CuisinePreferenceRepository : GenericRepository<CuisineType>, ICuisinePreferenceRepository
    {
        public CuisinePreferenceRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
