using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApiApplication.Dtos;
using WebApiApplication.Entities;

namespace WebApiApplication.Services
{
    public interface IAuthService
    {
        Task<string> Login(UserRequest userRequest);
    }
    
    public class AuthService : IAuthService
    {
        private readonly IConfiguration configuration;
        private readonly IUserService userService;
        public AuthService(IConfiguration configuration, IUserService userService)
        {
            this.configuration = configuration;
            this.userService = userService;
        }
        
        public async Task<string> Login(UserRequest userRequest)
        {
            var user = await userService.FindUserByEmailAsync(userRequest.Email);
            if (user == null) throw new Exception("not found user.");
            
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRequest.Password);
            if (BCrypt.Net.BCrypt.Verify(userRequest.Password, user.Password))
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
                new Claim(JwtRegisteredClaimNames.Birthdate, "2020-01-01"),
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