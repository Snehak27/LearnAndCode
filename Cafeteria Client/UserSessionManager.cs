using System;

namespace CafeteriaClient
{
    public class UserSessionManager
    {
        public int UserId { get; private set; }
        public int UserRoleId { get; private set; }

        public void SetUser(int userId, int userRoleId)
        {
            UserId = userId;
            UserRoleId = userRoleId;
        }

        public void ClearUser()
        {
            UserId = 0;
            UserRoleId = 0;
        }

        public int GetUserId()
        {
            return UserId;
        }
    }
}
