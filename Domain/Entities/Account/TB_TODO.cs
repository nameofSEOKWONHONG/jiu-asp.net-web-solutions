using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    [Table(nameof(TB_TODO))]
    [Index(nameof(CONTENTS), IsUnique = false)]
    public class TB_TODO : EntityBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        
        [Required]
        public string CONTENTS { get; set; }
        
        public DateTime? NOTIFY_DT { get; set; }
    }
}