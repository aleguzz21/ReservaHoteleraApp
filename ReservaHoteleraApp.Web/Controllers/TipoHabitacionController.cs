using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;

namespace ReservaHoteleraApp.Web.Controllers
{
    // Controlador del módulo "Tipos de Habitación".
    // Es un catálogo simple: no depende de otros módulos,
    // por eso solo necesita un servicio.
    public class TipoHabitacionController : Controller
    {
        private readonly ITipoHabitacionServices _tipoHabitacionServices;

        public TipoHabitacionController(ITipoHabitacionServices tipoHabitacionServices)
        {
            _tipoHabitacionServices = tipoHabitacionServices;
        }

        // GET /TipoHabitacion -> lista de tipos.
        public async Task<IActionResult> Index()
        {
            var result = await _tipoHabitacionServices.GetAllAsync();
            if (!result.Success)
                TempData["Error"] = result.Message;

            return View(result.Data ?? new List<TipoHabitacionModel>());
        }

        // GET /TipoHabitacion/Details/5 -> detalles de un tipo.
        public async Task<IActionResult> Details(int id)
        {
            var result = await _tipoHabitacionServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // GET /TipoHabitacion/Create -> formulario vacío.
        public IActionResult Create()
        {
            return View();
        }

        // POST /TipoHabitacion/Create -> guarda el nuevo tipo.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoHabitacionAdd tipoHabitacion)
        {
            if (!ModelState.IsValid)
                return View(tipoHabitacion);

            var result = await _tipoHabitacionServices.AddAsync(tipoHabitacion);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "No se pudo registrar el tipo de habitación.");
                return View(tipoHabitacion);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /TipoHabitacion/Edit/5 -> carga el tipo en el formulario.
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _tipoHabitacionServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            // Convertimos TipoHabitacionModel (lectura) en TipoHabitacionUpdate (actualización).
            var model = new TipoHabitacionUpdate
            {
                Id = result.Data.Id,
                Nombre = result.Data.Nombre,
                Descripcion = result.Data.Descripcion,
                Capacidad = result.Data.Capacidad,
                Precio = result.Data.Precio
            };
            return View(model);
        }

        // POST /TipoHabitacion/Edit -> guarda los cambios.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TipoHabitacionUpdate tipoHabitacion)
        {
            if (!ModelState.IsValid)
                return View(tipoHabitacion);

            var result = await _tipoHabitacionServices.UpdateAsync(tipoHabitacion);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "No se pudo actualizar el tipo de habitación.");
                return View(tipoHabitacion);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /TipoHabitacion/Delete/5 -> confirmación para eliminar.
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _tipoHabitacionServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // POST /TipoHabitacion/Delete -> elimina el tipo confirmado.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _tipoHabitacionServices.DeleteAsync(new TipoHabitacionRemove { Id = id });

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
