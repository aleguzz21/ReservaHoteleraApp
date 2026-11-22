using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Piso;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Pisos".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class PisoController : ControllerBase
    {
        // Servicio de negocio de los pisos.
        private readonly IPisoServices _pisoServices;
        private readonly ILogger<PisoController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public PisoController(IPisoServices pisoServices, ILogger<PisoController> logger)
        {
            _pisoServices = pisoServices;
            _logger = logger;
        }

        // ---------------------------------------------------------
        // LISTAR PISOS
        // GET: api/Piso
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos los pisos al servicio.
            var respuesta = await _pisoServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener los pisos: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Si todo salió bien, devolvemos la lista con código 200.
            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // DETALLES DE UN PISO
        // GET: api/Piso/5
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _pisoServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el piso {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // CREAR PISO
        // POST: api/Piso/CreatePiso
        // ---------------------------------------------------------
        [HttpPost("CreatePiso")]
        public async Task<IActionResult> Post([FromBody] PisoAdd piso)
        {
            var respuesta = await _pisoServices.AddAsync(piso);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // EDITAR PISO
        // POST: api/Piso/ModifyPiso
        // ---------------------------------------------------------
        [HttpPost("ModifyPiso")]
        public async Task<IActionResult> Put([FromBody] PisoUpdate piso)
        {
            var respuesta = await _pisoServices.UpdateAsync(piso);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // ELIMINAR PISO
        // POST: api/Piso/DisablePiso
        // ---------------------------------------------------------
        [HttpPost("DisablePiso")]
        public async Task<IActionResult> DisablePiso([FromBody] PisoRemove piso)
        {
            var respuesta = await _pisoServices.DeleteAsync(piso);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
