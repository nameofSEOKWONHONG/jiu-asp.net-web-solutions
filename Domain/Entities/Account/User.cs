using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{   
    [Table("TB_USER")]
    [Index(nameof(Email), nameof(ActivateUser), nameof(AutoConfirmEmail), IsUnique = false)]
    public class User : EntityBase
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        [Required, EmailAddress, MaxLength(200), NotNull]
        public string Email { get; set; }
        
        [Required, MaxLength(200), NotNull]
        public string Password { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public bool ActivateUser { get; set; } = false;
        
        public bool AutoConfirmEmail { get; set; } = false;    
        
        //public List<RefreshToken> RefreshTokens { get; set; }
        public Role Role { get; set; }
    }
}