using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    [Table("TB_TODO")]
    public class Todo : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required, MaxLength, StringLength(int.MaxValue, MinimumLength = 3)]
        public string Contents { get; set; }
        
        public DateTime? NotifyDate { get; set; }
    }
}