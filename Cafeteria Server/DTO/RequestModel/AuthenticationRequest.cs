using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeteriaServer.DTO
{
    public class AuthenticationRequest
    {
        public string EmployeeId { get; set; }
        public string Password { get; set; }
    }
}
