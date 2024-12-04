using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class EmployeePreferenceRepository : GenericRepository<EmployeePreference>, IEmployeePreferenceRepository
    {
        public EmployeePreferenceRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
