using Microsoft.EntityFrameworkCore;
using DbData;
//-- JWT --//
using JWTAuthentication.NET6._0.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("{SWAGGER_VERSION}", new OpenApiInfo { Title = "{PROJECT_TITLE}", Version = "{SWAGGER_VERSION}" });
            });

            //services.AddAuthorization();

            //-- For Entity Framework
            services.AddDbContext<AppDataContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            });

            //-- For Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDataContext>()
                .AddDefaultTokenProviders();

            services.Configure<CorsConfiguration>(Configuration.GetSection("corsConfiguration"));
            var corsPoly = Configuration.GetValue<string>("corsConfiguration:corsPoly").ToString().Split('|');
           
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
            #region JWT
            //-- Adding Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            //-- Adding Jwt Bearer
            .AddJwtBearer(options =>
             {
                 options.SaveToken = true;
                 options.RequireHttpsMetadata = false;
                 options.TokenValidationParameters = new TokenValidationParameters()
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidAudience = Configuration["JWT:ValidAudience"],
                     ValidIssuer = Configuration["JWT:ValidIssuer"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
                 };
            });
            #endregion

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim("Admin"));
                options.AddPolicy("User", policy => policy.RequireClaim("User"));
            });
            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
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

            app.UseAuthentication();

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
