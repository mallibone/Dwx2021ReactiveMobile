using System;
using System.Net.Http;
using System.Reactive.Linq;
using Akavache;
using Newtonsoft.Json;
using WebApplication;

namespace ReactiveWeather.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private const string BackendUrl = "https://dwxweatherforecast.azurewebsites.net/api/weatherforecast/forpostalcode/{0}";

        public WeatherService()
        {
            _httpClient = new HttpClient();
        }

        public IObservable<WeatherForecast> GetWeatherForecast(int postalCode)
        {
            var url = string.Format(BackendUrl, postalCode);
            return 
                Observable.StartAsync(() => _httpClient.GetStringAsync(url))
                .Select(JsonConvert.DeserializeObject<WeatherForecast>);
        }
        
        public IObservable<WeatherForecast> GetWeatherForecastCache(int postalCode)
        {
            return
                BlobCache.UserAccount.GetOrFetchObject(
                    $"forecast{postalCode}", 
                    () => GetWeatherForecast(postalCode),
                DateTimeOffset.Now.AddSeconds(5));
        }
    }
}