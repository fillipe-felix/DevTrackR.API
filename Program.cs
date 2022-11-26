using System.Reflection;
using System.Text.Json.Serialization;

using DevTrackR.API.Persistence;
using DevTrackR.API.Repository;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DevTrackeRCs");
builder.Services
    .AddDbContext<DevTrackRContext>(o => o.UseSqlServer(connectionString));

//builder.Services.AddDbContext<DevTrackRContext>(o => o.UseInMemoryDatabase("DevTrackeR"));

builder.Services.AddScoped<IPackageRepository, PackageRepository>();

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "API de TrackR",
            Version = "v1",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact()
            {
                Name = "Fillipe Félix"

            }
        });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseReDoc(c =>
    {
        c.DocumentTitle = "REDOC API Documentation";
        c.SpecUrl = "/swagger/v1/swagger.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
