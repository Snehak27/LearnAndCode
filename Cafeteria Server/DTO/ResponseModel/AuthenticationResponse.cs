using CafeteriaServer.DAL.Models;
using System;

namespace CafeteriaServer.DTO
{
    public class AuthenticationResponse
    {
        public bool IsAuthenticated { get; set; }
        public User User { get; set; }
        public string ErrorMessage { get; set; }
    }
}
