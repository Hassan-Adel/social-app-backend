using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SocialApp.API.Data;
using SocialApp.API.Helpers;
using SocialApp.API.Models;

namespace SocialApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddDbContext<ApplicationDBContext>(x => x
        //    .UseMySql(Configuration.GetConnectionString("DefaultConnection"))
        //    .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.IncludeIgnoredWarning)));

        //    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
        //        .AddJsonOptions(options =>
        //        {
        //            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        //        });

        //    services.AddScoped<IAuthRepository, AuthRepository>();
        //    services.AddScoped<ISocialRepository, SocialRepository>();
        //    services.AddCors();
        //    //Getthe section cloudinary settings and now the values for the properties
        //    //inside the cloudinary settings are going to match what's inside our appSettings.Json File 
        //    services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
        //    services.AddAutoMapper();
        //    services.AddTransient<Seed>();
        //    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
        //        options =>
        //        {
        //            options.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                ValidateIssuerSigningKey = true,
        //                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
        //                ValidateIssuer = false,
        //                ValidateAudience = false
        //            };
        //        });
        //    // AddScoped: Because we need an instance of this per request
        //    services.AddScoped<LogUserActivity>();
        //}

        // This method gets called In Development mode by default. Use this method to add services to the container.
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDBContext>(x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));


            //this configuration is based on the facts that I want to be able to query the users
            //and pull back all of their roles at the same time as well.
            IdentityBuilder builder = services.AddIdentityCore<User>(opt =>
           {
               opt.Password.RequireDigit = false;
               opt.Password.RequiredLength = 4;
               opt.Password.RequireNonAlphanumeric = false;
               opt.Password.RequireUppercase = false;
           });

            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<ApplicationDBContext>();
            builder.AddRoleValidator<RoleValidator<Role>>();
            builder.AddRoleManager<RoleManager<Role>>();
            builder.AddSignInManager<SignInManager<User>>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            //this is going to require authentication globally
            //it means I can remove all of the authorize attributes from the controllers.
            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddScoped<ISocialRepository, SocialRepository>();
            services.AddCors();
            //Getthe section cloudinary settings and now the values for the properties
            //inside the cloudinary settings are going to match what's inside our appSettings.Json File 
            services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
            services.AddAutoMapper();
            services.AddTransient<Seed>();
            // AddScoped: Because we need an instance of this per request
            services.AddScoped<LogUserActivity>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, Seed seeder)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async httpContext =>
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = httpContext.Features.Get<IExceptionHandlerFeature>();
                        if(error != null)
                        {
                            httpContext.Response.AddApplicationError(error.Error.Message);
                            await httpContext.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                //app.UseHsts();
            }
            seeder.SeedUsers();
            app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseDefaultFiles(); // to look for index.html
            app.UseStaticFiles(); // will look inside the wwwroot folder
            app.UseMvc(routes =>
            {
                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new {controller = "Fallback", action = "index"}
                    );
            });
        }
    }
}
