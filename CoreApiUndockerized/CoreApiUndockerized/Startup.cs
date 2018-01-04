using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CoreApiUndockerized.Data;
using CoreApiUndockerized.Data.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CoreApiUndockerized
{
    public class Startup
    {

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _env = env;
            _config = builder.Build();
        }

        private IHostingEnvironment _env;
        IConfigurationRoot _config { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_config);

            #region Repository and DB
            services.AddDbContext<ProductContext>(ServiceLifetime.Scoped);
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddTransient<ProductDbInitializer>();
            services.AddTransient<ProductIdentityInitializer>();
            #endregion

            #region SSL
            services.AddMvc(opt =>
            {
                if (!_env.IsProduction())
                {
                    opt.SslPort = 44388;
                }
                opt.Filters.Add(new RequireHttpsAttribute()); // For enabling https.
            });
            #endregion

            #region Identity and Cookie
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ProductContext>();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 401;
                        }

                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 403;
                        }

                        return Task.CompletedTask;
                    }
                };
            });
            #endregion

            #region AutoMap
            services.AddAutoMapper();
            #endregion

            #region CORS
            // I could do this inside UseCors too but this way I defined a policy for that. This provides granularity.
            services.AddCors(cfg =>
            {
                cfg.AddPolicy("Ensar", bldr =>
                {
                    bldr.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();

                    //.WithOrigins("http://hurriyet.com.tr") <- This tells anyone can access only from hurriyet.com.tr to my API.
                });

                cfg.AddPolicy("AnyGET", bldr =>
                {
                    bldr.AllowAnyHeader()
                        .WithMethods("GET")
                        .AllowAnyOrigin();
                });
            });
            #endregion

            #region JWT Middleware

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = _config["Tokens:Audience"];
                    options.ClaimsIssuer = _config["Tokens:Issuer"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = _config["Tokens:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
                    };
                });

            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ProductDbInitializer seeder, ProductIdentityInitializer identitySeeder, ILoggerFactory loggerFactory)
        {
            #region Logger
            loggerFactory.AddConsole(_config.GetSection("Logging"));
            loggerFactory.AddDebug(); 
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentity();
            app.UseMvc().UseAuthentication();
            
            #region CORS Commented Out
            //app.UseCors(cfg =>
            //{
            //    cfg.AllowAnyHeader()
            //    .AllowAnyMethod()
            //    .AllowAnyOrigin();

            //    //.WithOrigins("http://hurriyet.com.tr") <- This tells anyone can access from hurriyet.com.tr to my API.
            //});
            #endregion

            seeder.Seed().Wait();
            //identitySeeder.Seed().Wait(); // Comment this after first initial seed.

        }
    }
}
