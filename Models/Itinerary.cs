using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api_weather.Models;

[Table("Itinerary")]
[Index("FlightId", Name = "IFK_Rel_06")]
[Index("AirportId", Name = "IFK_Rel_09")]
[Index("AirportId", Name = "Itinerary_FKIndex1")]
[Index("FlightId", Name = "Itinerary_FKIndex2")]
public partial class Itinerary
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("Flight_id")]
    public int FlightId { get; set; }

    [Column("Airport_id")]
    public int AirportId { get; set; }

    [Column("journey")]
    public short? Journey { get; set; }

    [Column("date")]
    [Precision(0)]
    public DateTime? Date { get; set; }

    [ForeignKey("AirportId")]
    [InverseProperty("Itineraries")]
    public virtual Airport Airport { get; set; } = null!;

    [ForeignKey("FlightId")]
    [InverseProperty("Itineraries")]
    public virtual Flight Flight { get; set; } = null!;
}
