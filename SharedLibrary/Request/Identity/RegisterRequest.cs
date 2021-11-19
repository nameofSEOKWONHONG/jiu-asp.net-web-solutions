using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SharedLibrary.Request
{
    public class SignInRequest
    {
        [EmailAddress]
        [Required, MaxLength(200)]
        public string Email { get; set; }
        
        [Required, MaxLength(200)]
        public string Password { get; set; }
    }
    
    public class RegisterRequest
    {
        [EmailAddress]
        [Required, MaxLength(200)]
        public string Email { get; set; }
        
        [Required, MaxLength(200)]
        public string Password { get; set; }
        
        [Required, Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        
        public string PhoneNumber { get; set; }
    }
}