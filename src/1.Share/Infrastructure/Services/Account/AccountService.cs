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

namespace Infrastructure.Services.Account
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
        
        public async Task<string> Login(TB_USER tbUser)
        {
            var selectedUser = await _userService.FindUserByEmailAsync(tbUser.EMAIL);
            if (selectedUser == null) throw new Exception("not found user.");
            
            if (BCrypt.Net.BCrypt.Verify(tbUser.PASSWORD, selectedUser.PASSWORD))
            {
                return CreateToken(selectedUser);    
            }

            return string.Empty;
        }

        private string CreateToken(TB_USER tbUser)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.NameId, tbUser.ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, tbUser.EMAIL),
            };

            var role = this._roleService.GetRole(tbUser.ROLE.ID).GetAwaiter().GetResult();
            claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ROLE_TYPE.ToString()));
            claims.Add(new Claim(role.ROLE_TYPE.ToString(), tbUser.ROLE.ROLE_PERMISSION.ROLE_PERMISSION_TYPES.xJoin()));
            
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