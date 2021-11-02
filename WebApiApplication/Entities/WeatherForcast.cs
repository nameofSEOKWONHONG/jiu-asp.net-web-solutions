using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MongoDB.Bson;
using Realms;

namespace WebApiApplication.Entities {
    [ValidateNever]
    public class WeatherForcast : RealmObject {
        [PrimaryKey]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        [Required]
        public string Name {get;set;}
        [Required]
        public string Description { get; set; }
    }
}
