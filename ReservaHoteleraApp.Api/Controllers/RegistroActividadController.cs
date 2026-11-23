using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.RegistroActividad;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Registro de actividades".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class RegistroActividadController : ControllerBase
    {
        // Servicio de negocio del registro de actividades.
        private readonly IRegistroActividadServices _registroActividadServices;
        private readonly ILogger<RegistroActividadController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public RegistroActividadController(IRegistroActividadServices registroActividadServices, ILogger<RegistroActividadController> logger)
        {
            _registroActividadServices = registroActividadServices;
            _logger = logger;
        }

        // LISTAR REGISTROS DE ACTIVIDAD
        // GET: api/RegistroActividad
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos los registros al servicio.
            var respuesta = await _registroActividadServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el registro de actividades: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Si todo salió bien, devolvemos la lista con código 200.
            return Ok(respuesta);
        }

        // DETALLES DE UN REGISTRO DE ACTIVIDAD
        // GET: api/RegistroActividad/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _registroActividadServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el registro de actividad {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // CREAR REGISTRO DE ACTIVIDAD
        // POST: api/RegistroActividad/CreateRegistroActividad
        [HttpPost("CreateRegistroActividad")]
        public async Task<IActionResult> Post([FromBody] RegistroActividadAdd registroActividad)
        {
            var respuesta = await _registroActividadServices.AddAsync(registroActividad);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // EDITAR REGISTRO DE ACTIVIDAD
        // POST: api/RegistroActividad/ModifyRegistroActividad
        [HttpPost("ModifyRegistroActividad")]
        public async Task<IActionResult> Put([FromBody] RegistroActividadUpdate registroActividad)
        {
            var respuesta = await _registroActividadServices.UpdateAsync(registroActividad);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ELIMINAR REGISTRO DE ACTIVIDAD
        // POST: api/RegistroActividad/DisableRegistroActividad
        [HttpPost("DisableRegistroActividad")]
        public async Task<IActionResult> DisableRegistroActividad([FromBody] RegistroActividadRemove registroActividad)
        {
            var respuesta = await _registroActividadServices.DeleteAsync(registroActividad);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
