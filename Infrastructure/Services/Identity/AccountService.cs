using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using eXtensionSharp;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebApiApplication.Services.Abstract;

namespace Infrastructure.Services.Identity
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        public AccountService(IConfiguration configuration, IUserService userService, IRoleService roleService)
        {
            this._configuration = configuration;
            this._userService = userService;
            this._roleService = roleService;
        }
        
        public async Task<string> Login(User user)
        {
            var selectedUser = await _userService.FindUserByEmailAsync(user.Email);
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

            var role = this._roleService.GetRole(user.Role.Id).GetAwaiter().GetResult();
            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.RoleType.ToString()));
            claims.Add(new Claim(role.RoleType.ToString(), user.Role.RolePermission.RolePermissionTypes.xToJoin()));
            
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); 
            
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],    
                _configuration["Jwt:Issuer"],    
                claims,    
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token); 
        }
    }
}