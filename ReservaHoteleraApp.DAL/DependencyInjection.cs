using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservaHoteleraApp.DAL.Data;
using ReservaHoteleraApp.DAL.Repositories;

namespace ReservaHoteleraApp.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("HotelDb")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'HotelDb'.");

            services.AddDbContext<HotelDbContext>(options =>
                options.UseSqlServer(connectionString, sqlServer =>
                    sqlServer.MigrationsAssembly(typeof(HotelDbContext).Assembly.FullName)));

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<ITipoHabitacionRepository, TipoHabitacionRepository>();
            services.AddScoped<IHabitacionRepository, HabitacionRepository>();
            services.AddScoped<IReservaRepository, ReservaRepository>();
            services.AddScoped<IEstadiaRepository, EstadiaRepository>();
            services.AddScoped<IPagoRepository, PagoRepository>();
            services.AddScoped<IRegistroActividadRepository, RegistroActividadRepository>();

            return services;
        }
    }
}
