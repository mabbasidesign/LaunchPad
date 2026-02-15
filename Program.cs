using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using LaunchPad.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Serilog;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"C:/LaunchpadLog/{DateTime.Now:dddd}/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 30)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
// Add response compression
builder.Services.AddResponseCompression();
builder.Host.UseSerilog();

// Add JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "LaunchPadAPI",
            ValidAudience = "LaunchPadUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSuperSecretKey1234567890!@#$%^"))
        };
    });

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add Redis distributed cache
// Use in-memory cache for local development
builder.Services.AddDistributedMemoryCache();

// Add rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10, // 10 requests
                Window = TimeSpan.FromSeconds(10), // per 10 seconds
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            }
        ));
    options.RejectionStatusCode = 429;
});

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<LaunchPad.Services.IBookRepository, LaunchPad.Services.BookRepository>();

// Register user and auth services
builder.Services.AddScoped<LaunchPad.Services.IUserService, LaunchPad.Services.UserService>();
builder.Services.AddScoped<LaunchPad.Services.IAuthService, LaunchPad.Services.AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Authorization: Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();
// Use response compression middleware
app.UseResponseCompression();

// Use rate limiting middleware
app.UseRateLimiter();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
