using CafeteriaServer.Context;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.Repositories.Generic;
using System;

namespace CafeteriaServer.DAL.Repositories
{
    public class UserNotificationRepository : GenericRepository<UserNotification>, IUserNotificationRepository
    {
        public UserNotificationRepository(CafeteriaContext context) : base(context)
        {
        }
    }
}
