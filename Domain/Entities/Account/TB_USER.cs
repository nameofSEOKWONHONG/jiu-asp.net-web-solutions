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
    [Index(nameof(EMAIL), nameof(ACTIVE_USER_YN), nameof(AUTO_CONFIRM_EMAIL_YN), IsUnique = false)]
    public class TB_USER : EntityBase
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        
        [Required, EmailAddress, MaxLength(200), NotNull]
        public string EMAIL { get; set; }
        
        [Required, MaxLength(200), NotNull]
        public string PASSWORD { get; set; }
        
        public string PHONE_NUM { get; set; }
        
        public bool ACTIVE_USER_YN { get; set; } = false;
        
        public bool AUTO_CONFIRM_EMAIL_YN { get; set; } = false;    
        
        //public List<RefreshToken> RefreshTokens { get; set; }
        public TB_ROLE ROLE { get; set; }
    }
}