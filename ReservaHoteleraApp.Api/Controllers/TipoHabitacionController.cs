using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Tipos de habitación".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class TipoHabitacionController : ControllerBase
    {
        // Servicio de negocio de los tipos de habitación.
        private readonly ITipoHabitacionServices _tipoHabitacionServices;
        private readonly ILogger<TipoHabitacionController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public TipoHabitacionController(ITipoHabitacionServices tipoHabitacionServices, ILogger<TipoHabitacionController> logger)
        {
            _tipoHabitacionServices = tipoHabitacionServices;
            _logger = logger;
        }

        // LISTAR TIPOS DE HABITACIÓN
        // GET: api/TipoHabitacion
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos los tipos de habitación al servicio.
            var respuesta = await _tipoHabitacionServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener los tipos de habitación: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Si todo salió bien, devolvemos la lista con código 200.
            return Ok(respuesta);
        }

        // DETALLES DE UN TIPO DE HABITACIÓN
        // GET: api/TipoHabitacion/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _tipoHabitacionServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el tipo de habitación {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // CREAR TIPO DE HABITACIÓN
        // POST: api/TipoHabitacion/CreateTipoHabitacion
        [HttpPost("CreateTipoHabitacion")]
        public async Task<IActionResult> Post([FromBody] TipoHabitacionAdd tipoHabitacion)
        {
            var respuesta = await _tipoHabitacionServices.AddAsync(tipoHabitacion);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // EDITAR TIPO DE HABITACIÓN
        // POST: api/TipoHabitacion/ModifyTipoHabitacion
        [HttpPost("ModifyTipoHabitacion")]
        public async Task<IActionResult> Put([FromBody] TipoHabitacionUpdate tipoHabitacion)
        {
            var respuesta = await _tipoHabitacionServices.UpdateAsync(tipoHabitacion);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ELIMINAR TIPO DE HABITACIÓN
        // POST: api/TipoHabitacion/DisableTipoHabitacion
        [HttpPost("DisableTipoHabitacion")]
        public async Task<IActionResult> DisableTipoHabitacion([FromBody] TipoHabitacionRemove tipoHabitacion)
        {
            var respuesta = await _tipoHabitacionServices.DeleteAsync(tipoHabitacion);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
