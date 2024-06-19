using CafeteriaClient.DTO.Response;
using System;

namespace CafeteriaClient.DTO
{
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public User User { get; set; }
        public string ErrorMessage { get; set; }
        public List<NotificationResponse> Notifications { get; set; }
    }

    public class User
    {
        public int UserId { get; set; }
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
    }
}
