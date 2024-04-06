using api_weather.Auxmodels;
using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Reflection.Metadata;
using Newtonsoft.Json;
using api_weather.Models;
namespace api_weather.Tools
{
    public class Tool
    {
        private readonly IHttpClientFactory _httpClient;
        public Tool(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<AuxWeather?> GetWeatherDay(AuxWeather? weatherDay, double? lat, double? lon)
        {
            var client = _httpClient.CreateClient("ApiWeather");
            string url = $"?lat={lat}&lon={lon}&appid={"eb1014cc588d06f3554444089f83bca7"}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                weatherDay = JsonConvert.DeserializeObject<AuxWeather?>(response.Content.ReadAsStringAsync().Result);
                return weatherDay ?? null;
            }
            else
            {
                throw new HttpRequestException($"Error al obtener datos de la API: {response.StatusCode}");
            }
        }
    }
}
