using Microsoft.Extensions.DependencyInjection;
using ReservaHoteleraApp.BL.Services.Implementations;
using ReservaHoteleraApp.BL.Services.Interfaces;

namespace ReservaHoteleraApp.BL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioServices, UsuarioServices>();
            services.AddScoped<IClienteServices, ClienteServices>();
            services.AddScoped<ITipoHabitacionServices, TipoHabitacionServices>();
            services.AddScoped<IHabitacionServices, HabitacionServices>();
            services.AddScoped<IReservaServices, ReservaServices>();
            services.AddScoped<IEstadiaServices, EstadiaServices>();
            services.AddScoped<IPagoServices, PagoServices>();
            services.AddScoped<IRegistroActividadServices, RegistroActividadServices>();

            return services;
        }
    }
}
