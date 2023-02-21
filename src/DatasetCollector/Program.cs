using Coravel;
using DatasetCollector.DataBases;
using DatasetCollector.Parsers;
using DatasetCollector.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IParser, OpenDotaParser>();

builder.Services.AddDbContext<AppDbContext>(o => 
    o.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScheduler();
builder.Services.AddTransient<DataCollector>();
// builder.Services.AddHostedService<CollectionService>();

var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    var jobSchedule = scheduler.Schedule<DataCollector>();
    jobSchedule.Daily();
});

app.Run();
