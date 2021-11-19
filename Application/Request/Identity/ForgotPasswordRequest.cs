using System.ComponentModel.DataAnnotations;

namespace Application.Request.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}