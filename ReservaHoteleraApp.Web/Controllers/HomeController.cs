using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;
using ReservaHoteleraApp.Web.Models;
using System.Diagnostics;

namespace ReservaHoteleraApp.Web.Controllers
{
    // Controlador de la página de inicio (Home).
    // Su trabajo es cargar algunos hoteles y mostrarlos en la página principal.
    public class HomeController : Controller
    {
        // Servicio de negocio de los hoteles.
        // Lo usamos para pedir la lista de hoteles sin tocar la base de datos.
        private readonly IHotelServices _hotelServices;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public HomeController(IHotelServices hotelServices)
        {
            _hotelServices = hotelServices;
        }

        // GET: /Home
        // Muestra la página de inicio con el buscador y la lista de hoteles.
        public async Task<IActionResult> Index()
        {
            // 1. Pedimos todos los hoteles al servicio.
            var respuesta = await _hotelServices.GetAllAsync();

            // 2. Si hubo un error, guardamos el mensaje para mostrarlo en la vista.
            if (respuesta.Success == false)
            {
                TempData["Error"] = respuesta.Message;
            }

            // 3. Obtenemos la lista de hoteles.
            //    Si viene vacía, creamos una lista vacía para que la vista no falle.
            var hoteles = respuesta.Data;
            if (hoteles == null)
            {
                hoteles = new List<HotelModel>();
            }

            // 4. Enviamos la lista a la vista Views/Home/Index.cshtml.
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
