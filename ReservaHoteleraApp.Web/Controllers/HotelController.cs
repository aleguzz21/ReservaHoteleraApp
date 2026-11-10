using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;

namespace ReservaHoteleraApp.Web.Controllers
{
    // Controlador del módulo "Hoteles".
    // Su trabajo es recibir las peticiones del navegador y llamar a la capa BL (servicios).
    // El controlador NO habla directamente con la base de datos: solo usa IHotelServices.
    public class HotelController : Controller
    {
        // Servicio de negocio que contiene las reglas del módulo Hotel.
        private readonly IHotelServices _hotelServices;

        // ASP.NET Core entrega (inyecta) el servicio automáticamente al crear el controlador.
        public HotelController(IHotelServices hotelServices)
        {
            _hotelServices = hotelServices;
        }

        // GET /Hotel -> muestra la lista de hoteles.
        public async Task<IActionResult> Index()
        {
            // Pedimos todos los hoteles al servicio.
            var result = await _hotelServices.GetAllAsync();

            // Si hubo un error, lo guardamos en TempData para mostrarlo en la vista.
            if (!result.Success)
                TempData["Error"] = result.Message;

            // Si Data viene null, enviamos una lista vacía para que la vista no falle.
            return View(result.Data ?? new List<HotelModel>());
        }

        // GET /Hotel/Details/5 -> muestra los datos de un hotel.
        public async Task<IActionResult> Details(int id)
        {
            var result = await _hotelServices.GetByIdAsync(id);

            // Si no existe o hubo error, volvemos a la lista con un mensaje.
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // GET /Hotel/Create -> muestra el formulario vacío para crear un hotel.
        public IActionResult Create()
        {
            return View();
        }

        // POST /Hotel/Create -> recibe los datos del formulario y guarda el hotel.
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF.
        public async Task<IActionResult> Create(HotelAdd hotel)
        {
            // Si las validaciones del formulario (DataAnnotations) fallan, volvemos a mostrarlo.
            if (!ModelState.IsValid)
                return View(hotel);

            var result = await _hotelServices.AddAsync(hotel);
            if (!result.Success)
            {
                // Mostramos el error del servicio dentro del propio formulario.
                ModelState.AddModelError(string.Empty, result.Message ?? "No se pudo registrar el hotel.");
                return View(hotel);
            }

            // Mensaje de éxito que se verá en la lista (TempData sobrevive a la redirección).
            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /Hotel/Edit/5 -> carga el hotel y muestra el formulario de edición.
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _hotelServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            // Convertimos el modelo de lectura (HotelModel) en uno de actualización (HotelUpdate).
            var model = new HotelUpdate
            {
                Id = result.Data.Id,
                Nombre = result.Data.Nombre,
                Direccion = result.Data.Direccion,
                Telefono = result.Data.Telefono,
                Ciudad = result.Data.Ciudad
            };
            return View(model);
        }

        // POST /Hotel/Edit -> guarda los cambios del hotel.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HotelUpdate hotel)
        {
            if (!ModelState.IsValid)
                return View(hotel);

            var result = await _hotelServices.UpdateAsync(hotel);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "No se pudo actualizar el hotel.");
                return View(hotel);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /Hotel/Delete/5 -> muestra la pantalla de confirmación para eliminar.
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _hotelServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // POST /Hotel/Delete -> elimina el hotel cuando el usuario confirma.
        // Se llama "DeleteConfirmed" porque el GET ya se llama "Delete".
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _hotelServices.DeleteAsync(new HotelRemove { Id = id });

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
