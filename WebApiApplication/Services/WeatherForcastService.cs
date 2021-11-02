using System.Collections.Generic;
using System.Linq;
using Realms;
using WebApiApplication.Entities;

namespace WebApiApplication.Services {
    public interface IWeatherForcastService {
        WeatherForcast GetWeatherForcast(string name);
        void SaveWeatherForcast(WeatherForcast weatherForcast);
    }

    public class WeatherForcastService : IWeatherForcastService {
        private readonly IEnumerable<WeatherForcast> _weatherForcasts = new List<WeatherForcast>() {
            new WeatherForcast() { Name = "1", Description = "test1" },
            new WeatherForcast() { Name = "2", Description = "test2" },
            new WeatherForcast() { Name = "3", Description = "test3" },
            new WeatherForcast() { Name = "4", Description = "test4" },
        };

        public WeatherForcastService() {
        }

        public WeatherForcast GetWeatherForcast(string name)
        {
            var realm = Realm.GetInstance();
            return realm.All<WeatherForcast>().FirstOrDefault(x => x.Name == name);
        }

        public void SaveWeatherForcast(WeatherForcast weatherForcast)
        {
            var realm = Realm.GetInstance();
            realm.Write(() => {
                realm.Add<WeatherForcast>(weatherForcast);
            });
        }
    }
}
