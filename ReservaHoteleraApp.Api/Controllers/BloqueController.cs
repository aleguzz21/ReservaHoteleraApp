using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Bloque;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Bloques".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class BloqueController : ControllerBase
    {
        // Servicio de negocio de los bloques.
        private readonly IBloqueServices _bloqueServices;
        private readonly ILogger<BloqueController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public BloqueController(IBloqueServices bloqueServices, ILogger<BloqueController> logger)
        {
            _bloqueServices = bloqueServices;
            _logger = logger;
        }

        // ---------------------------------------------------------
        // LISTAR BLOQUES
        // GET: api/Bloque
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos los bloques al servicio.
            var respuesta = await _bloqueServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener los bloques: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Si todo salió bien, devolvemos la lista con código 200.
            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // DETALLES DE UN BLOQUE
        // GET: api/Bloque/5
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _bloqueServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el bloque {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // CREAR BLOQUE
        // POST: api/Bloque/CreateBloque
        // ---------------------------------------------------------
        [HttpPost("CreateBloque")]
        public async Task<IActionResult> Post([FromBody] BloqueAdd bloque)
        {
            var respuesta = await _bloqueServices.AddAsync(bloque);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // EDITAR BLOQUE
        // POST: api/Bloque/ModifyBloque
        // ---------------------------------------------------------
        [HttpPost("ModifyBloque")]
        public async Task<IActionResult> Put([FromBody] BloqueUpdate bloque)
        {
            var respuesta = await _bloqueServices.UpdateAsync(bloque);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // ELIMINAR BLOQUE
        // POST: api/Bloque/DisableBloque
        // ---------------------------------------------------------
        [HttpPost("DisableBloque")]
        public async Task<IActionResult> DisableBloque([FromBody] BloqueRemove bloque)
        {
            var respuesta = await _bloqueServices.DeleteAsync(bloque);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
