using CafeteriaServer.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
    }
}
