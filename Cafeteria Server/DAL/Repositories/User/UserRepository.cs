using CafeteriaServer.Context;
using CafeteriaServer.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
