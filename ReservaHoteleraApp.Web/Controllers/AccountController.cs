using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.Web.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.DAL.Models.Objects.Cliente;
using Microsoft.EntityFrameworkCore.Internal;

namespace ReservaHoteleraApp.Web.Controllers
{
    // controlador de la cuenta: inicia sesion, registrarse y cerrar sesion
    public class AccountController : Controller
    {
        private readonly IUsuarioServices _usuarioServices;
        private readonly IClienteServices _clienteServices;

        // ASP.net core nos entrega los servicios ya creados
        // (inyeccion de dependencias)

        public AccountController(IUsuarioServices usuarioServices, IClienteServices clienteServices)
        {
            _usuarioServices = usuarioServices;
            _clienteServices = clienteServices;

        }
        // mostrar el formulario de inicio de sesion
        [HttpGet]
        public IActionResult Login()
        { 
        return View(new LoginViewModel());
        }

        // procesar el formulario de inicio de sesion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // 1. si el formulario no es valido (campos vacios), volvemos a mostrar el formulario
            // con los errores
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 2. validar el usuario y la contrasena (contrasena esta hasheada en BD)
            var respuesta = await _usuarioServices.ValidarCredencialesAsync(model.NombreUsuario, model.Contrasena);

            if (respuesta.Success == false || respuesta.Data == null) 
            { 
                ModelState.AddModelError(string.Empty, respuesta.Message?? "Usuario o contrasena incorrectos");
                return View(model);
            }

            var usuario = respuesta.Data;
            // 3. crear las claims (informacion del usuario que se guarda en la cookie)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario?? ""),
                new Claim(ClaimTypes.Role, usuario.Rol?? "Cliente")
            };

            // 4. Armammos el identity y el principal (quien es el usuario)
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // 5. Creamos la cookie de autenticacion. IsPersistent = model.Recordarme indica si la cookie
            // se guarda en el navegador o se borra al cerrar el navegador

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
               IsPersistent = model.Recordarme
            });

            // 6. redirigimos al usuario a la pagina de inicio
            return RedirectToAction("Index", "Home");
        }
        // mostrar el formulario de registro
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        // procesar el formulario de registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // 1. validaciones del viewmodel (campos vacios, contrasenas no coinciden, etc)

            if (ModelState.IsValid == false)
            {
                return View(model);
            }
            // 2. creamos el usuario. El rol siempre es "Cliente" cuando se registra desde la web.
            // (no viene del formulario)
            var usuario = new UsuarioAdd
            {
                NombreUsuario = model.NombreUsuario,
                Contrasena = model.Contrasena,
                Rol = "Cliente",
                Activo = true
            };

            var usuarioResult = await _usuarioServices.AddAsync(usuario);
            if (usuarioResult.Success == false)
            { 
            ModelState.AddModelError(string.Empty, usuarioResult.Message ?? "Error al crear el usuario");
                return View(model);
            }
            // 3. creamos el cliente (informacion personal del usuario)
            var usuarioCreado = await _usuarioServices.GetByNombreUsuarioAsync(model.NombreUsuario);
            if (usuarioCreado.Success == false || usuarioCreado.Data == null)
            {
                ModelState.AddModelError(string.Empty, "El usuario se creo pero no se puo recuperar");
                return View(model);
            }

            //4. creamos el cliente (informacion personal del usuario)
            var cliente = new ClienteAdd
            {
                NombreCliente = model.NombreCliente,
                Documento = model.Documento,
                Correo = model.Correo,
                Telefono = model.Telefono,
                IdUsuario = usuarioCreado.Data.Id
            };

            var clienteResult = await _clienteServices.AddAsync(cliente);
            if (clienteResult.Success == false)
            {
                //5. si hubo un error al crear el cliente, eliminamos el usuario creado para no dejar datos inconsistentes
                await _usuarioServices.DeleteAsync(new UsuarioRemove { Id = usuarioCreado.Data.Id });

                ModelState.AddModelError(string.Empty, clienteResult.Message ?? "Error al crear el cliente");
                return View(model);
            }
            // 6. redirigimos al usuario a la pagina de inicio de sesion

            TempData["Success"] = "Usuario registrado correctamente. Inicie sesion para continuar.";
            return RedirectToAction(nameof(Login));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // mostrar la pagina de acceso denegado

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }


}