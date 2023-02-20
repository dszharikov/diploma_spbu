using DatasetCollector.DataBases;
using Microsoft.EntityFrameworkCore;

namespace DatasetCollector;

public class MatchContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        
        return context;
    }

    public static void Destroy(AppDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}