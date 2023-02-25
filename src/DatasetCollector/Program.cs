using System.Net;
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

app.MapGet("/download", (AppDbContext context) => 
{
    string directoryPath = "/collector/";
    var maxMatchId = context.Matches.Any() ? context.Matches.Max(match => match.MatchId).ToString() : "0";
    var fileName = maxMatchId + ".csv";
    var fullPath = directoryPath + fileName;
    
    if (!File.Exists(fullPath))
    {
        var di = new DirectoryInfo(directoryPath);
        foreach (var file in di.GetFiles())
        {
            file.Delete(); 
        }

        var matches = context.Matches;
        var csv = CsvSerializer.SerializeToCsv(matches);
        Console.WriteLine(csv);
        File.WriteAllText(fullPath, csv);
    }

    return Results.File(File.ReadAllBytes(fullPath), "application/octet-stream", "Matches.csv");
});

app.MapGet("/info", (AppDbContext context) =>
{
    var matchesNumber = context.Matches.Count();
    var averageMMR = context.Matches.Select(m => m.AverageMMR).Average();
    return $"Total count of matches = {matchesNumber}, averageMMR = {averageMMR}";
});

Console.WriteLine($"The app is started: {DateTime.Now}");

app.Run();
