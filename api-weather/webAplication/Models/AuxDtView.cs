using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace webAplication.Models
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
    public  class WeatherDay
    {
        public int Id { get; set; }

        public int AirportId { get; set; }

        public string? Main { get; set; }

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public double? Temp { get; set; }

        public double? FeelsLike { get; set; }

        public double? TempMin { get; set; }

        public double? TempMax { get; set; }

        public double? Pressure { get; set; }

        public double? Humidity { get; set; }

        public double? Visibility { get; set; }

        public double? WindSpeed { get; set; }

    }
}
