using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Dtos;
using SharedLibrary.Entities;
using SharedLibrary.Request;
using WebApiApplication.Services.Abstract;

namespace WebApiApplication.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration configuration;
        private readonly IUserService userService;
        public AuthService(IConfiguration configuration, IUserService userService)
        {
            this.configuration = configuration;
            this.userService = userService;
        }
        
        public async Task<string> Login(RegisterRequest registerRequest)
        {
            var user = await userService.FindUserByEmailAsync(registerRequest.Email);
            if (user == null) throw new Exception("not found user.");
            
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);
            if (BCrypt.Net.BCrypt.Verify(registerRequest.Password, user.Password))
            {
                return CreateToken(user);    
            }

            return string.Empty;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); 
            
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],    
                configuration["Jwt:Issuer"],    
                claims,    
                expires: DateTime.Now.AddMinutes(120),    
                signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token); 
        }
    }
}