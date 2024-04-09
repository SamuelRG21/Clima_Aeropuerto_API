using api_weather.Models;
using Newtonsoft.Json;
using CsvHelper.Configuration.Attributes;
namespace api_weather.Auxmodels
{
    public class TicketsAux
    {
        [Name("origin")]
        public string? Origin { get; set; }
        [Name("destination")]
        public string? Destination { get; set; }
        [Name("airline")]
        public string Airline { get; set; }
        [Name("flight_num")]
        public int Flight_num { get; set; }
        [Name("origin_iata_code")]
        public string Origin_iata_code { get; set; }
        [Name("origin_name")]
        public string Origin_name { get; set; }
        [Name("origin_latitude")]
        public float Origin_latitude { get; set; }
        [Name("origin_longitude")]
        public float Origin_longitude { get; set; }
        [Name("destination_iata_code")]
        public string Destination_iata_code { get; set; }
        [Name("destination_name")]
        public string Destination_name { get; set; }
        [Name("destination_latitude")]
        public float Destination_latitude { get; set; }
        [Name("destination_longitude")]
        public float Destination_longitude { get; set; }
    }
}
