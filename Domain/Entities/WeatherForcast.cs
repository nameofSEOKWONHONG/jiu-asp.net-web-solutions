using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities {
    public class WeatherForecast {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
