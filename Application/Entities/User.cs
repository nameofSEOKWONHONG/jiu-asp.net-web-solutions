using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace Application.Entities
{
    
    [Table("User")]
    public class User : EntityBase
    {
        [Required, Key]
        public Guid Id { get; set; }
        
        [Required, EmailAddress, MaxLength(200), NotNull]
        public string Email { get; set; }
        
        [Required, MaxLength(200), NotNull]
        public string Password { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public bool ActivateUser { get; set; } = false;
        
        public bool AutoConfirmEmail { get; set; } = false;    
    }
}