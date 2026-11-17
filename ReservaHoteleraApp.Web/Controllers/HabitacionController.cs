using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;

namespace ReservaHoteleraApp.Web.Controllers
{
    // Controlador del módulo "Habitaciones".
    // Una habitación depende de un PISO y de un TIPO DE HABITACIÓN,
    // por eso necesitamos tres servicios: habitaciones, pisos y tipos.
    public class HabitacionController : Controller
    {
        private readonly IHabitacionServices _habitacionServices;
        private readonly IPisoServices _pisoServices;
        private readonly ITipoHabitacionServices _tipoHabitacionServices;

        public HabitacionController(
            IHabitacionServices habitacionServices,
            IPisoServices pisoServices,
            ITipoHabitacionServices tipoHabitacionServices)
        {
            _habitacionServices = habitacionServices;
            _pisoServices = pisoServices;
            _tipoHabitacionServices = tipoHabitacionServices;
        }

        // Llena los dos desplegables del formulario:
        //   ViewBag.Pisos -> lista de pisos
        //   ViewBag.Tipos -> lista de tipos de habitación
        // Los parámetros indican qué opción debe quedar seleccionada.
        private async Task CargarCombosAsync(int? idPiso = null, int? idTipo = null)
        {
            var pisos = await _pisoServices.GetAllAsync();
            var tipos = await _tipoHabitacionServices.GetAllAsync();

            ViewBag.Pisos = new SelectList(pisos.Data ?? new(), "Id", "Numero", idPiso);
            ViewBag.Tipos = new SelectList(tipos.Data ?? new(), "Id", "Nombre", idTipo);
        }

        // GET /Habitacion -> habitaciones disponibles para las fechas indicadas.
        // Con verTodas=true (administrador) se muestra el inventario completo.
        public async Task<IActionResult> Index(string? fechaEntrada, string? fechaSalida, bool verTodas = false)
        {
            DateTime entrada;
            if (!DateTime.TryParse(fechaEntrada, out entrada) || entrada < DateTime.Today)
                entrada = DateTime.Today;

            DateTime salida;
            if (!DateTime.TryParse(fechaSalida, out salida) || salida <= entrada)
                salida = entrada.AddDays(1);

            ViewBag.FechaEntrada = entrada.ToString("yyyy-MM-dd");
            ViewBag.FechaSalida = salida.ToString("yyyy-MM-dd");
            ViewBag.EntradaFecha = entrada;
            ViewBag.SalidaFecha = salida;
            ViewBag.VerTodas = verTodas;

            if (verTodas)
            {
                var todas = await _habitacionServices.GetAllAsync();
                if (!todas.Success)
                    TempData["Error"] = todas.Message;

                return View(todas.Data ?? new List<HabitacionModel>());
            }

            var disponibles = await _habitacionServices.GetDisponiblesAsync(entrada, salida);
            if (!disponibles.Success)
                TempData["Error"] = disponibles.Message;

            return View(disponibles.Data ?? new List<HabitacionModel>());
        }

        // GET /Habitacion/Details/5 -> detalles de una habitación.
        public async Task<IActionResult> Details(int id)
        {
            var result = await _habitacionServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // GET /Habitacion/Create -> formulario vacío.
        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult> Create()
        {
            await CargarCombosAsync();
            return View();
        }

        // POST /Habitacion/Create -> guarda la nueva habitación.
        [Authorize(Roles ="Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HabitacionAdd habitacion)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombosAsync(habitacion.IdPiso, habitacion.IdTipoHabitacion);
                return View(habitacion);
            }

            var result = await _habitacionServices.AddAsync(habitacion);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "No se pudo registrar la habitación.");
                await CargarCombosAsync(habitacion.IdPiso, habitacion.IdTipoHabitacion);
                return View(habitacion);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /Habitacion/Edit/5 -> carga la habitación en el formulario.
        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _habitacionServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            // Convertimos HabitacionModel (lectura) en HabitacionUpdate (actualización).
            var model = new HabitacionUpdate
            {
                Id = result.Data.Id,
                Numero = result.Data.Numero,
                IdPiso = result.Data.IdPiso,
                IdTipoHabitacion = result.Data.IdTipoHabitacion,
                PrecioNoche = result.Data.PrecioNoche,
                Estado = result.Data.Estado
            };
            await CargarCombosAsync(model.IdPiso, model.IdTipoHabitacion);
            return View(model);
        }

        // POST /Habitacion/Edit -> guarda los cambios.
        [Authorize(Roles ="Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HabitacionUpdate habitacion)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombosAsync(habitacion.IdPiso, habitacion.IdTipoHabitacion);
                return View(habitacion);
            }

            var result = await _habitacionServices.UpdateAsync(habitacion);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "No se pudo actualizar la habitación.");
                await CargarCombosAsync(habitacion.IdPiso, habitacion.IdTipoHabitacion);
                return View(habitacion);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /Habitacion/Delete/5 -> confirmación para eliminar.
        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _habitacionServices.GetByIdAsync(id);
            if (!result.Success || result.Data is null)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // POST /Habitacion/Delete -> elimina la habitación confirmada.
        [Authorize(Roles ="Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _habitacionServices.DeleteAsync(new HabitacionRemove { Id = id });

            if (!result.Success)
                TempData["Error"] = result.Message;
            else
                TempData["Success"] = result.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}
