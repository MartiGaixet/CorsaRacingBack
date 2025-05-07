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

            // Configurar base de datos
            builder.Services.AddDbContext<Context>(options =>
                options.UseSqlServer("Server=tcp:corsaserver.database.windows.net,1433;Initial Catalog=CorsaRacingDB;Persist Security Info=False;User ID=adminmarti;Password=Ramonrubial61;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30"));

            // Registrar repositorios y servicios en el contenedor de dependencias
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IChampionshipRepository, ChampionshipRepository>();
            builder.Services.AddScoped<IChampionshipService, ChampionshipService>();
            builder.Services.AddScoped<IRacesRepository, RaceRepository>();
            builder.Services.AddScoped<IRaceService, RaceService>();
            builder.Services.AddScoped<IParticipationRaceRepository, ParticipationRaceRepository>();
            builder.Services.AddScoped<IParticipationRaceService, ParticipationRaceService>();

            // Configurar controladores y JSON para evitar referencias circulares
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            // Agregar servicio CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy.WithOrigins(
                        "http://localhost:3000",           // Desarrollo local
                        "https://tuapp.vercel.app"        // Sustituye por el dominio real de Vercel cuando lo tengas
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Configurar el pipeline de la aplicación
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Usar política CORS
            app.UseCors("FrontendPolicy");

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
