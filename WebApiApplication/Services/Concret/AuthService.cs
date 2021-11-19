using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Application.Dtos;
using Application.Entities;
using Application.Request;
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
        
        public async Task<string> Login(User user)
        {
            var selectedUser = await userService.FindUserByEmailAsync(user.Email);
            if (selectedUser == null) throw new Exception("not found user.");
            
            if (BCrypt.Net.BCrypt.Verify(user.Password, selectedUser.Password))
            {
                return CreateToken(selectedUser);    
            }

            return string.Empty;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
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