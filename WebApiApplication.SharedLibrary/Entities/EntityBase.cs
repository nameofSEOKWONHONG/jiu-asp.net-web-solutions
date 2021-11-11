using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace WebApiApplication.SharedLibrary.Entities
{
    public class EntityBase
    {
        [Required, NotNull, MaxLength(200)]
        public string WriteId { get; set; }
        
        [Required, NotNull]
        public DateTime WriteDt { get; set; }
        
        public string UpdateId { get; set; }
        
        public DateTime? UpdateDt { get; set; }
    }
}