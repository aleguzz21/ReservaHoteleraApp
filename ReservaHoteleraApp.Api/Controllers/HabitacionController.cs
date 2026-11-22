using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Habitaciones".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class HabitacionController : ControllerBase
    {
        // Servicio de negocio de las habitaciones.
        private readonly IHabitacionServices _habitacionServices;
        private readonly ILogger<HabitacionController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public HabitacionController(IHabitacionServices habitacionServices, ILogger<HabitacionController> logger)
        {
            _habitacionServices = habitacionServices;
            _logger = logger;
        }

        // ---------------------------------------------------------
        // LISTAR HABITACIONES
        // GET: api/Habitacion
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos las habitaciones al servicio.
            var respuesta = await _habitacionServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener las habitaciones: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Si todo salió bien, devolvemos la lista con código 200.
            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // HABITACIONES DISPONIBLES ENTRE DOS FECHAS
        // GET: api/Habitacion/Disponibles?fechaEntrada=2026-01-01&fechaSalida=2026-01-05
        // ---------------------------------------------------------
        [HttpGet("Disponibles")]
        public async Task<IActionResult> Disponibles([FromQuery] DateTime fechaEntrada, [FromQuery] DateTime fechaSalida)
        {
            var respuesta = await _habitacionServices.GetDisponiblesAsync(fechaEntrada, fechaSalida);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener las habitaciones disponibles: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // DETALLES DE UNA HABITACIÓN
        // GET: api/Habitacion/5
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _habitacionServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener la habitación {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // CREAR HABITACIÓN
        // POST: api/Habitacion/CreateHabitacion
        // ---------------------------------------------------------
        [HttpPost("CreateHabitacion")]
        public async Task<IActionResult> Post([FromBody] HabitacionAdd habitacion)
        {
            var respuesta = await _habitacionServices.AddAsync(habitacion);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // EDITAR HABITACIÓN
        // POST: api/Habitacion/ModifyHabitacion
        // ---------------------------------------------------------
        [HttpPost("ModifyHabitacion")]
        public async Task<IActionResult> Put([FromBody] HabitacionUpdate habitacion)
        {
            var respuesta = await _habitacionServices.UpdateAsync(habitacion);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // ELIMINAR HABITACIÓN
        // POST: api/Habitacion/DisableHabitacion
        // ---------------------------------------------------------
        [HttpPost("DisableHabitacion")]
        public async Task<IActionResult> DisableHabitacion([FromBody] HabitacionRemove habitacion)
        {
            var respuesta = await _habitacionServices.DeleteAsync(habitacion);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
