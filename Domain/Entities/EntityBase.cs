using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Domain.Entities
{
    [Index(nameof(WRITE_ID), nameof(WRITE_DT), nameof(UPDATE_ID), nameof(UPDATE_DT), IsUnique = false)]
    public class EntityBase
    {
        [Required]
        public string WRITE_ID { get; set; }
        
        [Required]
        public DateTime WRITE_DT { get; set; }
        
        public string UPDATE_ID { get; set; }
        
        public DateTime? UPDATE_DT { get; set; }
    }
}