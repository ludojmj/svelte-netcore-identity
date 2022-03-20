using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.OpenApi.Models;
using Server.DbModels;
using Server.Service;
using Server.Service.Interfaces;
using Server.Shared;

namespace Server
{
    public class Startup
    {
        private readonly IConfiguration _conf;
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration conf, IWebHostEnvironment env)
        {
            _conf = conf;
            _env = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();
            services.AddControllers();
            services.AddCors();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddLogging(builder =>
            {
                builder.AddApplicationInsights(_conf["APPINSIGHTS_INSTRUMENTATIONKEY"]);
                builder.AddFilter<ApplicationInsightsLoggerProvider>("", LogLevel.Trace);
                builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Error);
            });
            services.AddApplicationInsightsTelemetry();

            // Add DB
            services.AddDbContext<StuffDbContext>(options => options.UseSqlite(
                _conf.GetConnectionString("SqlConnectionString"),
                sqlServerOptions => sqlServerOptions.CommandTimeout(_conf.GetSection("ConnectionStrings:SqlCommandTimeout").Get<int>()))
            );

            // Add Authent
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = _conf["JwtToken:Authority"];
                    options.Audience = _conf["JwtToken:Audience"];
                });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(TraceHandlerFilterAttribute));
                options.Filters.Add(typeof(ModelValidationFilterAttribute));
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {   // Managed by Shared/ModelValidationFilter.cs
                options.SuppressModelStateInvalidFilter = true;
            });

            var now = DateTime.Now;
            services.AddHsts(configureOptions =>
            {
                configureOptions.Preload = true;
                configureOptions.IncludeSubDomains = true;
                configureOptions.MaxAge = now.AddYears(1) - now;
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = _conf.GetSection("https_port").Get<int>();
            });

            // Register the Swagger generator
            services.AddSwaggerGen(c =>
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

            // Register Service
            services.AddScoped<IUserAuthService, UserAuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStuffService, StuffService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // https://docs.microsoft.com/fr-fr/aspnet/core/fundamentals/middleware/?view=aspnetcore-6.0
        public void Configure(IApplicationBuilder app)
        {
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
        }
    }
}
