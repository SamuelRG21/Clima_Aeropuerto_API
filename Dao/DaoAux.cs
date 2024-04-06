using api_weather.Auxmodels;
using api_weather.Models;
using api_weather.Tools;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.ComponentModel;

namespace api_weather.Dao
{
    public class DaoAux
    {
        private readonly Tool _tool;
        public DaoAux(Tool tool)
        {
            _tool = tool;
        }
        public async Task<AuxDtView> ReadFile(IFormFile file, FlightContext _flightContext)
        {
            DateTime dt = DateTime.Now;
            AuxDtView auxDtView = new(); //lista auxiliar de vuelos para enviar a los clientes
            Message message = new(); //mensaje para la respuesta
            var content = new StreamReader(file.OpenReadStream()); //leer el archivo CSV
            var line = content.ReadLine(); //Saltar la primera fila que contiene los encabezados
            List<TicketsAux> listAuxt = new();//lista auxiliar donde se van a cargar los datos del CSV
            List<Airport?> airportListAux = new(); //lista auxiliar para cargar los datos de aeropuertos
            List<Flight> listFlight = new(); //lista auxiliar para agrupar datos de los vuelos
            List<Itinerary> listItinerary = new(); //lista auxiliar para agrupar datos de los intinerarios
            try
            {
                while ((line = content?.ReadLine()) != null) //iterar las filas del archivo CSV
            {
                var auxln = line.Split(';'); //obtener arreglo de elementos
                listAuxt.Add(new TicketsAux() //agregar arreglo a la lista auxiliar con los datos del CSV (3000)
                {
                    Origin = auxln[0],
                    Destination = auxln[1],
                    Airline = auxln[2],
                    Flight_num = auxln[3],
                    Origin_name = auxln[5],
                    Origin_latitude = auxln[6],
                    Origin_longitude = auxln[7],
                    Destination_name = auxln[9],
                    Destination_latitude = auxln[10],
                    Destination_longitude = auxln[11]
                });

                List<Airport> airportsAu = new(); //crear lista de aeropuertos y agregar el aeropuerto de origen y el destino
                airportsAu.AddRange([
                    new Airport {
                        Name = auxln[5],
                        City = auxln[0],
                        Lat = float.Parse(auxln[6]),
                        Lon = float.Parse(auxln[7]),},
                    new Airport{
                        Name = auxln[9],
                        City = auxln[1],
                        Lat = float.Parse(auxln[10]),
                        Lon = float.Parse(auxln[11]),
                    }]);
                airportListAux = FilterDtAirport(airportListAux, airportsAu); //En caso de que los aeropuertos no existan en la lista auxiliar se agregan

                Flight flightAu; //crear vuelo
                flightAu = new() //agregar al vuelo los datos de aerolinea, numero de vuelo y un folio (existen casos donde el vuelo tiene el mismo numero y aerolinea pero diferente origen y destino)
                {
                    Airline = auxln[2],
                    FlightNum = int.Parse(auxln[3]),
                    Folio = auxln[2] + auxln[3] + auxln[0] + auxln[1],
                };

                listFlight = FilterDtFlight(listFlight, flightAu); //Agregar el vuelo a la lista en caso de que no exista en esta
            }

            _flightContext.Airports.AddRange(airportListAux);  //Agregar a contexto en base de datos la lista de aeropuertos
            _flightContext.Flights.AddRange(listFlight); //Agregar a contexto en base de datos la lista de vuelos
            _flightContext.SaveChanges(); //guardar cambios

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
            _flightContext.WeatherDays.AddRange(listW);
            //_flightContext.SaveChanges();

            //crear lista auxiliar para obtener el id de los vuelos ya guardados por cada boleto y guardarlos en base de datos
            var ticketsFlAux = listAuxt.Select(t => new {
                id = listFlight.Where(q => q.Folio == t.Airline + t.Flight_num + t.Origin + t.Destination).FirstOrDefault().Id });

            _flightContext.Tickets.AddRange(ticketsFlAux.Select(q => new Ticket {
                FlightId = q.id
            })); //agregar el Id de cada vuelo a la base de datos, representando cada uno de los 3000 boletos

            _flightContext.SaveChanges();

            List<Itinerary> itAux = new List<Itinerary>();
            itAux.AddRange(listAuxt.GroupBy(q => new { q.Airline, q.Flight_num, q.Origin, q.Destination })
                .SelectMany(group => new Itinerary[]{
                    new Itinerary
                    {
                        FlightId = listFlight.FirstOrDefault(q => q.Folio == group.Key.Airline + group.Key.Flight_num + group.Key.Origin + group.Key.Destination).Id,
                        AirportId = airportListAux.FirstOrDefault(q => q.City == group.Key.Origin).Id,
                        Journey = 0,
                        Date = dt
                    },
                    new Itinerary
                    {
                        FlightId = listFlight.FirstOrDefault(q => q.Folio == group.Key.Airline + group.Key.Flight_num + group.Key.Origin + group.Key.Destination).Id,
                        AirportId = airportListAux.FirstOrDefault(q => q.City == group.Key.Destination).Id,
                        Journey = 1,
                        Date = dt
                    }
                }));//crear lista auxiliar para obtener los itinerarios de cada vuelo que se ha guardado en la base de datos

            _flightContext.Itineraries.AddRange(itAux);
            _flightContext.SaveChanges();

                
                 foreach (var item in listFlight)
                 {
                    AuxFlight uxF = new();
                     uxF.Airline = item.Airline;
                     uxF.Flight_num = item.FlightNum.ToString();
                    List<AuxAirport?> Airports = new();
                    Airports.AddRange(itAux.Where(q => q.FlightId == item.Id).SelectMany(airp => new AuxAirport[]
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
            List<Flight?> flights = _flightContext.Flights.ToList();
            List<Itinerary?> itin = _flightContext.Itineraries.ToList();
            List<Airport?> airports = _flightContext.Airports.ToList();
            List<WeatherDay?> wd = _flightContext.WeatherDays.ToList();

            foreach (var item in flights)
            {
                AuxFlight uxF = new();
                uxF.Airline = item.Airline;
                uxF.Flight_num = item.FlightNum.ToString();
                List<AuxAirport?> Airports = new();
                try {
                Airports.AddRange(itin.Where(q => q.FlightId == item.Id).SelectMany(airp => new AuxAirport[]
                {
                        new AuxAirport
                        {
                            Name = airports.Find(q=> q.Id == airp.AirportId).Name,
                            City = airports.Find(q=> q.Id == airp.AirportId).City,
                            Latitude = airp.Airport.Lat.ToString(),
                            Longitude = airp.Airport.Lon.ToString(),
                            Journey = airp.Journey,
                            Weather = wd.Find(q=> q.AirportId == airp.AirportId),
                        },
                }));
                uxF.Airports.AddRange(Airports);
                auxDtView.Flight.Add(uxF);
                    auxDtView.message = SetMessage(200, "OK", "DATA FOUND");
                }
                catch (Exception e)
                {
                    auxDtView.message = SetMessage(500, "ERROR", "FILE NOT SAVED");
                    throw;
                }
            }
            return auxDtView;//retornar la lista Auxiliar con los datos a mostrar en la vista
        }
        public List<Airport?> FilterDtAirport(List<Airport?> dtAirport, List<Airport?> airport)
        {
            foreach (var item in airport)
            {
                if (dtAirport.Where(q => q?.City == item?.City && q.Name == item?.Name).Count() == 0)
                {
                    dtAirport.Add(item);
                }
            }
            return dtAirport;
        }

        public List<Flight?> FilterDtFlight(List<Flight?> dtFlight, Flight? ticket)
        {
            if (dtFlight.Where(q => q?.Folio == ticket.Folio).Count() == 0)
            {
                dtFlight.Add(ticket);
            }
            return dtFlight;
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
