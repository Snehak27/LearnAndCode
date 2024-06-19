using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using System;

namespace CafeteriaServer.DTO
{
    public class AuthenticationResponse
    {
        public bool IsAuthenticated { get; set; }
        public User User { get; set; }
        public string ErrorMessage { get; set; }
        public List<NotificationResponse> Notifications { get; set; }
    }
}
