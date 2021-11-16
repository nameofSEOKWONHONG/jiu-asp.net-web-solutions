using System.ComponentModel.DataAnnotations;

namespace SharedLibrary.Request.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}