using ProductManagementApi.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<AuthResult> SignUpSubAdmin(SignupDto signupDto,string role);
        Task<AuthResult> Login(LoginDto loginDto);
        Task<AuthResult> RefreshToken(RefreshTokenDto value);
        void Logout(string userId);
    }
}
