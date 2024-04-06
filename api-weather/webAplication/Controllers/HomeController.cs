using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using webAplication.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace webAplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _HttpClient;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _HttpClient = new HttpClient();
            _HttpClient.BaseAddress = new Uri("https://localhost:7234/api/");
        }
        [HttpGet]
        public async Task<IActionResult> Index(AuxDtView? dts)
        {
            AuxDtView? dt = new AuxDtView();
            HttpResponseMessage response = null;
            try
            {
                response = await _HttpClient.GetAsync("itinerary");
            }
            catch (Exception e)
            {
                return View(dt);
                throw;
            }
            if (response.StatusCode != HttpStatusCode.InternalServerError)
            {
                dt = JsonConvert.DeserializeObject<AuxDtView>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                dt.message.Code = 500;
                dt.message.Description = "Several internal error";
                dt.message.Status = "Error";
            }
            return View(dt);
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            AuxDtView? dt = new AuxDtView();
            HttpResponseMessage response = null;
            //ContentType en esta caso es multpart ya que se enviarán archivos
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            //control de flujos para leer el archivo y mandarlo al servicio web
            BinaryReader br = new BinaryReader(file.OpenReadStream());
            byte[] data = br.ReadBytes((int)file.OpenReadStream().Length);
            ByteArrayContent bytes = new ByteArrayContent(data);
            bytes.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            //agregar al multicontent los bytes que contiene el archivo y su nombre
            multiContent.Add(bytes, "file", file.FileName);
            response = await _HttpClient.PostAsync("itinerary", multiContent);

            if (response.StatusCode != HttpStatusCode.InternalServerError)
            {
                dt = JsonConvert.DeserializeObject<AuxDtView>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                dt.message.Code = 500;
                dt.message.Description = "Several internal error";
                dt.message.Status = "Error";
            }
            return Json(new { status = "success", data = dt.message, flights = dt.Flight });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
