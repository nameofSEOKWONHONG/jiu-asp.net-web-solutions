using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("TB_GROUP")]
    public class TB_GROUP : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), ForeignKey("User")]
        public Guid ID { get; set; }
        
        [Required, MaxLength(200)]
        public string GROUP_NM { get; set; }
        
        [Required]
        public virtual TB_USER USER { get; set; }
    }
}