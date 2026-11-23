using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Estadia;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Estadías".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class EstadiaController : ControllerBase
    {
        // Servicio de negocio de las estadías.
        private readonly IEstadiaServices _estadiaServices;
        private readonly ILogger<EstadiaController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public EstadiaController(IEstadiaServices estadiaServices, ILogger<EstadiaController> logger)
        {
            _estadiaServices = estadiaServices;
            _logger = logger;
        }

        // LISTAR ESTADÍAS
        // GET: api/Estadia
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos las estadías al servicio.
            var respuesta = await _estadiaServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener las estadías: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Si todo salió bien, devolvemos la lista con código 200.
            return Ok(respuesta);
        }

        // DETALLES DE UNA ESTADÍA
        // GET: api/Estadia/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _estadiaServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener la estadía {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // CREAR ESTADÍA
        // POST: api/Estadia/CreateEstadia
        [HttpPost("CreateEstadia")]
        public async Task<IActionResult> Post([FromBody] EstadiaAdd estadia)
        {
            var respuesta = await _estadiaServices.AddAsync(estadia);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // EDITAR ESTADÍA
        // POST: api/Estadia/ModifyEstadia
        [HttpPost("ModifyEstadia")]
        public async Task<IActionResult> Put([FromBody] EstadiaUpdate estadia)
        {
            var respuesta = await _estadiaServices.UpdateAsync(estadia);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ELIMINAR ESTADÍA
        // POST: api/Estadia/DisableEstadia
        [HttpPost("DisableEstadia")]
        public async Task<IActionResult> DisableEstadia([FromBody] EstadiaRemove estadia)
        {
            var respuesta = await _estadiaServices.DeleteAsync(estadia);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
