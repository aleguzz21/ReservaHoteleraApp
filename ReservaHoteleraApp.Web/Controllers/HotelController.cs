using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;

namespace ReservaHoteleraApp.Web.Controllers
{
    // Controlador del módulo "Hoteles".
    // Recibe las peticiones del navegador, llama al servicio y devuelve una vista.
    // El controlador NO habla directamente con la base de datos.
    public class HotelController : Controller
    {
        // Servicio de negocio de los hoteles.
        private readonly IHotelServices _hotelServices;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public HotelController(IHotelServices hotelServices)
        {
            _hotelServices = hotelServices;
        }

        // ---------------------------------------------------------
        // LISTAR HOTELES
        // GET: /Hotel
        // También recibe los datos del buscador de la página de inicio.
        // ---------------------------------------------------------
        public async Task<IActionResult> Index(string? destino, DateTime? fechaEntrada, DateTime? fechaSalida, int? adultos, int? ninos, int? habitaciones)
        {
            // 1. Pedimos los hoteles al servicio.
            //    La búsqueda por destino es una regla de negocio y vive en la capa BL.
            var respuesta = await _hotelServices.BuscarAsync(destino);

            // 2. Si hubo un error, guardamos el mensaje para mostrarlo en la vista.
            if (respuesta.Success == false)
            {
                TempData["Error"] = respuesta.Message;
            }

            // 3. Obtenemos la lista.
            //    Si viene vacía, creamos una lista vacía para que la vista no falle.
            var hoteles = respuesta.Data;
            if (hoteles == null)
            {
                hoteles = new List<HotelModel>();
            }

            // 4. Guardamos el destino y los demás datos del buscador por si la vista los necesita.
            if (string.IsNullOrWhiteSpace(destino) == false)
            {
                ViewData["Destino"] = destino.Trim();
            }

            ViewData["FechaEntrada"] = fechaEntrada;
            ViewData["FechaSalida"] = fechaSalida;
            ViewData["Adultos"] = adultos;
            ViewData["Ninos"] = ninos;
            ViewData["Habitaciones"] = habitaciones;

            // 5. Enviamos la lista a la vista Views/Hotel/Index.cshtml.
            return View(hoteles);
        }

        // ---------------------------------------------------------
        // DETALLES DE UN HOTEL
        // GET: /Hotel/Details/5
        // ---------------------------------------------------------
        public async Task<IActionResult> Details(int id)
        {
            var respuesta = await _hotelServices.GetByIdAsync(id);

            // Si el hotel no existe o hubo un error, volvemos a la lista.
            if (respuesta.Success == false || respuesta.Data == null)
            {
                TempData["Error"] = respuesta.Message;
                return RedirectToAction("Index");
            }

            return View(respuesta.Data);
        }

        // ---------------------------------------------------------
        // CREAR HOTEL (mostrar formulario)
        // GET: /Hotel/Create
       // ---------------------------------------------------------
       [Authorize(Roles ="Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // ---------------------------------------------------------
        // CREAR HOTEL (guardar formulario)
        // POST: /Hotel/Create        
        // ---------------------------------------------------------
        [Authorize(Roles ="Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF.
        public async Task<IActionResult> Create(HotelAdd hotel)
        {
            // Si el formulario tiene errores de validación, lo mostramos otra vez.
            if (ModelState.IsValid == false)
            {
                return View(hotel);
            }

            var respuesta = await _hotelServices.AddAsync(hotel);

            // Si el servicio no pudo guardar, mostramos el error en el formulario.
            if (respuesta.Success == false)
            {
                string? mensajeDelServicio = respuesta.Message;
                string mensaje = "No se pudo registrar el hotel.";

                if (string.IsNullOrEmpty(mensajeDelServicio) == false)
                {
                    mensaje = mensajeDelServicio;
                }

                ModelState.AddModelError("", mensaje);
                return View(hotel);
            }

            // Si todo salió bien, mostramos el mensaje y volvemos a la lista.
            TempData["Success"] = respuesta.Message;
            return RedirectToAction("Index");
        }

        // ---------------------------------------------------------
        // EDITAR HOTEL (mostrar formulario)
        // GET: /Hotel/Edit/5        
        // ---------------------------------------------------------
        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            var respuesta = await _hotelServices.GetByIdAsync(id);

            if (respuesta.Success == false || respuesta.Data == null)
            {
                TempData["Error"] = respuesta.Message;
                return RedirectToAction("Index");
            }

            // Copiamos los datos del hotel al objeto que usa el formulario de edición.
            var hotelParaEditar = new HotelUpdate();
            hotelParaEditar.Id = respuesta.Data.Id;
            hotelParaEditar.Nombre = respuesta.Data.Nombre;
            hotelParaEditar.Direccion = respuesta.Data.Direccion;
            hotelParaEditar.Telefono = respuesta.Data.Telefono;
            hotelParaEditar.Ciudad = respuesta.Data.Ciudad;

            return View(hotelParaEditar);
        }

        // ---------------------------------------------------------
        // EDITAR HOTEL (guardar cambios)
        // POST: /Hotel/Edit
        // ---------------------------------------------------------
        [Authorize(Roles ="Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HotelUpdate hotel)
        {
            if (ModelState.IsValid == false)
            {
                return View(hotel);
            }

            var respuesta = await _hotelServices.UpdateAsync(hotel);

            if (respuesta.Success == false)
            {
                string? mensajeDelServicio = respuesta.Message;
                string mensaje = "No se pudo actualizar el hotel.";

                if (string.IsNullOrEmpty(mensajeDelServicio) == false)
                {
                    mensaje = mensajeDelServicio;
                }

                ModelState.AddModelError("", mensaje);
                return View(hotel);
            }

            TempData["Success"] = respuesta.Message;
            return RedirectToAction("Index");
        }

        // ---------------------------------------------------------
        // ELIMINAR HOTEL (mostrar confirmación)
        // GET: /Hotel/Delete/5
        // ---------------------------------------------------------
        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var respuesta = await _hotelServices.GetByIdAsync(id);

            if (respuesta.Success == false || respuesta.Data == null)
            {
                TempData["Error"] = respuesta.Message;
                return RedirectToAction("Index");
            }

            return View(respuesta.Data);
        }

        // ---------------------------------------------------------
        // ELIMINAR HOTEL (confirmado)
        // POST: /Hotel/Delete
        // Se llama "DeleteConfirmed" porque el GET ya se llama "Delete".
        // ---------------------------------------------------------
        [Authorize(Roles ="Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Creamos el objeto que necesita el servicio para eliminar.
            var hotelParaEliminar = new HotelRemove();
            hotelParaEliminar.Id = id;

            var respuesta = await _hotelServices.DeleteAsync(hotelParaEliminar);

            if (respuesta.Success == false)
            {
                TempData["Error"] = respuesta.Message;
            }
            else
            {
                TempData["Success"] = respuesta.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
