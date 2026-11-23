using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Cliente;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Clientes".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        // Servicio de negocio de los clientes.
        private readonly IClienteServices _clienteServices;
        private readonly ILogger<ClienteController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public ClienteController(IClienteServices clienteServices, ILogger<ClienteController> logger)
        {
            _clienteServices = clienteServices;
            _logger = logger;
        }

        // LISTAR CLIENTES
        // GET: api/Cliente
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos los clientes al servicio.
            var respuesta = await _clienteServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener los clientes: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Si todo salió bien, devolvemos la lista con código 200.
            return Ok(respuesta);
        }

        // DETALLES DE UN CLIENTE
        // GET: api/Cliente/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _clienteServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el cliente {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // CLIENTE ASOCIADO A UN USUARIO
        // GET: api/Cliente/PorUsuario/5
        [HttpGet("PorUsuario/{idUsuario}")]
        public async Task<IActionResult> PorUsuario(int idUsuario)
        {
            var respuesta = await _clienteServices.GetByUsuarioIdAsync(idUsuario);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el cliente del usuario {IdUsuario}: {ErrorMessage}", idUsuario, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // CREAR CLIENTE
        // POST: api/Cliente/CreateCliente
        [HttpPost("CreateCliente")]
        public async Task<IActionResult> Post([FromBody] ClienteAdd cliente)
        {
            var respuesta = await _clienteServices.AddAsync(cliente);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // EDITAR CLIENTE
        // POST: api/Cliente/ModifyCliente
        [HttpPost("ModifyCliente")]
        public async Task<IActionResult> Put([FromBody] ClienteUpdate cliente)
        {
            var respuesta = await _clienteServices.UpdateAsync(cliente);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ELIMINAR CLIENTE
        // POST: api/Cliente/DisableCliente
        [HttpPost("DisableCliente")]
        public async Task<IActionResult> DisableCliente([FromBody] ClienteRemove cliente)
        {
            var respuesta = await _clienteServices.DeleteAsync(cliente);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
