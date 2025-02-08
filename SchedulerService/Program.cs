using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchedulerService.Database;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Response;
using SchedulerService.Middlewares;
using SchedulerService.Repositories;
using SchedulerService.Services;
using SchedulerService.Services.Interfaces;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

var services = builder.Services;

var connectionString = builder.Configuration.GetValue<string>("Database:ConnectionString");
var poolSize = builder.Configuration.GetValue<int>("Database:PoolSize");

services.AddDbContextPool<SchedulerContext>(options =>
{
    options
        .UseNpgsql(connectionString)
        /*.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))*/;
}, poolSize);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey
            (Convert.FromBase64String(builder.Configuration["Jwt:Key"])),
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
    };
});
builder.Services.AddAuthorization();

services.AddScopedServices();
services.AddScoped(typeof(ICrudRepository<>), typeof(RepositoryBase<>));
services.AddScoped<INotificationPublisher, NotificationPublisher>();
services.AddScoped<IAmazonSQS>(_ =>
    new AmazonSQSClient(builder.Configuration["Sqs:AccessKey"], builder.Configuration["Sqs:SecretKey"], RegionEndpoint.EUCentral1));

services.AddDateOnlyTimeOnlyStringConverters();

services.AddFluentValidationAutoValidation();
services.AddValidatorsFromAssemblyContaining<Program>();

services
    .AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = 
            c => new BadRequestObjectResult(ApiResponse.ValidationFailed(c.ModelState));
    })
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.UseDateOnlyTimeOnlyStringConverters();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "SchedulerService", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowAnyHeader();
        
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSerilogRequestLogging();

// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();