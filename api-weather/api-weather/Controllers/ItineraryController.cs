using api_weather.Auxmodels;
using api_weather.Dao;
using api_weather.Models;
using api_weather.Tools;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
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
                using (var transac = await _flightContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        auxDtView = await _daoAux.ReadFile(file, _flightContext, auxDtView);//enviar archivo y contexto para realizar guardado en base de datos
                        await transac.CommitAsync(); // Confirmar la transacción
                    }
                    catch (Exception)
                    {
                        await transac.RollbackAsync(); //revertir transacción
                        auxDtView.message = _daoAux.SetMessage(500, "INTERNAL SERVER", "ERROR AL GUARDAR DATOS");
                    }
                }                               
            }
            else {
                auxDtView.message = _daoAux.SetMessage(404, "NOT FOUND","FILE EMPTY");
            }
            return StatusCode(auxDtView.message.Code, auxDtView);
        }

        [HttpGet]
        public async Task<ObjectResult> Get()
        {
            AuxDtView auxDtView = await _daoAux.GetData(_flightContext);
            return StatusCode(auxDtView.message.Code, auxDtView);
        }
    }
}
