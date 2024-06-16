using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class EmployeeResponseRepository : GenericRepository<EmployeeResponse>, IEmployeeResponseRepository
    {
        public EmployeeResponseRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
