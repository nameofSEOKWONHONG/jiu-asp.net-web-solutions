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
    public class TB_ROLE : AutoIncEntityBase
    {
        [Required, MaxLength(10)]
        public ENUM_ROLE_TYPE ROLE_TYPE { get; set; }
        
        public TB_ROLE_PERMISSION ROLE_PERMISSION { get; set; }
    }
}