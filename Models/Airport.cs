using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api_weather.Models;

[Table("Airport")]
public partial class Airport
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(120)]
    [Unicode(false)]
    public string? Name { get; set; }

    [Column("city")]
    [StringLength(5)]
    [Unicode(false)]
    public string? City { get; set; }

    [Column("lat")]
    public double? Lat { get; set; }

    [Column("lon")]
    public double? Lon { get; set; }

    [InverseProperty("Airport")]
    public virtual ICollection<Itinerary> Itineraries { get; set; } = new List<Itinerary>();

    [InverseProperty("Airport")]
    public virtual ICollection<WeatherDay> WeatherDays { get; set; } = new List<WeatherDay>();
}
