using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Bson;
using Realms;

namespace WebApiApplication.Entities {
    [ValidateNever]
    public class WeatherForecast {
        [PrimaryKey]
        public int Id { get; set; }
        
        [Required]
        public DateTime Date { get; set; }
        
        [Required]
        public int TemperatureC { get; set; }
        
        [NotMapped]
        public int TemperatureF => (32 + (int)(TemperatureC / 0.5556));
        
        [Required]
        public string Summary { get; set; }
    }
}
