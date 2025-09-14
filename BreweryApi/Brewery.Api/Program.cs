using Brewery.Api.Middleware;
using Brewery.Application.Interfaces;
using Brewery.Application.Services;
using Brewery.Persistence.Clients;
using Brewery.Persistence.Repositories;
using Microsoft.OpenApi.Models;
//using Api.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;  // Changed this line
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Brewery API", 
        Version = "v1",
        Description = "API for brewery information with search, filtering, and sorting capabilities"
    });
    
    
    var xmlFile = Path.Combine(AppContext.BaseDirectory, "Brewery.Api.xml");
    if (File.Exists(xmlFile))
    {
        c.IncludeXmlComments(xmlFile);
    }
});


builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version"),
        new MediaTypeApiVersionReader("ver")
    );
}).AddVersionedApiExplorer(setup =>  // Changed to AddVersionedApiExplorer
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});


builder.Services.AddMemoryCache();


builder.Services.AddHttpClient<IOpenBreweryApiClient, OpenBreweryApiClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openbrewerydb.org/v1/");
    client.DefaultRequestHeaders.Add("User-Agent", "BreweryAPI/1.0");
});

builder.Services.AddScoped<IBreweryRepository, BreweryRepository>();
builder.Services.AddScoped<IBreweryService, BreweryService>();

builder.Services.AddDbContext<BreweryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Brewery API v1");
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();