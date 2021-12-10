using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("Group")]
    public class Group : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), ForeignKey("User")]
        public Guid Id { get; set; }
        
        [Required, MaxLength(200)]
        public string GroupName { get; set; }
        
        [Required]
        public virtual User User { get; set; }
    }
}