using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace api_weather.Models;

public partial class FlightContext : DbContext
{
    public FlightContext()
    {
    }

    public FlightContext(DbContextOptions<FlightContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Airport> Airports { get; set; }

    public virtual DbSet<Flight> Flights { get; set; }

    public virtual DbSet<Itinerary> Itineraries { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<UserDb> UserDbs { get; set; }

    public virtual DbSet<WeatherDay> WeatherDays { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Airport__3213E83F68A4B700");
        });

        modelBuilder.Entity<Flight>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Flight__3213E83F462EA165");
        });

        modelBuilder.Entity<Itinerary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Itinerar__3213E83F438B5DE2");

            entity.HasOne(d => d.Airport).WithMany(p => p.Itineraries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__Airpo__5CD6CB2B");

            entity.HasOne(d => d.Flight).WithMany(p => p.Itineraries)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__Fligh__5DCAEF64");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ticket__3213E83FCD5E06B6");

            entity.HasOne(d => d.Flight).WithMany(p => p.Tickets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ticket__Flight_i__571DF1D5");
        });

        modelBuilder.Entity<UserDb>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User_db__3213E83F772FCDD2");
        });

        modelBuilder.Entity<WeatherDay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Weather___3213E83F28FBA603");

            entity.HasOne(d => d.Airport).WithMany(p => p.WeatherDays)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Weather_d__Airpo__59FA5E80");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
