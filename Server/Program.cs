using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Server.DbModels;
using Server.Shared;
using Server.Services.Interfaces;
using Server.Services;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager _conf = builder.Configuration;
IWebHostEnvironment _env = builder.Environment;

// Add services to the container.
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.AddCors();
builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging(builder =>
{
    builder.AddApplicationInsights(_conf["APPINSIGHTS_INSTRUMENTATIONKEY"]);
    builder.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Trace);
    builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Error);
});
builder.Services.AddApplicationInsightsTelemetry();

// Add DB
builder.Services.AddDbContext<StuffDbContext>(options => options.UseSqlite(
    _conf.GetConnectionString("SqlConnectionString"),
    sqlServerOptions => sqlServerOptions.CommandTimeout(_conf.GetSection("ConnectionStrings:SqlCommandTimeout").Get<int>()))
);

// Add Authent
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = _conf["JwtToken:Authority"];
        options.Audience = _conf["JwtToken:Audience"];
    });

builder.Services.AddMvc(options =>
{
    options.Filters.Add(typeof(TraceHandlerFilterAttribute));
    options.Filters.Add(typeof(ModelValidationFilterAttribute));
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{   // Managed by Shared/ModelValidationFilter.cs
    options.SuppressModelStateInvalidFilter = true;
});

var now = DateTime.Now;
builder.Services.AddHsts(configureOptions =>
{
    configureOptions.Preload = true;
    configureOptions.IncludeSubDomains = true;
    configureOptions.MaxAge = now.AddYears(1) - now;
});

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
    options.HttpsPort = _conf.GetSection("https_port").Get<int>();
});

// Register the Swagger generator
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Server", Version = "v1" });
    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[] {}
                    }
    });
});

// Register Services
builder.Services.AddScoped<IUserAuthService, UserAuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStuffService, StuffService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/api/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseFileServer(new FileServerOptions
{
    EnableDirectoryBrowsing = false,
    EnableDefaultFiles = true,
    DefaultFilesOptions = { DefaultFileNames = { "index.html" } }
});
if (!_env.IsProduction())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" } };
        });
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Server V1");
        c.DisplayRequestDuration();
        c.EnableTryItOutByDefault();
    });
}

app.UseRouting();
if (_env.IsDevelopment())
{
    app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}
else
{
    var corsList = _conf.GetSection("AuthCors").Get<string[]>();
    app.UseCors(builder => builder
        .WithOrigins(corsList)
        .AllowAnyMethod()
        .AllowAnyHeader()
    );
}

app.UseAuthentication();
app.UseAuthorization();
app.UseSecurity();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().RequireAuthorization();
    endpoints.MapFallbackToFile("/index.html");
    endpoints.MapHealthChecks("/health");
});

app.Run();
