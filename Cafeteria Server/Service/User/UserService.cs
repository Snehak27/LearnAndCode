using CafeteriaServer.DTO;
using CafeteriaServer.DAL.Models;
using CafeteriaServer.UnitofWork;
using System;

namespace CafeteriaServer.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthenticationResponse> AuthenticateUser(AuthenticationRequest authenticationRequest)
        {
            var authenticationResult = new AuthenticationResponse();

            var user = await _unitOfWork.Users.Find(u => u.EmployeeId == authenticationRequest.EmployeeId);

            if (user != null && user.Password == authenticationRequest.Password)
            {
                authenticationResult.IsAuthenticated = true;
                authenticationResult.User = user;
            }
            else
            {
                authenticationResult.IsAuthenticated = false;
                authenticationResult.ErrorMessage = user == null ? "User not found." : "Invalid password.";
            }

            return authenticationResult;
        }

        public async Task<IEnumerable<MenuItem>> GetAllMenuItems()
        {
            return await _unitOfWork.MenuItems.GetAll();
        }

        public async Task<User> GetUserById(int userId)
        {
            var user = await _unitOfWork.Users.GetById(userId);
            if(user != null) 
            { 
                return user; 
            }
            else
            {
                throw new Exception("User not found");
            }
        }
    }
}
