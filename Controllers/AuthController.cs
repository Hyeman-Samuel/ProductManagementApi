using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagementApi.Identity;
using ProductManagementApi.Models.DataTransferObjects;
using ProductManagementApi.Repository.Interfaces;
using ProductManagementApi.Utility;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductManagementApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        // GET: api/<controller>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return Response<string>(data: null, message: "", ApiResponseCode.INVALID_REQUEST, errors: ListModelStateErrors.ToArray());
            }

            var result = await _userRepository.Login(loginDto);
            if(result.HasError)
                return Response<string>(data: null, message: "Invalid login attempt", ApiResponseCode.INVALID_REQUEST, errors: ListModelStateErrors.ToArray());

            return ApiResponse(data: result);
        }

        // GET: api/<controller>
        [HttpPost("create")]
        public async Task<IActionResult> CreateSubAdmin([FromBody]SignupDto signupDto)
        {
            if (!ModelState.IsValid)
            {
                return Response<string>(data: null, message: "", ApiResponseCode.INVALID_REQUEST, errors: ListModelStateErrors.ToArray());
            }
            var result = await _userRepository.SignUpSubAdmin(signupDto,Roles.SubAdmin);
            if (result.HasError)
                return Response<string>(data: null, message: result.Message, ApiResponseCode.INVALID_REQUEST, errors: ListModelStateErrors.ToArray());

            return ApiResponse(data: result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
            {
                return Response<string>(data: null, message: "", ApiResponseCode.INVALID_REQUEST, errors: ListModelStateErrors.ToArray());
            }
            var result = await _userRepository.RefreshToken(refreshTokenDto);
            if (result.HasError)
                return Response<string>(data: null, message: result.Message, ApiResponseCode.INVALID_REQUEST, errors: ListModelStateErrors.ToArray());

            return ApiResponse(data: result);
        }


        [HttpDelete("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            string userIdAsString = HttpContext.User.FindFirst("UserId").Value;
            if(!Guid.TryParse(userIdAsString,out Guid userId))
            {
                return Unauthorized();
            }
            _userRepository.Logout(userId.ToString());

            return Ok();
        }


    }
}
