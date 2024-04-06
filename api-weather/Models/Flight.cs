using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api_weather.Models;

[Table("Flight")]
public partial class Flight
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("flight_num")]
    public int? FlightNum { get; set; }

    [Column("airline")]
    [StringLength(15)]
    [Unicode(false)]
    public string? Airline { get; set; }

    [Column("folio")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Folio { get; set; }

    [InverseProperty("Flight")]
    public virtual ICollection<Itinerary> Itineraries { get; set; } = new List<Itinerary>();

    [InverseProperty("Flight")]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
