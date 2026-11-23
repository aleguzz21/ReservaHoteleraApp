using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Hoteles".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        // Servicio de negocio de los hoteles.
        private readonly IHotelServices _hotelServices;
        private readonly ILogger<HotelController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public HotelController(IHotelServices hotelServices, ILogger<HotelController> logger)
        {
            _hotelServices = hotelServices;
            _logger = logger;
        }

        // LISTAR HOTELES
        // GET: api/Hotel
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos los hoteles al servicio.
            var respuesta = await _hotelServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener los hoteles: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Si todo salió bien, devolvemos la lista con código 200.
            return Ok(respuesta);
        }

        // BUSCAR HOTELES POR DESTINO
        // GET: api/Hotel/Buscar?destino=Santo
        [HttpGet("Buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string? destino)
        {
            var respuesta = await _hotelServices.BuscarAsync(destino);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al buscar hoteles: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // DETALLES DE UN HOTEL
        // GET: api/Hotel/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _hotelServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el hotel {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // CREAR HOTEL
        // POST: api/Hotel/CreateHotel
        [HttpPost("CreateHotel")]
        public async Task<IActionResult> Post([FromBody] HotelAdd hotel)
        {
            var respuesta = await _hotelServices.AddAsync(hotel);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // EDITAR HOTEL
        // POST: api/Hotel/ModifyHotel
        [HttpPost("ModifyHotel")]
        public async Task<IActionResult> Put([FromBody] HotelUpdate hotel)
        {
            var respuesta = await _hotelServices.UpdateAsync(hotel);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ELIMINAR HOTEL
        // POST: api/Hotel/DisableHotel
        [HttpPost("DisableHotel")]
        public async Task<IActionResult> DisableHotel([FromBody] HotelRemove hotel)
        {
            var respuesta = await _hotelServices.DeleteAsync(hotel);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }
    }
}
