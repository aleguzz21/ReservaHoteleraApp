using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Piso;

namespace ReservaHoteleraApp.Web.Controllers
{
    // Controlador del módulo "Pisos".
    // Un piso pertenece a un bloque, por eso usamos IBloqueServices
    // para llenar el desplegable de bloques.
    public class PisoController : Controller
    {
        private readonly IPisoServices _pisoServices;
        private readonly IBloqueServices _bloqueServices;

        public PisoController(IPisoServices pisoServices, IBloqueServices bloqueServices)
        {
            _pisoServices = pisoServices;
            _bloqueServices = bloqueServices;
        }

        // Llena ViewBag.Bloques con la lista de bloques para el <select>.
        private async Task CargarBloquesAsync(int? seleccionado = null)
        {
            var bloques = await _bloqueServices.GetAllAsync();
            ViewBag.Bloques = new SelectList(bloques.Data ?? new(), "Id", "Nombre", seleccionado);
        }

        // GET /Piso -> lista de pisos.
        public async Task<IActionResult> Index()
        {
            var result = await _pisoServices.GetAllAsync();
            if (!result.Success)
                TempData["Error"] = result.Message;

            return View(result.Data ?? new List<PisoModel>());
        }

        // GET /Piso/Details/5 -> detalles de un piso.
        public async Task<IActionResult> Details(int id)
        {
            var result = await _pisoServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // GET /Piso/Create -> formulario vacío.
        public async Task<IActionResult> Create()
        {
            await CargarBloquesAsync();
            return View();
        }

        // POST /Piso/Create -> guarda el nuevo piso.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PisoAdd piso)
        {
            if (!ModelState.IsValid)
            {
                await CargarBloquesAsync(piso.IdBloque);
                return View(piso);
            }

            var result = await _pisoServices.AddAsync(piso);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "No se pudo registrar el piso.");
                await CargarBloquesAsync(piso.IdBloque);
                return View(piso);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /Piso/Edit/5 -> carga el piso en el formulario.
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _pisoServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            // Convertimos PisoModel (lectura) en PisoUpdate (actualización).
            var model = new PisoUpdate
            {
                Id = result.Data.Id,
                Numero = result.Data.Numero,
                IdBloque = result.Data.IdBloque,
                Descripcion = result.Data.Descripcion
            };
            await CargarBloquesAsync(model.IdBloque);
            return View(model);
        }

        // POST /Piso/Edit -> guarda los cambios.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PisoUpdate piso)
        {
            if (!ModelState.IsValid)
            {
                await CargarBloquesAsync(piso.IdBloque);
                return View(piso);
            }

            var result = await _pisoServices.UpdateAsync(piso);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "No se pudo actualizar el piso.");
                await CargarBloquesAsync(piso.IdBloque);
                return View(piso);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /Piso/Delete/5 -> confirmación para eliminar.
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _pisoServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // POST /Piso/Delete -> elimina el piso confirmado.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _pisoServices.DeleteAsync(new PisoRemove { Id = id });

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
