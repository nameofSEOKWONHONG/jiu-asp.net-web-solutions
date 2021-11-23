using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public class EntityBase
    {
        [Required]
        public Guid WriteId { get; set; }
        
        [Required]
        public DateTime WriteDt { get; set; }
        
        public Guid UpdateId { get; set; }
        
        public DateTime? UpdateDt { get; set; }
    }
}