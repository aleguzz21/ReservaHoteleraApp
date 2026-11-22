using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Pago;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Pagos".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class PagoController : ControllerBase
    {
        // Servicio de negocio de los pagos.
        private readonly IPagoServices _pagoServices;
        private readonly ILogger<PagoController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public PagoController(IPagoServices pagoServices, ILogger<PagoController> logger)
        {
            _pagoServices = pagoServices;
            _logger = logger;
        }

        // ---------------------------------------------------------
        // LISTAR PAGOS
        // GET: api/Pago
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos los pagos al servicio.
            var respuesta = await _pagoServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener los pagos: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Si todo salió bien, devolvemos la lista con código 200.
            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // DETALLES DE UN PAGO
        // GET: api/Pago/5
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _pagoServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el pago {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // CREAR PAGO
        // POST: api/Pago/CreatePago
        // ---------------------------------------------------------
        [HttpPost("CreatePago")]
        public async Task<IActionResult> Post([FromBody] PagoAdd pago)
        {
            var respuesta = await _pagoServices.AddAsync(pago);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // EDITAR PAGO
        // POST: api/Pago/ModifyPago
        // ---------------------------------------------------------
        [HttpPost("ModifyPago")]
        public async Task<IActionResult> Put([FromBody] PagoUpdate pago)
        {
            var respuesta = await _pagoServices.UpdateAsync(pago);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // ELIMINAR PAGO
        // POST: api/Pago/DisablePago
        // ---------------------------------------------------------
        [HttpPost("DisablePago")]
        public async Task<IActionResult> DisablePago([FromBody] PagoRemove pago)
        {
            var respuesta = await _pagoServices.DeleteAsync(pago);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
