using api_weather.Auxmodels;
using api_weather.Dao;
using api_weather.Models;
using api_weather.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace api_weather.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItineraryController : ControllerBase
    {
        private readonly FlightContext _flightContext;
        private readonly DaoAux _daoAux;

        public ItineraryController(FlightContext context, DaoAux dao)
        {
            _flightContext = context;
            _daoAux = dao ;
        }

        [HttpPost]
        public async Task<ObjectResult> Post([FromForm(Name = "file")] IFormFile file) //obtener archivo CSV en el servicio
        {
            AuxDtView auxDtView = new();
            if (file != null && file.Length > 0)//Verificar el archivo
            {
                _flightContext.Database.ExecuteSqlRaw("dbo.resetDataSet");//ejecutar store procedure para descartar datos del archivo anterior
                auxDtView = await _daoAux.ReadFile(file, _flightContext);//enviar archivo y contexto para realizar guardado en base de datos
            }
            else {
                auxDtView.message = _daoAux.SetMessage(404, "NOT FOUND","FILE EMPTY");
            }
            return StatusCode(auxDtView.message.Code, auxDtView);
        }

        [HttpGet]
        public async Task<ObjectResult> Get()
        {
            AuxDtView auxDtView = new();
            auxDtView = await _daoAux.GetData(_flightContext);//obtener los climas de los vuelos guardados en la base de datos
            return StatusCode(auxDtView.message.Code, auxDtView);
        }
    }
}
