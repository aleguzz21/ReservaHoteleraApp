using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Bloque;

namespace ReservaHoteleraApp.Web.Controllers
{
    // Controlador del módulo "Bloques".
    // Un bloque pertenece a un hotel, por eso también necesitamos IHotelServices
    // para llenar el desplegable (dropdown) de hoteles en los formularios.
    public class BloqueController : Controller
    {
        private readonly IBloqueServices _bloqueServices;
        private readonly IHotelServices _hotelServices;

        public BloqueController(IBloqueServices bloqueServices, IHotelServices hotelServices)
        {
            _bloqueServices = bloqueServices;
            _hotelServices = hotelServices;
        }

        // Carga la lista de hoteles en ViewBag.Hoteles para usarla en el <select>.
        // "seleccionado" indica qué hotel debe quedar marcado (útil al editar o al re-mostrar el formulario con error).
        private async Task CargarHotelesAsync(int? seleccionado = null)
        {
            var hoteles = await _hotelServices.GetAllAsync();
            ViewBag.Hoteles = new SelectList(hoteles.Data ?? new(), "Id", "Nombre", seleccionado);
        }

        // GET /Bloque -> lista de bloques.
        public async Task<IActionResult> Index()
        {
            var result = await _bloqueServices.GetAllAsync();
            if (!result.Success)
                TempData["Error"] = result.Message;

            return View(result.Data ?? new List<BloqueModel>());
        }

        // GET /Bloque/Details/5 -> detalles de un bloque.
        public async Task<IActionResult> Details(int id)
        {
            var result = await _bloqueServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // GET /Bloque/Create -> formulario vacío.
        public async Task<IActionResult> Create()
        {
            await CargarHotelesAsync();
            return View();
        }

        // POST /Bloque/Create -> guarda el nuevo bloque.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BloqueAdd bloque)
        {
            if (!ModelState.IsValid)
            {
                await CargarHotelesAsync(bloque.IdHotel); // recargamos el combo
                return View(bloque);
            }

            var result = await _bloqueServices.AddAsync(bloque);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "No se pudo registrar el bloque.");
                await CargarHotelesAsync(bloque.IdHotel);
                return View(bloque);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /Bloque/Edit/5 -> carga el bloque en el formulario de edición.
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _bloqueServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            // Pasamos de BloqueModel (lectura) a BloqueUpdate (actualización).
            var model = new BloqueUpdate
            {
                Id = result.Data.Id,
                Nombre = result.Data.Nombre,
                IdHotel = result.Data.IdHotel,
                Descripcion = result.Data.Descripcion
            };
            await CargarHotelesAsync(model.IdHotel);
            return View(model);
        }

        // POST /Bloque/Edit -> guarda los cambios.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BloqueUpdate bloque)
        {
            if (!ModelState.IsValid)
            {
                await CargarHotelesAsync(bloque.IdHotel);
                return View(bloque);
            }

            var result = await _bloqueServices.UpdateAsync(bloque);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "No se pudo actualizar el bloque.");
                await CargarHotelesAsync(bloque.IdHotel);
                return View(bloque);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /Bloque/Delete/5 -> confirmación para eliminar.
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bloqueServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // POST /Bloque/Delete -> elimina el bloque confirmado.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _bloqueServices.DeleteAsync(new BloqueRemove { Id = id });

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
