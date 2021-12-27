using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductManagementApi.Identity;
using ProductManagementApi.Models.DataTransferObjects;
using ProductManagementApi.Persistence;
using ProductManagementApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagementApi.Repository.Implementation
{
    public class UserRepository:IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly RoleManager<IdentityRole> _roleManager;
        readonly ApplicationDbContext _context;
        public UserRepository(
            UserManager<ApplicationUser> userManager,
            TokenValidationParameters tokenValidationParameters,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            this.tokenValidationParameters = tokenValidationParameters;
            _roleManager = roleManager;
        }

        public async Task<AuthResult> Login(LoginDto loginDto)
        {
            var userFoundByEmail = await _userManager.FindByEmailAsync(loginDto.Email);
            if (userFoundByEmail != null && await _userManager.CheckPasswordAsync(userFoundByEmail,loginDto.Password))
            {
                return await GenerateJWTToken(userFoundByEmail);
            }
            else
            {
                // result.AddError("Invalid Username/Password");
                return new AuthResult { HasError = true };
            }
        }

        public async void Logout(string userId)
        {
           var refreshTokens = _context.RefreshTokens.Where(x => x.UserId == userId);
            _context.RefreshTokens.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync(); 
        }

        public async Task<AuthResult> RefreshToken(RefreshTokenDto value)
        {

            var validToken = GetPrincipalFromToken(value.Token);
            if (validToken == null)
            {
                return new AuthResult { HasError = true, Message = "Invalid Token" };
            }

            var expirydate = long.Parse(validToken.Claims.Single(p => p.Type == JwtRegisteredClaimNames.Exp).Value);
            var expirytimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(expirydate);
            if (expirytimeUtc > DateTime.UtcNow)
            {
                return new AuthResult { HasError = true, Message = "Token is still valid" };
            }

            var jti = validToken.Claims.Single(p => p.Type == JwtRegisteredClaimNames.Jti).Value;

            var stored = await _context.RefreshTokens.SingleOrDefaultAsync(p => p.Token == value.RefreshToken);
            if (stored == null)
            {
                return new AuthResult { HasError = true, Message = "Refresh Token does not exist" };
            }
            if (stored.Invalidated)
            {
                return new AuthResult { HasError = true, Message = "Refresh Token has been invalidated. User Logged out" };
            }

            if (DateTime.UtcNow > stored.ExpiryDate)
            {
                return new AuthResult { HasError = true, Message = "Refresh Token has not expired" };
            }

            if (stored.IsUsed)
            {
                return new AuthResult { HasError = true, Message = "Refresh Token have been used" };
            }

            if (stored.JwtId != jti)
            {
                return new AuthResult { HasError = true, Message = "Refresh Token does not match" }; 
            }

            stored.IsUsed = true;
            var appuser = await _userManager.FindByIdAsync(stored.UserId);
            _context.RefreshTokens.Remove(stored);
            await _context.SaveChangesAsync();            

            return await GenerateJWTToken(appuser);
            
        }


        public async Task<AuthResult> SignUpSubAdmin(SignupDto signupDto,string role)
        {
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = signupDto.Email,
                UserName = signupDto.Email
            };
           
                if (await _roleManager.FindByNameAsync(role) == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                };

            try
                {
                    var _user = await _userManager.CreateAsync(user, signupDto.Password);
                if (_user.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    return await GenerateJWTToken(user);
                }
                else { return new AuthResult { HasError = true, Message = "User creation failed" }; }
                }
                catch (Exception ex)
                {
                return new AuthResult { HasError = true, Message = ex.Message };
            }
                    
        }
        

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsValidAlgorithm(validatedToken))
                    return null;
                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private bool IsValidAlgorithm(SecurityToken securityToken)
        {
            return (securityToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        private async Task<AuthResult> GenerateJWTToken(ApplicationUser user)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var signinKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("MySuperSecureKey"));

            var roles = await _userManager.GetRolesAsync(user);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                { new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                  new Claim("UserId",user.Id),
                  new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                  new Claim(ClaimTypes.Role, roles[0])
                }),
                Issuer = "issue",
                Audience = "audience",
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)

            };         
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(1),
                Token = Guid.NewGuid().ToString(),
                JwtId = token.Id
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResult { ExpiryTimeinMinutes = (token.ValidTo - token.ValidFrom).TotalMinutes.ToString(), Token = tokenHandler.WriteToken(token),RefreshToken=refreshToken.Token};
        }
    }
}

