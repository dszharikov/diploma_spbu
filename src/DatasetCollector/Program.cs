using Coravel;
using DatasetCollector.DataBases;
using DatasetCollector.Parsers;
using DatasetCollector.Services;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IParser, OpenDotaParser>();

if (builder.Environment.IsProduction())
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    builder.Services.AddDbContext<AppDbContext>(o => 
        o.UseNpgsql(connectionString));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        options.UseInMemoryDatabase(Guid.NewGuid().ToString());
    });
}


builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScheduler();
builder.Services.AddTransient<DataCollector>();

var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    var jobSchedule = scheduler.Schedule<DataCollector>();
    jobSchedule.Daily();
});

app.MapGet("/csv", (AppDbContext context) =>
{
    var matches = context.Matches;
    var csv = CsvSerializer.SerializeToCsv(matches);
    return csv;
});

Console.WriteLine($"The app is started: {DateTime.Now}");

app.Run();
