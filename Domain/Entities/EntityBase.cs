using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Domain.Entities
{
    public class EntityBase
    {
        [Required]
        public string WriteId { get; set; }
        
        [Required]
        public DateTime WriteDt { get; set; }
        
        public string UpdateId { get; set; }
        
        public DateTime? UpdateDt { get; set; }
    }
}