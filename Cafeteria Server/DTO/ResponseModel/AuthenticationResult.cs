using CafeteriaServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CafeteriaServer.DTO
{
    public class AuthenticationResult
    {
        public bool IsAuthenticated { get; set; }
        public User User { get; set; }
        public string ErrorMessage { get; set; }
    }
}
