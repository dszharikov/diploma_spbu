using DatasetCollector.DataBases;
using DatasetCollector.Parsers;
using DatasetCollector.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IParser, OpenDotaParser>();

builder.Services.AddDbContext<AppDbContext>(o => 
    o.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddHostedService<CollectionService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
