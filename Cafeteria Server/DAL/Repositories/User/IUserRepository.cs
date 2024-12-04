using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
    }
}
