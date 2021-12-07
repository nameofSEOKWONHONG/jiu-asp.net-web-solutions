using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    [Index(nameof(WriteId), nameof(WriteDt), nameof(UpdateId), nameof(UpdateDt))]
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