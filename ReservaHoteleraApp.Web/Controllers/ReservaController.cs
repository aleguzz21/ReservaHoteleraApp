using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.Web.Models;

namespace ReservaHoteleraApp.Web.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly IReservaServices _reservaServices;
        private readonly IHabitacionServices _habitacionServices;
        private readonly IClienteServices _clienteServices;


        public ReservaController(IReservaServices reservaServices, IHabitacionServices habitacionServices, IClienteServices clienteServices)
        {
            _reservaServices = reservaServices;
            _habitacionServices = habitacionServices;
            _clienteServices = clienteServices;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int habitacionId, string? fechaEntrada, string? fechaSalida)
        {
            var habitacion = await _habitacionServices.GetByIdAsync(habitacionId);
            if (!habitacion.Success || habitacion.Data is null)
            {
                TempData["Error"] = habitacion.Message;
                return RedirectToAction("Index", "Habitacion");
            }

            if (habitacion.Data.Estado == "Mantenimiento")
            {
                TempData["Error"] = "La habitación está en mantenimiento.";
                return RedirectToAction("Index", "Habitacion");
            }

            // Las fechas pueden venir desde la lista de disponibles.
            var entrada = DateTime.Today;
            if (DateTime.TryParse(fechaEntrada, out var entradaPedida) && entradaPedida >= DateTime.Today)
                entrada = entradaPedida;

            var salida = entrada.AddDays(1);
            if (DateTime.TryParse(fechaSalida, out var salidaPedida) && salidaPedida > entrada)
                salida = salidaPedida;

            var model = new ReservaViewModel
            {
                IdHabitacion = habitacion.Data.Id,
                NumeroHabitacion = habitacion.Data.Numero,
                HotelNombre = habitacion.Data.HotelNombre,
                PisoNumero = habitacion.Data.PisoNumero,
                TipoHabitacionNombre = habitacion.Data.TipoHabitacionNombre,
                PrecioNoche = habitacion.Data.PrecioNoche,
                FechaEntrada = entrada,
                FechaSalida = salida,
                CantidadPersonas = 1
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var habitacion = await _habitacionServices.GetByIdAsync(model.IdHabitacion);

            if (!habitacion.Success || habitacion.Data is null)
            {
                ModelState.AddModelError("", "La habitación no existe.");
                return View(model);
            }

            if (habitacion.Data.Estado == "Mantenimiento")
            {
                ModelState.AddModelError("", "La habitación está en mantenimiento y no se puede reservar.");
                return View(model);
            }

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var idUsuario))
            {
                ModelState.AddModelError("", "No se pudo identificar al usuario.");
                return View(model);
            }

            var cliente = await _clienteServices.GetByUsuarioIdAsync(idUsuario);
            if (!cliente.Success || cliente.Data is null)
            {
                ModelState.AddModelError("", "Tu usuario no tiene ficha de cliente.");
                return View(model);
            }

            var disponible = await _reservaServices.EstaDisponibleAsync(
                model.IdHabitacion, model.FechaEntrada, model.FechaSalida);

            if (!disponible.Success || disponible.Data == false)
            {
                ModelState.AddModelError("", disponible.Message ?? "La habitación no está disponible.");
                return View(model);
            }

            var noches = (model.FechaSalida - model.FechaEntrada).Days;

            var reserva = new ReservaAdd
            {
                IdCliente = cliente.Data.Id,
                IdHabitacion = model.IdHabitacion,
                CantidadPersonas = model.CantidadPersonas,
                Estado = "Pendiente",
                TotalReserva = noches * habitacion.Data.PrecioNoche,
                FechaEntrada = model.FechaEntrada,
                FechaSalida = model.FechaSalida
            };

            var resultado = await _reservaServices.AddAsync(reserva);
            if (!resultado.Success)
            {
                ModelState.AddModelError("", resultado.Message ?? "No se pudo registrar la reserva.");
                return View(model);
            }

            TempData["Success"] = "Reserva registrada correctamente.";
            return RedirectToAction("Index", "Habitacion");
        }

        // GET /Reserva -> "Mis reservas". El administrador ve todas.
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Administrador"))
            {
                var todas = await _reservaServices.GetAllConDetalleAsync();
                if (!todas.Success)
                    TempData["Error"] = todas.Message;

                return View(todas.Data ?? new List<ReservaDetalleModel>());
            }

            var idCliente = await ObtenerIdClienteAsync();
            if (idCliente is null)
            {
                TempData["Error"] = "Tu usuario no tiene ficha de cliente.";
                return View(new List<ReservaDetalleModel>());
            }

            var reservas = await _reservaServices.GetByClienteIdAsync(idCliente.Value);
            if (!reservas.Success)
                TempData["Error"] = reservas.Message;

            return View(reservas.Data ?? new List<ReservaDetalleModel>());
        }

        // POST /Reserva/Cancelar -> marca la reserva como cancelada.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var esAdministrador = User.IsInRole("Administrador");
            var idCliente = 0;

            if (!esAdministrador)
            {
                var cliente = await ObtenerIdClienteAsync();
                if (cliente is null)
                {
                    TempData["Error"] = "Tu usuario no tiene ficha de cliente.";
                    return RedirectToAction(nameof(Index));
                }

                idCliente = cliente.Value;
            }

            var resultado = await _reservaServices.CancelarAsync(id, idCliente, esAdministrador);
            if (!resultado.Success)
                TempData["Error"] = resultado.Message;
            else
                TempData["Success"] = resultado.Message;

            return RedirectToAction(nameof(Index));
        }

        // Traduce el IdUsuario del claim en el IdCliente de la ficha.
        private async Task<int?> ObtenerIdClienteAsync()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var idUsuario))
                return null;

            var cliente = await _clienteServices.GetByUsuarioIdAsync(idUsuario);
            if (!cliente.Success || cliente.Data is null)
                return null;

            return cliente.Data.Id;
        }

    }

}