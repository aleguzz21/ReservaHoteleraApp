using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;

namespace ReservaHoteleraApp.Api.Controllers
{
    // Controlador de la API del módulo "Usuarios".
    // Recibe las peticiones HTTP, llama al servicio y devuelve la respuesta en formato JSON.
    // El controlador NO habla directamente con la base de datos.
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        // Servicio de negocio de los usuarios.
        private readonly IUsuarioServices _usuarioServices;
        private readonly ILogger<UsuarioController> _logger;

        // ASP.NET Core nos entrega el servicio ya creado (inyección de dependencias).
        public UsuarioController(IUsuarioServices usuarioServices, ILogger<UsuarioController> logger)
        {
            _usuarioServices = usuarioServices;
            _logger = logger;
        }

        // ---------------------------------------------------------
        // LISTAR USUARIOS (sin mostrar la contraseña)
        // GET: api/Usuario
        // ---------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 1. Pedimos los usuarios al servicio.
            var respuesta = await _usuarioServices.GetAllAsync();

            // 2. Si hubo un error, devolvemos el mensaje con código 400.
            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener los usuarios: {ErrorMessage}", respuesta.Message);
                return BadRequest(respuesta);
            }

            // 3. Convertimos cada usuario a un objeto sin contraseña.
            var listaUsuarios = new List<UsuarioRespuesta>();

            if (respuesta.Data == null)
            {
                return Ok(new { respuesta.Success, respuesta.Message, Data = listaUsuarios });
            }

            foreach (var usuario in respuesta.Data)
            {
                listaUsuarios.Add(ConvertirUsuario(usuario));
            }

            // 4. Devolvemos la lista con código 200.
            return Ok(new { respuesta.Success, respuesta.Message, Data = listaUsuarios });
        }

        // ---------------------------------------------------------
        // DETALLES DE UN USUARIO (sin mostrar la contraseña)
        // GET: api/Usuario/5
        // ---------------------------------------------------------
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var respuesta = await _usuarioServices.GetByIdAsync(id);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el usuario {Id}: {ErrorMessage}", id, respuesta.Message);
                return BadRequest(respuesta);
            }

            UsuarioRespuesta? usuarioRespuesta = null;

            if (respuesta.Data == null)
            {
                return Ok(new { respuesta.Success, respuesta.Message, Data = usuarioRespuesta });
            }

            usuarioRespuesta = ConvertirUsuario(respuesta.Data);

            return Ok(new { respuesta.Success, respuesta.Message, Data = usuarioRespuesta });
        }

        // ---------------------------------------------------------
        // USUARIO POR NOMBRE DE USUARIO (sin mostrar la contraseña)
        // GET: api/Usuario/PorNombre/AlexGuzman
        // ---------------------------------------------------------
        [HttpGet("PorNombre/{nombreUsuario}")]
        public async Task<IActionResult> PorNombre(string nombreUsuario)
        {
            var respuesta = await _usuarioServices.GetByNombreUsuarioAsync(nombreUsuario);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al obtener el usuario {NombreUsuario}: {ErrorMessage}", nombreUsuario, respuesta.Message);
                return BadRequest(respuesta);
            }

            UsuarioRespuesta? usuarioRespuesta = null;

            if (respuesta.Data == null)
            {
                return Ok(new { respuesta.Success, respuesta.Message, Data = usuarioRespuesta });
            }

            usuarioRespuesta = ConvertirUsuario(respuesta.Data);

            return Ok(new { respuesta.Success, respuesta.Message, Data = usuarioRespuesta });
        }

        // ---------------------------------------------------------
        // VALIDAR CREDENCIALES
        // POST: api/Usuario/ValidarCredenciales
        // ---------------------------------------------------------
        [HttpPost("ValidarCredenciales")]
        public async Task<IActionResult> ValidarCredenciales([FromBody] ValidarCredencialesRequest request)
        {
            var respuesta = await _usuarioServices.ValidarCredencialesAsync(request.NombreUsuario, request.Contrasena);

            if (respuesta.Success == false)
            {
                _logger.LogError("Error al validar las credenciales del usuario {NombreUsuario}: {ErrorMessage}", request.NombreUsuario, respuesta.Message);
                return BadRequest(respuesta);
            }

            UsuarioRespuesta? usuarioRespuesta = null;

            if (respuesta.Data == null)
            {
                return Ok(new { respuesta.Success, respuesta.Message, Data = usuarioRespuesta });
            }

            usuarioRespuesta = ConvertirUsuario(respuesta.Data);

            return Ok(new { respuesta.Success, respuesta.Message, Data = usuarioRespuesta });
        }

        // ---------------------------------------------------------
        // CREAR USUARIO
        // POST: api/Usuario/CreateUsuario
        // ---------------------------------------------------------
        [HttpPost("CreateUsuario")]
        public async Task<IActionResult> Post([FromBody] UsuarioAdd usuario)
        {
            var respuesta = await _usuarioServices.AddAsync(usuario);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // EDITAR USUARIO
        // POST: api/Usuario/ModifyUsuario
        // ---------------------------------------------------------
        [HttpPost("ModifyUsuario")]
        public async Task<IActionResult> Put([FromBody] UsuarioUpdate usuario)
        {
            var respuesta = await _usuarioServices.UpdateAsync(usuario);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // ---------------------------------------------------------
        // ELIMINAR USUARIO
        // POST: api/Usuario/DisableUsuario
        // ---------------------------------------------------------
        [HttpPost("DisableUsuario")]
        public async Task<IActionResult> DisableUsuario([FromBody] UsuarioRemove usuario)
        {
            var respuesta = await _usuarioServices.DeleteAsync(usuario);

            if (respuesta.Success == false)
            {
                return BadRequest(respuesta);
            }

            return Ok(respuesta);
        }

        // Convierte un usuario de la base de datos a un objeto sin contraseña.
        private UsuarioRespuesta ConvertirUsuario(UsuarioModel usuario)
        {
            var usuarioRespuesta = new UsuarioRespuesta();
            usuarioRespuesta.Id = usuario.Id;
            usuarioRespuesta.NombreUsuario = usuario.NombreUsuario;
            usuarioRespuesta.Rol = usuario.Rol;
            usuarioRespuesta.Activo = usuario.Activo;
            return usuarioRespuesta;
        }
    }

    // Datos del usuario que se pueden mostrar (sin la contraseña).
    public class UsuarioRespuesta
    {
        public int Id { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Rol { get; set; }
        public bool Activo { get; set; }
    }

    // Datos que se reciben para validar las credenciales.
    public class ValidarCredencialesRequest
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}
