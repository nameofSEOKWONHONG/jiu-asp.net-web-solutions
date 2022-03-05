using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{   
    [Table("TB_USER")]
    [Index(nameof(EMAIL), nameof(ACTIVE_USER_YN), nameof(AUTO_CONFIRM_EMAIL_YN), IsUnique = false)]
    public class TB_USER : GuidEntityBase
    {
        [Required, EmailAddress, MaxLength(200), NotNull]
        public string EMAIL { get; set; }
        
        [Required, MaxLength(200), NotNull]
        public string PASSWORD { get; set; }
        
        public string MOBILE { get; set; }
        
        public bool ACTIVE_USER_YN { get; set; } = false;
        
        public bool AUTO_CONFIRM_EMAIL_YN { get; set; } = false;    
        
        //public List<RefreshToken> RefreshTokens { get; set; }
        public TB_ROLE ROLE { get; set; }
        
        public class Validator : AbstractValidator<TB_USER>
        {
            public Validator()
            {
                RuleFor(m => m.MOBILE).MaximumLength(12);
            }
        }
    }
}