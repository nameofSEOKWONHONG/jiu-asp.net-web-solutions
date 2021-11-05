using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace WebApiApplication.SharedLibrary.Entities
{
    
    [Table("User")]
    public class User
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required, EmailAddress, MaxLength(200), NotNull]
        public string Email { get; set; }
        
        [Required, MaxLength(200), NotNull]
        public string Password { get; set; }
    }
}