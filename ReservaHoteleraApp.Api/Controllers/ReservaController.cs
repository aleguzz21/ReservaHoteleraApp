using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Reservas".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class ReservaController : ControllerBase
    {
        // Servicio de negocio de las reservas.
        private readonly IReservaServices _reservaServices;
        private readonly ILogger<ReservaController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public ReservaController(IReservaServices reservaServices, ILogger<ReservaController> logger)
        {
            _reservaServices = reservaServices;
            _logger = logger;
        }

        // LISTAR RESERVAS
        // GET: api/Reserva
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos las reservas al servicio.
            var respuesta = await _reservaServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener las reservas: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Si todo salió bien, devolvemos la lista con código 200.
            return Ok(respuesta);
        }

        // LISTAR RESERVAS CON DETALLE
        // GET: api/Reserva/ConDetalle
        [HttpGet("ConDetalle")]
        public async Task<IActionResult> ConDetalle()
        {
            var respuesta = await _reservaServices.GetAllConDetalleAsync();

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener las reservas con detalle: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // RESERVAS DE UN CLIENTE
        // GET: api/Reserva/Cliente/5
        [HttpGet("Cliente/{idCliente}")]
        public async Task<IActionResult> Cliente(int idCliente)
        {
            var respuesta = await _reservaServices.GetByClienteIdAsync(idCliente);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener las reservas del cliente {IdCliente}: {ErrorMessage}", idCliente, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // VERIFICAR DISPONIBILIDAD DE UNA HABITACIÓN
        // GET: api/Reserva/Disponible?idHabitacion=1&fechaEntrada=2026-01-01&fechaSalida=2026-01-05
        [HttpGet("Disponible")]
        public async Task<IActionResult> Disponible([FromQuery] int idHabitacion, [FromQuery] DateTime fechaEntrada, [FromQuery] DateTime fechaSalida)
        {
            var respuesta = await _reservaServices.EstaDisponibleAsync(idHabitacion, fechaEntrada, fechaSalida);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al verificar la disponibilidad de la habitación {IdHabitacion}: {ErrorMessage}", idHabitacion, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // DETALLES DE UNA RESERVA
        // GET: api/Reserva/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _reservaServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener la reserva {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // CANCELAR RESERVA
        // POST: api/Reserva/Cancelar
        [HttpPost("Cancelar")]
        public async Task<IActionResult> Cancelar([FromBody] CancelarReservaRequest request)
        {
            var respuesta = await _reservaServices.CancelarAsync(request.IdReserva, request.IdCliente, request.EsAdministrador);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // CREAR RESERVA
        // POST: api/Reserva/CreateReserva
        [HttpPost("CreateReserva")]
        public async Task<IActionResult> Post([FromBody] ReservaAdd reserva)
        {
            var respuesta = await _reservaServices.AddAsync(reserva);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // EDITAR RESERVA
        // POST: api/Reserva/ModifyReserva
        [HttpPost("ModifyReserva")]
        public async Task<IActionResult> Put([FromBody] ReservaUpdate reserva)
        {
            var respuesta = await _reservaServices.UpdateAsync(reserva);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ELIMINAR RESERVA
        // POST: api/Reserva/DisableReserva
        [HttpPost("DisableReserva")]
        public async Task<IActionResult> DisableReserva([FromBody] ReservaRemove reserva)
        {
            var respuesta = await _reservaServices.DeleteAsync(reserva);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }

    // Datos que se reciben para cancelar una reserva.
    public class CancelarReservaRequest
    {
        public int IdReserva { get; set; }
        public int IdCliente { get; set; }
        public bool EsAdministrador { get; set; }
    }
}
