﻿using Microsoft.EntityFrameworkCore;
using TrainingSystem.Data.Entities;

namespace TrainingSystem.Data;

public class ForumDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public ForumDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
    }
}