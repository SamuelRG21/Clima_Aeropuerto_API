using api_weather.Models;

namespace api_weather.Auxmodels
{
    public class TicketsAux
    {
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public string? Airline { get; set; }
        public string? Flight_num { get; set; }
        public string? Origin_iata_code { get; set; }
        public string? Origin_name { get; set; }
        public string? Origin_latitude { get; set; }
        public string? Origin_longitude { get; set; }
        public string? Destination_iata_code { get; set; }
        public string? Destination_name { get; set; }
        public string? Destination_latitude { get; set; }
        public string? Destination_longitude { get; set; }
    }
}
