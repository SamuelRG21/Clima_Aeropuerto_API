using Microsoft.EntityFrameworkCore;
using api_weather.Models;
using api_weather.Tools;
using System.Net.Http.Headers;
using api_weather.Dao;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<FlightContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnetion")));
builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddHttpClient("ApiWeather", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiWeather:URL"]);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddSingleton<Tool>();
builder.Services.AddSingleton<DaoAux>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
