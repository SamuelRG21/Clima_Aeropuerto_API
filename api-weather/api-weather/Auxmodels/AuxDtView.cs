using api_weather.Models;

namespace api_weather.Auxmodels
{
    public class AuxDtView
    {
        public Message message { get; set; }
        public List<AuxFlight> Flight { get; set; }

        public AuxDtView()
        {
            Flight = new List<AuxFlight>();
            message = new Message();
        }
    }
    public class AuxFlight
    {
        public string? Airline { get; set; }
        public string? Flight_num { get; set; }
        public List<AuxAirport?> Airports { get; set; }
        public AuxFlight()
        {
            Airports = new List<AuxAirport?>();
        }
    }
    public class AuxAirport
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public WeatherDay? Weather { get; set; }
        public short? Journey { get; set; }
        public AuxAirport()
        {
            Weather = new WeatherDay();
        }
    } 
}
