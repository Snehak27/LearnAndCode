using System;

namespace CafeteriaServer.DTO
{
    public class AuthenticationRequest
    {
        public string EmployeeId { get; set; }
        public string Password { get; set; }
    }
}
