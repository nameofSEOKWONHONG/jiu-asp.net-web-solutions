using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApiApplication.SharedLibrary.Dtos
{
    public class UserRequest
    {
        [EmailAddress]
        [Required, MaxLength(200)]
        public string Email { get; set; }
        [Required, MaxLength(200)]
        public string Password { get; set; }
    }
}