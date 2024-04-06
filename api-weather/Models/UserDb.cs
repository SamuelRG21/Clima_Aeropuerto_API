using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace api_weather.Models;

[Table("User_db")]
public partial class UserDb
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Username { get; set; }

    [Column("password_hash")]
    [StringLength(30)]
    [Unicode(false)]
    public string? PasswordHash { get; set; }

    [Column("auth_key")]
    [StringLength(50)]
    [Unicode(false)]
    public string? AuthKey { get; set; }

    [Column("status_us")]
    public short? StatusUs { get; set; }
}
