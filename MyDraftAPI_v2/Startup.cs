﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyDraftAPI_v2.Middleware;
using DbData;

namespace MyDraftAPI_v2
{
    public class Startup
    {
        private static string[] settingsFile = { "appsettings.json" };

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public class CorsConfiguration
        {
            public string? corsPoly { get; set; }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => Configuration);

            services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

            services.AddSingleton<DraftEngine_v2, DraftEngine_v2>();

            services.AddHostedService<PostStartup>();

            //services.AddAuthorization();
            services.AddDbContext<AppDataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            });

            services.Configure<CorsConfiguration>(Configuration.GetSection("corsConfiguration"));
            var corsPoly = Configuration.GetValue<string>("corsConfiguration:corsPoly").ToString().Split('|');
            //var cors = Configuration["CORS"].ToString().Split('|');

            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy",
                    builder =>
                    {
                        builder
                        .WithOrigins(corsPoly)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {

            //if (env.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Our Custom middleware, which will used for handling the http requests and validation etc
            //app.UseCustomMiddleware(Configuration);

            app.UseHttpsRedirection();

            app.UseCors("MyPolicy");

            app.UseRouting();

            //app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //var builder = new ConfigurationBuilder();
            //builder.SetBasePath(env.ContentRootPath);
            //foreach (var file in settingsFile)
            //{
            //    builder.AddJsonFile(file, reloadOnChange: true, optional: false);
            //}

            appLifetime.ApplicationStarted.Register(OnStarted);
        }

        private void OnStarted()
        {
            //"On-started" logic
        }
    }
}