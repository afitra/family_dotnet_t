using System.Text;
using familyMart.Connection;
using familyMart.Interface;
using familyMart.Master;
using familyMart.Repository;
using familyMart.Usecase;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SiloamApi Dong", Version = "v1" });
    c.EnableAnnotations();
    c.MapType<string>(() => new OpenApiSchema { Type = "string" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter the token in the field provided.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = string.Empty
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


// start add costom logging    
builder.Services.AddHttpLogging(logging =>
{
    // Customize HTTP logging here.
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("my-response-header");
    logging.MediaTypeOptions.AddText("application/json");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/miniApiLog.log", rollingInterval: RollingInterval.Day, encoding: Encoding.UTF8)
    .CreateLogger();


builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


builder.Services.AddSingleton<BasicLogger>();
builder.Services.AddSingleton(new BasicConfiguration());

// start  add config db nya
builder
    .Services
    .AddDbContext<BasicDatabase>(
        options =>
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("LocalConnectionString"))

        // .LogTo(Console.WriteLine, LogLevel.Information)
        // untuk menampilkan log query ke db optional
    );
// end  add config db nya
// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IEmployeeUsecase, EmployeeUsecase>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAbsenRepository, AbsenRepository>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1"); });
}

app.UseHttpsRedirection();


// start add config auto logger request - response
app.UseMiddleware<LoggingMiddleware>();
// end add config auto logger request - response

app.UseCors(options =>
{
    options.AllowAnyOrigin();
    options.AllowAnyMethod();
    options.AllowAnyHeader();
});


app.MapControllers();

app.Run();