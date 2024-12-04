using System;

namespace CafeteriaClient.DTO
{
    public class AuthenticationRequest
    {
        public string EmployeeId { get; set; }
        public string Password { get; set; }
    }
}
