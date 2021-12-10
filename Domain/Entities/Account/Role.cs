using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Domain.Enums;
using eXtensionSharp;

namespace Domain.Entities
{
    [Table("TB_ROLE")]
    public class Role : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required, MaxLength(10)]
        public ENUM_ROLE_TYPE RoleType { get; set; }
        
        public RolePermission RolePermission { get; set; }
    }
}