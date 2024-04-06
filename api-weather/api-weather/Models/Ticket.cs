using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api_weather.Models;

[Table("Ticket")]
[Index("FlightId", Name = "IFK_Rel_07")]
[Index("FlightId", Name = "Ticket_FKIndex1")]
public partial class Ticket
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("Flight_id")]
    public int FlightId { get; set; }

    [ForeignKey("FlightId")]
    [InverseProperty("Tickets")]
    public virtual Flight Flight { get; set; } = null!;
}
