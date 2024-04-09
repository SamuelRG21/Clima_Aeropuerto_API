using api_weather.Auxmodels;
using api_weather.Models;
using api_weather.Tools;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace api_weather.Dao
{
    public class DaoAux
    {
        private readonly Tool _tool;
        public DaoAux(Tool tool)
        {
            _tool = tool;
        }
        public async Task<AuxDtView> ReadFile(IFormFile file, FlightContext _flightContext, AuxDtView auxDtView)
        {
            List<TicketsAux> ticketsFL = []; //Lista auxiliar para agregar los registros del archivo CSV
            DateTime dt = DateTime.Now; //obtener fecha actual
            try
            {
                using var streamReader = new StreamReader(file.OpenReadStream()); //leer los registros del archivo csv.00000
                using var csv = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture));
                ticketsFL = await csv.GetRecordsAsync<TicketsAux>().ToListAsync();//leer los registros y convertirlos en una lista
                await _flightContext.Database.ExecuteSqlRawAsync("dbo.resetDataSet");//ejecutar store procedure para descartar datos anteriores           

                //depende de 
                var listAuxF = ticketsFL.GroupBy(q => new
                { //Lista auxiliar para obtener los tickets únicos (vuelos)
                    q.Flight_num,
                    q.Airline,
                    q.Origin_iata_code,
                    q.Destination_iata_code
                }).Select(group => new { Ticket = group.First(), Count = group.Count() }).ToList();

                List<Flight> listFlight = [];//crear lista para los vuelos
                listFlight = listAuxF.Select(q => q.Ticket).Select(q => new Flight() //tomar de la lista auxiliar de tickets únicos, los datos necesarios para crear la lista de vuelos
                {
                    FlightNum = q.Flight_num,
                    Airline = q.Airline,
                    Folio = q.Airline + q.Flight_num + q.Origin_iata_code + q.Destination_iata_code,
                }).ToList();
                await _flightContext.Flights.AddRangeAsync(listFlight);  //Agregar lista de vuelos únicos al contexto de la base de datos

                List<Airport> airportListAux = [];//crear lista de tipo Aeropuerto
                airportListAux = listAuxF.Select(q => q.Ticket).SelectMany(aero => new Airport[]{ //de la lista filtrada de ticket por vuelo, obtener la lista de aeropuertos únicos
                new() {
                    City = aero.Origin_iata_code,
                    Name = aero.Origin_name,
                    Lat = aero.Origin_latitude,
                    Lon = aero.Origin_longitude},
                new() {
                    City = aero.Destination_iata_code,
                    Name = aero.Destination_name,
                    Lat = aero.Destination_latitude,
                    Lon = aero.Destination_longitude},
            }).DistinctBy(q => q.City).ToList();
                await _flightContext.Airports.AddRangeAsync(airportListAux);  //Agregar lista de aeropuertos unicos al contexto de la base de datos
                await _flightContext.SaveChangesAsync();

                //obtener los tickets por cada vuelo
                List<Ticket> listTickets = [];
                listTickets = listAuxF.SelectMany(q =>
                {
                    var flightId = listFlight.First(f => f.Folio == q.Ticket.Airline + q.Ticket.Flight_num + q.Ticket.Origin + q.Ticket.Destination).Id;
                    var ticketsForFlight = new List<Ticket>();
                    for (int i = 0; i < q.Count; i++)
                    {
                        ticketsForFlight.Add(new Ticket
                        {
                            FlightId = flightId,
                        });
                    }
                    return ticketsForFlight;
                }).ToList();
                await _flightContext.Tickets.AddRangeAsync(listTickets); //agregar lsita de tickets a base de datos


                //Obtener datos de clima para cada una de las ciudades
                List<WeatherDay> listW = new();//lista auxiliar de datos de clima
                foreach (var item in airportListAux)
                {
                    AuxWeather? weatherDay = null;
                    weatherDay = await _tool.GetWeatherDay(weatherDay, item.Lat, item.Lon);
                    if (weatherDay != null)
                    {
                        WeatherDay wDay = new()
                        {
                            AirportId = item.Id,
                            Main = weatherDay.weather[0].main,
                            Description = weatherDay.weather[0].description,
                            Icon = weatherDay.weather[0].icon,
                            Temp = weatherDay.main.temp,
                            FeelsLike = weatherDay.main.feels_like,
                            TempMin = weatherDay.main.temp_min,
                            TempMax = weatherDay.main.temp_max,
                            Pressure = weatherDay.main.pressure,
                            Humidity = weatherDay.main.humidity,
                            Visibility = weatherDay.visibility,
                            WindSpeed = weatherDay.wind.speed
                        };
                        listW.Add(wDay);//agregar lista auxiliar de datos de clima al contexto
                    }
                }
                await _flightContext.WeatherDays.AddRangeAsync(listW);


                List<Itinerary> listItinerary = []; //lista de itinerarios 2 por cada vuelo (origen-destino)
                listItinerary = listAuxF.Select(q => q.Ticket).SelectMany(q =>
                {
                    var flightId = listFlight.First(f => f.Folio == q.Airline + q.Flight_num + q.Origin + q.Destination).Id;
                    var orID = airportListAux.First(a => a.City == q.Origin).Id;
                    var desId = airportListAux.First(a => a.City == q.Destination).Id;
                    return new Itinerary[]{
                    new()
                    {
                        FlightId = flightId,
                        AirportId = orID,
                        Journey = 0,
                        Date = dt
                    },
                    new()
                    {
                        FlightId = flightId,
                        AirportId = desId,
                        Journey = 1,
                        Date = dt
                    }
                    };
                }).ToList();//crear lista para obtener los itinerarios de cada vuelo que se ha guardado en la base de datos
                await _flightContext.Itineraries.AddRangeAsync(listItinerary); //agregar los itinerarios al contexto de la base de datos
                await _flightContext.SaveChangesAsync();//guardar en base de datos

                foreach (var item in listFlight)
                {
                    AuxFlight uxF = new();
                    uxF.Airline = item.Airline;
                    uxF.Flight_num = item.FlightNum.ToString();
                    List<AuxAirport?> Airports = new();
                    Airports.AddRange(listItinerary.Where(q => q.FlightId == item.Id).SelectMany(airp => new AuxAirport[]
                    {
                        new AuxAirport
                        {
                            Name = airp.Airport.Name,
                            City = airp.Airport.City,
                            Latitude = airp.Airport.Lat.ToString(),
                            Longitude = airp.Airport.Lon.ToString(),
                            Journey = airp.Journey,
                            Weather = airp.Airport.WeatherDays.First()},
                    }));
                    uxF.Airports.AddRange(Airports);
                    auxDtView.Flight.Add(uxF);
                }
                auxDtView.message = SetMessage(200, "OK", "FILE SAVED");
            }
            catch (Exception e)
            {
                auxDtView.message = SetMessage(500, "ERROR", "FILE NOT SAVED");
                throw;
            }
            return auxDtView;//retornar la lista Auxiliar con los datos a mostrar en la vista
        }

        public async Task<AuxDtView> GetData(FlightContext _flightContext)
        {
            AuxDtView auxDtView = new();
            try
            {
                List<Flight> flights = await _flightContext.Flights.ToListAsync();//obtener lista de vuelos
                List<Itinerary> itin = await _flightContext.Itineraries.ToListAsync();//obtener lista de intinerarios
                List<Airport> airports = await _flightContext.Airports.ToListAsync();//obtener lista de aeropuertos
                List<WeatherDay> wd = await _flightContext.WeatherDays.ToListAsync();//obtener lista de estado climático

                foreach (var item in flights)//iterar la lista de vuelos
                {
                    AuxFlight uxF = new();
                    uxF.Airline = item.Airline;
                    uxF.Flight_num = item.FlightNum.ToString();
                    List<AuxAirport?> Listirports = new();//lista auxiliar para ir agregando los datos que se mostrarán

                    Listirports.AddRange(itin.Where(q => q.FlightId == item.Id).SelectMany(airp => new AuxAirport[]
                    {
                        new()
                        {
                            Name = airports.Find(q=> q.Id == airp.AirportId)?.Name,
                            City = airports.Find(q=> q.Id == airp.AirportId)?.City,
                            Latitude = airp.Airport.Lat.ToString(),
                            Longitude = airp.Airport.Lon.ToString(),
                            Journey = airp.Journey,
                            Weather = wd.Find(q=> q.AirportId == airp.AirportId),
                        },
                    }));
                    uxF.Airports.AddRange(Listirports);
                    auxDtView.Flight.Add(uxF);
                }
                auxDtView.message = SetMessage(200, "OK", "DATA FOUND");
            }
            catch (Exception e)
            {
                auxDtView.message = SetMessage(500, "ERROR", "NO DATA");
                throw;
            }
            return auxDtView;//retornar la lista Auxiliar con los datos a mostrar en la vista
        }

        public Message SetMessage(int Code, string Status, string Description)
        {
            Message msn = new()
            {
                Code = Code,
                Status = Status,
                Description = Description
            };
            return msn;
        }
    }
}
