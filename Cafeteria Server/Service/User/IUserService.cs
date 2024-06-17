using CafeteriaServer.DAL.Models;
using CafeteriaServer.DTO;
using System;

namespace CafeteriaServer.Service
{
    public interface IUserService
    {
        Task<AuthenticationResponse> AuthenticateUser(AuthenticationRequest authenticationRequest);
        Task<IEnumerable<MenuItem>> GetAllMenuItems();
    }
}
