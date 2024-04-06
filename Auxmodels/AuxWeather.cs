using Newtonsoft.Json;


namespace api_weather.Auxmodels
{
    public class AuxWeather
    {
        public List<Weather>? weather { get; set; }
        public Main? main { get; set; }
        public int visibility { get; set; }
        public Wind? wind { get; set; }
    }

    public class Main
    {
        public float? temp { get; set; }
        public float? feels_like { get; set; }
        public float? temp_min { get; set; }
        public float? temp_max { get; set; }
        public float? pressure { get; set; }
        public float? humidity { get; set; }
    }

    public class Weather
    {
        public string? main { get; set; }
        public string? description { get; set; }
        public string? icon { get; set; }
    }

    public class Wind
    {
        public double? speed { get; set; }
    }

}
