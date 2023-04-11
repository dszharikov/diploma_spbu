using System.Globalization;
using Coravel;
using DatasetCollector.DataBases;
using DatasetCollector.Parsers;
using DatasetCollector.Services;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<DatasetCollector.Parsers.IParser, OpenDotaParser>();

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

System.Console.WriteLine("---> Dataset Collector was instantiated");

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScheduler();
builder.Services.AddTransient<DataCollector>();

var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    var jobSchedule = scheduler.Schedule<DataCollector>();
    jobSchedule.Daily().RunOnceAtStart();
});

app.MapGet("/example", (AppDbContext context) =>
{
    var matches = context.Matches.Take(25);
    var csv = CsvSerializer.SerializeToCsv(matches);
    return csv;
});

// app.MapGet("/download", async (AppDbContext context) => 
// {
//     var fileName = DateTime.Now.ToString("dd-MM-yyyy-hh") + ".csv";
//     var directoryPath = "/collector/";
//     var fullPath = directoryPath + fileName;
    
//     if (File.Exists(fullPath))
//     {
//         return Results.File(fullPath, "application/octet-stream", "Matches.csv");
//     }

//     var di = new DirectoryInfo(directoryPath);
//     foreach (var file in di.GetFiles())
//     {
//         file.Delete(); 
//     }
        
//     using (var writer = new StreamWriter(fullPath))
//     using (var csvWriter = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
//     {
//         await csvWriter.WriteRecordsAsync(context.Matches);
//     }
//     return Results.File(fullPath, "application/octet-stream", "Matches.csv");
// });

app.MapGet("/info", (AppDbContext context) =>
{
    var matchesNumber = context.Matches.Count();
    var averageMMR = context.Matches.Select(m => m.AverageMMR).Average();
    return $"Total count of matches = {matchesNumber}, averageMMR = {averageMMR}";
});

Console.WriteLine($"The app is started: {DateTime.Now}");

app.Run();