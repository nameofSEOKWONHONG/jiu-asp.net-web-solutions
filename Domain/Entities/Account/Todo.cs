using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    [Table("TB_TODO")]
    [Index(nameof(Contents), IsUnique = false)]
    public class Todo : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        public string Contents { get; set; }
        
        public DateTime? NotifyDate { get; set; }
    }
}