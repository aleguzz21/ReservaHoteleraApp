using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;
using ReservaHoteleraApp.Web.Models;
using System.Diagnostics;

namespace ReservaHoteleraApp.Web.Controllers
{
    // Controlador de la página de inicio (Home).
    // Muestra los hoteles y permite buscar habitaciones disponibles por hotel y fechas.
    public class HomeController : Controller
    {
        // Servicios de negocio: hoteles y habitaciones (sin tocar la base de datos aquí).
        private readonly IHotelServices _hotelServices;
        private readonly IHabitacionServices _habitacionServices;

        public HomeController(IHotelServices hotelServices, IHabitacionServices habitacionServices)
        {
            _hotelServices = hotelServices;
            _habitacionServices = habitacionServices;
        }

        // GET: /Home
        // Si llega hotelId o fechas, además de los hoteles devuelve las habitaciones libres.
        public async Task<IActionResult> Index(int? hotelId, string? fechaEntrada, string? fechaSalida)
        {
            var respuesta = await _hotelServices.GetAllAsync();
            if (!respuesta.Success)
                TempData["Error"] = respuesta.Message;

            var hoteles = respuesta.Data ?? new List<HotelModel>();

            ViewBag.HotelId = hotelId;
            ViewBag.Buscando = false;
            ViewBag.Habitaciones = new List<HabitacionModel>();

            var quiereBuscar = hotelId.HasValue
                || !string.IsNullOrWhiteSpace(fechaEntrada)
                || !string.IsNullOrWhiteSpace(fechaSalida);

            if (quiereBuscar)
            {
                // Fechas: si no llegan o son inválidas, se usa hoy → mañana.
                DateTime entrada;
                if (!DateTime.TryParse(fechaEntrada, out entrada) || entrada < DateTime.Today)
                    entrada = DateTime.Today;

                DateTime salida;
                if (!DateTime.TryParse(fechaSalida, out salida) || salida <= entrada)
                    salida = entrada.AddDays(1);

                ViewBag.FechaEntrada = entrada;
                ViewBag.FechaSalida = salida;

                var disponibles = await _habitacionServices.GetDisponiblesAsync(entrada, salida);
                if (!disponibles.Success)
                {
                    TempData["Error"] = disponibles.Message;
                }
                else
                {
                    var lista = disponibles.Data ?? new List<HabitacionModel>();
                    if (hotelId.HasValue)
                        lista = lista.Where(h => h.IdHotel == hotelId.Value).ToList();

                    ViewBag.Habitaciones = lista;
                    ViewBag.Buscando = true;
                }
            }

            return View(hoteles);
        }

        // GET: /Home/Privacy
        // Muestra la página de privacidad.
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: /Home/Error
        // Muestra la página de error.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var modelo = new ErrorViewModel();
            modelo.RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View(modelo);
        }
    }
}
