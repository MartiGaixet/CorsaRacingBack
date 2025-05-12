using Microsoft.AspNetCore.Cors.Infrastructure;
using CorsaRacing.Models;
using CorsaRacing.Repositories;
using CorsaRacing.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace CorsaRacing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer("preguntar a admin por contraseña y usuario");
            
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IChampionshipRepository, ChampionshipRepository>();
            builder.Services.AddScoped<IChampionshipService, ChampionshipService>();
            builder.Services.AddScoped<IRacesRepository, RaceRepository>();
            builder.Services.AddScoped<IRaceService, RaceService>();
            builder.Services.AddScoped<IParticipationRaceRepository, ParticipationRaceRepository>();
            builder.Services.AddScoped<IParticipationRaceService, ParticipationRaceService>();

            
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:3000",           
                        "https://corsaracing.vercel.app"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            
            app.UseCors("FrontendPolicy");

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
