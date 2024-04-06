using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace api_weather.Models;

[Table("Weather_day")]
[Index("AirportId", Name = "IFK_Rel_08")]
[Index("AirportId", Name = "Weather_day_FKIndex1")]
public partial class WeatherDay
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("Airport_id")]
    public int AirportId { get; set; }

    [Column("main")]
    [StringLength(70)]
    [Unicode(false)]
    public string? Main { get; set; }

    [Column("description")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Description { get; set; }

    [Column("icon")]
    [StringLength(8)]
    [Unicode(false)]
    public string? Icon { get; set; }

    [Column("temp")]
    public double? Temp { get; set; }

    [Column("feels_like")]
    public double? FeelsLike { get; set; }

    [Column("temp_min")]
    public double? TempMin { get; set; }

    [Column("temp_max")]
    public double? TempMax { get; set; }

    [Column("pressure")]
    public double? Pressure { get; set; }

    [Column("humidity")]
    public double? Humidity { get; set; }

    [Column("visibility")]
    public double? Visibility { get; set; }

    [Column("wind_speed")]
    public double? WindSpeed { get; set; }

    [ForeignKey("AirportId")]
    [InverseProperty("WeatherDays")]
    [JsonIgnore]
    public virtual Airport Airport { get; set; } = null!;
}
