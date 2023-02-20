using DatasetCollector.Models;
using Microsoft.EntityFrameworkCore;

namespace DatasetCollector.DataBases;

public sealed class AppDbContext : DbContext
{
    public DbSet<Match> Matches { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}