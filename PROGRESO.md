# Progreso del proyecto — Diseño Home (A&V Solutions)

**Fecha de este registro:** 15/11/2026
**Proyecto:** `C:\Users\DELL\source\repos\ReservaHoteleraApp`
**Solución:** `ReservaHoteleraApp.slnx` (.NET 10, ASP.NET Core MVC, capas Web → BL → DAL)

Este documento guarda el avance de la parte de **diseño** y los cambios que se hicieron
para poder continuar en otra ocasión sin empezar de nuevo.

---

## 1. Objetivo de la sesión

- Darle a la página de inicio un formato tipo Booking.com.
- Usar los colores del logo de A&V Solutions.
- Respetar las clases y métodos existentes.
- Mantener la arquitectura por capas y aplicar principios SOLID.
- Dejar el código fácil de leer para explicarlo al maestro.

---

## 2. Estado actual (lo que ya está hecho)

### 2.1 Página de inicio
**Archivo:** `ReservaHoteleraApp.Web\Views\Home\Index.cshtml`

- Hero verde oscuro con el título **"Encuentra tu próxima estancia"**.
- Subtítulo actual (editado por el usuario):
  **"Busca ofertas en los hoteles mejores valorados"**.
- Buscador con:
  - Destino (texto).
  - Fecha de entrada y fecha de salida (`type="date"`).
  - Ocupación (adultos, niños, habitaciones).
  - Botón **Buscar**.
- Checkbox "Viajo por trabajo".
- Sección **"¿Por qué elegir A&V Solutions?"** con 4 tarjetas.
- Sección **"Alojamientos disponibles"** con tarjetas generadas desde `HotelModel`.
  - El enlace **"Ver todos"** quedó **comentado** con `@* ... *@` (no eliminado).
- El modelo de la vista es `IEnumerable<HotelModel>`.

### 2.2 Encabezado y pie
**Archivo:** `ReservaHoteleraApp.Web\Views\Shared\_Layout.cshtml`

- Cabecera verde oscuro estilo Booking.
- Logo solo (sin el texto "A&V Solutions"), a 50 px, dentro de un recuadro blanco.
- Botones/link de la cabecera:
  - `DOP` con bandera.
  - "Registra tu alojamiento" → `Hotel/Create`.
  - "Ver hoteles" → `Hotel/Index`.
- Menú ordenado según la **pirámide de dependencias**:
  1. Inicio
  2. Hoteles
  3. Bloques
  4. Pisos
  5. Tipos de Habitación
  6. Habitaciones
- Cada enlace tiene un `title` que indica su nivel.
- Pie de página: `© 2026 - A&V Solutions - Privacidad`.

### 2.3 Paleta de colores del logo
**Archivo:** `ReservaHoteleraApp.Web\wwwroot\css\site.css`

Colores tomados directamente del PNG del logo:

| Uso | Color |
|---|---|
| Verde oscuro (cabecera/hero) | `#06482c` |
| Verde (acentos/buscador) | `#279b2c` |
| Azul (botones/enlaces) | `#057ac9` |
| Fondo claro de iconos | `#e8f5e9` |

- Ya no quedan colores azules de Booking (`#003580`, `#006ce4`).
- Incluye estilos responsivos para el buscador, la cabecera y las tarjetas.

### 2.4 Restricciones de fechas
**Archivos:** `Views\Home\Index.cshtml` y `wwwroot\js\site.js`

- En el bloque Razor se calcula:
  `var hoy = DateTime.Today.ToString("yyyy-MM-dd");`
- Los dos `input type="date"` tienen `min="@hoy"`, así el calendario no permite
  fechas anteriores a hoy.
- En `site.js`:
  - Al cambiar la fecha de entrada, se actualiza `fechaSalida.min`.
  - Si la salida quedó antes que la entrada, se limpia.
- Nota: esto es validación del lado del cliente (experiencia de usuario).
  Falta validación en el servidor cuando se implementen reservas.

### 2.5 Controladores simplificados
**Archivos:**
- `ReservaHoteleraApp.Web\Controllers\HomeController.cs`
- `ReservaHoteleraApp.Web\Controllers\HotelController.cs`

- Comentarios en español, paso a paso.
- `if (respuesta.Success == false)` y `== null` en lugar de `!`, `??`, `is null`.
- Nombres claros: `respuesta`, `hoteles`, `hotelParaEditar`, `hotelParaEliminar`.
- `HomeController.Index` obtiene los hoteles con `IHotelServices.GetAllAsync()`.
- `HotelController.Index` recibe los datos del buscador y llama a
  `IHotelServices.BuscarAsync(destino)`.

### 2.6 SOLID y arquitectura
**Archivos:**
- `ReservaHoteleraApp.BL\Services\InterfacesBL\IHotelServices.cs`
- `ReservaHoteleraApp.BL\Services\ImplementationsBL\HotelServices.cs`

- Se agregó `BuscarAsync(string? destino)` a la interfaz y su implementación en BL.
- La búsqueda por destino (regla de negocio) ya **no está en el controlador**.
- Principios aplicados:
  - **SRP:** el controlador solo coordina; BL tiene la regla de búsqueda.
  - **OCP:** nuevos criterios de búsqueda se agregan en BL, no en el controlador.
  - **DIP:** el controlador depende de `IHotelServices`; BL depende de `IHotelRepository`.
  - **ISP:** `IHotelServices` expone solo operaciones de hoteles.
  - **Capas:** Web → BL → DAL → Base de datos.

---

## 3. Archivos modificados en esta etapa

```
ReservaHoteleraApp.Web\Views\Home\Index.cshtml
ReservaHoteleraApp.Web\Views\Shared\_Layout.cshtml
ReservaHoteleraApp.Web\wwwroot\css\site.css
ReservaHoteleraApp.Web\wwwroot\js\site.js
ReservaHoteleraApp.Web\Controllers\HomeController.cs
ReservaHoteleraApp.Web\Controllers\HotelController.cs
ReservaHoteleraApp.BL\Services\InterfacesBL\IHotelServices.cs
ReservaHoteleraApp.BL\Services\ImplementationsBL\HotelServices.cs
```

---

## 4. Cómo ejecutar y verificar

1. Detener la app en Visual Studio (`Shift + F5`) antes de compilar,
   para evitar el bloqueo de archivos en `bin`.
2. Compilar: `Ctrl + Shift + B`.
3. Ejecutar: `F5`.
4. Rutas para probar:
   - `/` → página de inicio.
   - `/Hotel` → lista de hoteles.
   - `/Hotel?destino=Santo` → búsqueda por destino.

Comando alternativo por consola (desde la carpeta de la solución):

```powershell
dotnet build ReservaHoteleraApp.slnx
dotnet run --project ReservaHoteleraApp.Web\ReservaHoteleraApp.Web.csproj --urls http://localhost:5199
```

---

## 5. Pendientes / próximos pasos

- [ ] Decidir si "Ver todos" se elimina definitivamente o se mueve a otro lugar
      (actualmente está comentado en `Views\Home\Index.cshtml`).
- [ ] Validar fechas en el servidor cuando se implemente el guardado de reservas.
- [ ] Las fechas y la ocupación todavía **no filtran**; por ahora solo filtra el destino.
- [ ] Crear controladores y vistas para Reserva, Estadía, Pago y Cliente
      (existen en BL/DAL pero no en Web).
- [ ] Unificar estilos de botones, tablas, formularios y mensajes de alerta
      (pendiente de la sección 16.3 de la bitácora).
- [ ] Definir tipografía institucional.
- [ ] Revisar si se aplica el mismo estilo de SOLID a los demás controladores.

---

## 6. Notas y decisiones

- No se modificó la base de datos ni las entidades.
- No se agregaron clases nuevas de negocio; se reutilizó `IHotelServices`.
- El logo y la identidad visual se tomaron de la guía de la bitácora
  (`Documents\Bitacora_ReservaHoteleraApp\BITACORA_CapaPresentacion.md`, sección 16.2).
- El build normal puede fallar con "archivo bloqueado" si Visual Studio tiene
  la app en ejecución. En ese caso, detener la depuración o compilar en otra carpeta.

---

## 7. Historial de avance

- **15/11/2026:** Diseño del home tipo Booking con la paleta del logo
  (verde `#06482c`, verde `#279b2c`, azul `#057ac9`).
- **15/11/2026:** Cabecera/footer y menú ordenado por la pirámide
  Hotel → Bloque → Piso → Tipos de Habitación → Habitaciones.
- **15/11/2026:** Controladores simplificados y búsqueda movida a la capa BL (SOLID).
- **15/11/2026:** Restricciones de fechas con `min` y JavaScript.
- **15/11/2026:** Ajustes del usuario: nuevo subtítulo y "Ver todos" comentado.

---

# Parte 2 — Autenticación, roles y reservas

**Fecha de este registro:** 15/11/2026
**Estado actual:** Fase 2 completada; Fase 3 en curso (ViewModels y `AccountController` listos, faltan las vistas). Ver Parte 3.
**Cómo retomar:** leer la sección 21 (qué falta) y la Parte 3 (sección 20 en adelante).

## 8. Objetivo de esta etapa

- Login y registro de usuarios.
- Roles:
  - `Administrador`: puede crear, editar y eliminar hoteles, bloques, pisos, tipos de habitación y habitaciones.
  - `Cliente`: solo puede reservar.
- Los clientes autenticados pueden reservar habitaciones.
- No se modificó el esquema de la base de datos.

## 9. Decisiones técnicas

- Autenticación por **cookies** con roles (no ASP.NET Core Identity).
- Contraseñas con `PasswordHasher<Usuario>` del paquete `Microsoft.Extensions.Identity.Core`.
- El rol se guarda en un *claim* al iniciar sesión.
- El registro siempre crea usuarios con rol `Cliente`; el administrador se crea actualizando la BD.
- El total de la reserva y la disponibilidad se validan en el servidor.
- El controlador no habla con la base de datos: siempre pasa por BL.

## 10. Fase 1 — COMPLETADA

### Archivos modificados

- `ReservaHoteleraApp.BL\ReservaHoteleraApp.BL.csproj`
  - `Microsoft.Extensions.Identity.Core` fijado en `10.0.13` (la 11.0.0 causaba `NU1605`).
- `ReservaHoteleraApp.DAL\Repositories\InterfacesDAL\IUsuarioRepository.cs`
  - `GetByNombreUsuarioAsync`.
- `ReservaHoteleraApp.DAL\Repositories\ImplementationsDAL\UsuarioRepository.cs`
  - Implementación de `GetByNombreUsuarioAsync`.
- `ReservaHoteleraApp.BL\Services\InterfacesBL\IUsuarioServices.cs`
  - `GetByNombreUsuarioAsync` y `ValidarCredencialesAsync`.
- `ReservaHoteleraApp.BL\Services\ImplementationsBL\UsuarioServices.cs`
  - `using Microsoft.AspNetCore.Identity;`
  - `using ReservaHoteleraApp.DAL.Models.Result;`
  - `private readonly PasswordHasher<Usuario> _passwordHasher = new();`
  - Hash de la contraseña en `AddAsync` antes de guardar.
  - `GetByNombreUsuarioAsync` y `ValidarCredencialesAsync`.
- `ReservaHoteleraApp.BL.Tests\UsuarioServicesTests.cs`
  - `GetByNombreUsuarioAsync` agregado a `FakeUsuarioRepository` y `FailingUsuarioRepository`.

### Verificación

```powershell
dotnet build ReservaHoteleraApp.slnx
```

Resultado: **0 errores, 0 advertencias**.

### Notas

- Los usuarios viejos con contraseña en texto plano ya no podrán iniciar sesión.
- `UpdateAsync` de usuarios no hashea; no usar el formulario de edición para cambiar contraseñas.
- Si Visual Studio tiene la app corriendo, detener la depuración antes de compilar.

---

## 11. Fase 2 — Cookies en Program.cs (COMPLETADA — 16/11/2026)

**Archivo:** `ReservaHoteleraApp.Web\Program.cs`

### 11.1 Agregar el using arriba

```csharp
using Microsoft.AspNetCore.Authentication.Cookies;
```

### 11.2 Después de `builder.Services.AddControllersWithViews();`

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
```

### 11.3 En el pipeline, antes de `app.UseAuthorization();`

```csharp
app.UseAuthentication();
```

Debe quedar así:

```csharp
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
```

### Verificación

Compila. Todavía no hay login, pero la app debe seguir funcionando.

**Resultado (16/11/2026):** completado en `Program.cs`.
- Se agregó `using Microsoft.AspNetCore.Authentication.Cookies;`.
- `AddAuthentication(...).AddCookie(...)` y `AddAuthorization()` después de `AddControllersWithViews()`.
- `app.UseAuthentication();` antes de `app.UseAuthorization();`.
- `dotnet build`: 0 errores, 0 advertencias.

---

## 12. Fase 3 — Registro y login (EN CURSO — 16/11/2026)

**Estado:** los dos ViewModels y el `AccountController` ya están creados y compilan.
Falta crear las vistas de `Views/Account` (ver Parte 3 al final del documento).

### 12.1 Crear `ReservaHoteleraApp.Web\Models\LoginViewModel.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace ReservaHoteleraApp.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public string NombreUsuario { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = "";

        public bool Recordarme { get; set; }
    }
}
```

### 12.2 Crear `ReservaHoteleraApp.Web\Models\RegisterViewModel.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace ReservaHoteleraApp.Web.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(50)]
        public string NombreUsuario { get; set; } = "";

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = "";

        [DataType(DataType.Password)]
        [Compare(nameof(Contrasena), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; } = "";

        [Required, StringLength(150)]
        public string NombreCliente { get; set; } = "";

        [Required, StringLength(30)]
        public string Documento { get; set; } = "";

        [Required, EmailAddress, StringLength(150)]
        public string Correo { get; set; } = "";

        [StringLength(25)]
        public string? Telefono { get; set; }
    }
}
```

### 12.3 Crear `ReservaHoteleraApp.Web\Controllers\AccountController.cs`

```csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Cliente;
using ReservaHoteleraApp.DAL.Models.Objects.Usuario;
using ReservaHoteleraApp.Web.Models;

namespace ReservaHoteleraApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioServices _usuarioServices;
        private readonly IClienteServices _clienteServices;

        public AccountController(IUsuarioServices usuarioServices, IClienteServices clienteServices)
        {
            _usuarioServices = usuarioServices;
            _clienteServices = clienteServices;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _usuarioServices.ValidarCredencialesAsync(
                model.NombreUsuario, model.Contrasena);

            if (!resultado.Success || resultado.Data is null)
            {
                ModelState.AddModelError("", resultado.Message ?? "Usuario o contraseña incorrectos.");
                return View(model);
            }

            var usuario = resultado.Data;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario ?? ""),
                new Claim(ClaimTypes.Role, usuario.Rol ?? "Cliente")
            };

            var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identidad),
                new AuthenticationProperties { IsPersistent = model.Recordarme });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var usuarioAdd = new UsuarioAdd
            {
                NombreUsuario = model.NombreUsuario,
                Contrasena = model.Contrasena,
                Rol = "Cliente",
                Activo = true
            };

            var usuarioResult = await _usuarioServices.AddAsync(usuarioAdd);
            if (!usuarioResult.Success)
            {
                ModelState.AddModelError("", usuarioResult.Message ?? "No se pudo crear el usuario.");
                return View(model);
            }

            var usuarioCreado = await _usuarioServices.GetByNombreUsuarioAsync(model.NombreUsuario);
            if (!usuarioCreado.Success || usuarioCreado.Data is null)
            {
                ModelState.AddModelError("", "El usuario se creó pero no se pudo recuperar.");
                return View(model);
            }

            var clienteAdd = new ClienteAdd
            {
                IdUsuario = usuarioCreado.Data.Id,
                NombreCliente = model.NombreCliente,
                Documento = model.Documento,
                Correo = model.Correo,
                Telefono = model.Telefono
            };

            var clienteResult = await _clienteServices.AddAsync(clienteAdd);
            if (!clienteResult.Success)
            {
                await _usuarioServices.DeleteAsync(new UsuarioRemove { Id = usuarioCreado.Data.Id });
                ModelState.AddModelError("", clienteResult.Message ?? "No se pudo crear el cliente.");
                return View(model);
            }

            TempData["Success"] = "Cuenta creada correctamente. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
```

### 12.4 Crear `ReservaHoteleraApp.Web\Views\Account\Login.cshtml`

```razor
@model LoginViewModel

@{
    ViewData["Title"] = "Iniciar sesión";
}

<h1>Iniciar sesión</h1>

<form asp-action="Login" method="post" class="col-md-6">
    <div asp-validation-summary="All" class="text-danger"></div>

    <div class="mb-3">
        <label asp-for="NombreUsuario" class="form-label">Usuario</label>
        <input asp-for="NombreUsuario" class="form-control" />
        <span asp-validation-for="NombreUsuario" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Contrasena" class="form-label">Contraseña</label>
        <input asp-for="Contrasena" class="form-control" />
        <span asp-validation-for="Contrasena" class="text-danger"></span>
    </div>

    <div class="form-check mb-3">
        <input asp-for="Recordarme" class="form-check-input" />
        <label asp-for="Recordarme" class="form-check-label">Recordarme</label>
    </div>

    <button type="submit" class="btn btn-primary">Entrar</button>
    <a asp-action="Register" class="btn btn-link">Crear cuenta</a>
</form>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 12.5 Crear `ReservaHoteleraApp.Web\Views\Account\Register.cshtml`

```razor
@model RegisterViewModel

@{
    ViewData["Title"] = "Crear cuenta";
}

<h1>Crear cuenta</h1>

<form asp-action="Register" method="post" class="col-md-6">
    <div asp-validation-summary="All" class="text-danger"></div>

    <div class="mb-3">
        <label asp-for="NombreUsuario" class="form-label">Usuario</label>
        <input asp-for="NombreUsuario" class="form-control" />
        <span asp-validation-for="NombreUsuario" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Contrasena" class="form-label">Contraseña</label>
        <input asp-for="Contrasena" class="form-control" />
        <span asp-validation-for="Contrasena" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="ConfirmarContrasena" class="form-label">Confirmar contraseña</label>
        <input asp-for="ConfirmarContrasena" class="form-control" />
        <span asp-validation-for="ConfirmarContrasena" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="NombreCliente" class="form-label">Nombre completo</label>
        <input asp-for="NombreCliente" class="form-control" />
        <span asp-validation-for="NombreCliente" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Documento" class="form-label">Documento</label>
        <input asp-for="Documento" class="form-control" />
        <span asp-validation-for="Documento" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Correo" class="form-label">Correo</label>
        <input asp-for="Correo" class="form-control" />
        <span asp-validation-for="Correo" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="Telefono" class="form-label">Teléfono</label>
        <input asp-for="Telefono" class="form-control" />
        <span asp-validation-for="Telefono" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Registrarme</button>
    <a asp-action="Login" class="btn btn-link">Ya tengo cuenta</a>
</form>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 12.6 Crear `ReservaHoteleraApp.Web\Views\Account\AccessDenied.cshtml`

```razor
@{
    ViewData["Title"] = "Acceso denegado";
}

<h1>Acceso denegado</h1>
<p>No tienes permisos para entrar a esta sección.</p>
<a asp-controller="Home" asp-action="Index" class="btn btn-primary">Volver al inicio</a>
```

### Verificación

Compila, entra a `/Account/Register`, crea un usuario y luego inicia sesión en `/Account/Login`.

---

## 13. Fase 4 — Sesión en _Layout.cshtml (PENDIENTE)

**Archivo:** `ReservaHoteleraApp.Web\Views\Shared\_Layout.cshtml`

### 13.1 Reemplazar el `<ul class="booking-nav-actions">...</ul>`

```razor
<ul class="booking-nav-actions">
    @if (User.Identity?.IsAuthenticated == true)
    {
        <li>
            <span class="booking-link-light">Hola, @User.Identity.Name</span>
        </li>

        @if (User.IsInRole("Administrador"))
        {
            <li>
                <a class="booking-link-light" asp-controller="Hotel" asp-action="Create">Registra tu alojamiento</a>
            </li>
        }

        <li>
            <form asp-controller="Account" asp-action="Logout" method="post" class="d-inline">
                <button type="submit" class="btn booking-btn-light">Cerrar sesión</button>
            </form>
        </li>
    }
    else
    {
        <li>
            <a class="booking-link-light" asp-controller="Account" asp-action="Register">Alta gratuita</a>
        </li>
        <li>
            <a class="btn booking-btn-light" asp-controller="Account" asp-action="Login">Iniciar sesión</a>
        </li>
    }

    <li>
        <a class="btn booking-btn-light" asp-controller="Hotel" asp-action="Index">Ver hoteles</a>
    </li>
</ul>
```

### 13.2 Dejar públicas solo las pestañas Inicio y Hoteles

Envolver las demás pestañas:

```razor
@if (User.IsInRole("Administrador"))
{
    <li class="booking-tab">
        <a asp-controller="Bloque" asp-action="Index" title="Nivel 2 · Bloque">Bloques</a>
    </li>
    <li class="booking-tab">
        <a asp-controller="Piso" asp-action="Index" title="Nivel 3 · Piso">Pisos</a>
    </li>
    <li class="booking-tab">
        <a asp-controller="TipoHabitacion" asp-action="Index" title="Catálogo · Tipo de habitación">Tipos de Habitación</a>
    </li>
    <li class="booking-tab">
        <a asp-controller="Habitacion" asp-action="Index" title="Nivel 4 · Habitación">Habitaciones</a>
    </li>
}
```

### Verificación

- Sin sesión: "Alta gratuita" e "Iniciar sesión".
- Con sesión: "Hola, usuario" y "Cerrar sesión".
- "Registra tu alojamiento" solo aparece al administrador.

---

## 14. Fase 5 — Autorización por roles (PENDIENTE)

Agregar en cada controlador:

```csharp
using Microsoft.AspNetCore.Authorization;
```

### 14.1 HotelController — proteger solo acciones de administración

Poner `[Authorize(Roles = "Administrador")]` encima de:

- `Create()` GET
- `Create(HotelAdd)` POST
- `Edit(int)` GET
- `Edit(HotelUpdate)` POST
- `Delete(int)` GET
- `DeleteConfirmed(int)` POST

`Index` y `Details` quedan públicos.

### 14.2 HabitacionController — proteger solo acciones de administración

Poner `[Authorize(Roles = "Administrador")]` encima de:

- `Create()` GET
- `Create(HabitacionAdd)` POST
- `Edit(int)` GET
- `Edit(HabitacionUpdate)` POST
- `Delete(int)` GET
- `DeleteConfirmed(int)` POST

`Index` y `Details` quedan públicos para que el cliente vea habitaciones.

### 14.3 Bloque, Piso y TipoHabitacion — clase completa

```csharp
[Authorize(Roles = "Administrador")]
public class BloqueController : Controller
```

Hacer lo mismo con `PisoController` y `TipoHabitacionController`.

### Verificación

- Sin sesión, `/Hotel/Create` redirige a `/Account/Login`.
- Con cliente, `/Hotel/Create` redirige a `/Account/AccessDenied`.
- Con administrador, entra normal.
- `/Hotel` y `/Hotel/Details/1` siguen abiertos.

---

## 15. Fase 6 — Crear el administrador (PENDIENTE)

1. Registrarse desde `/Account/Register`.
2. Ejecutar en SSMS o HeidiSQL:

```sql
UPDATE Usuarios
SET Rol = 'Administrador'
WHERE NombreUsuario = 'tu_usuario';
```

3. Verificar:

```sql
SELECT Id, NombreUsuario, Rol, Activo
FROM Usuarios
WHERE NombreUsuario = 'tu_usuario';
```

4. Cerrar sesión y volver a entrar para que la cookie tome el rol nuevo.

---

## 16. Fase 7 — Reservas del cliente (PENDIENTE)

### 16.1 DAL: buscar cliente por usuario

**`IClienteRepository.cs`**

```csharp
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Cliente;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IClienteRepository : IRepository<ClienteModel, ClienteAdd, ClienteUpdate, ClienteRemove>
    {
        Task<OperationResult<ClienteModel>> GetByUsuarioIdAsync(int idUsuario);
    }
}
```

**`ClienteRepository.cs`** — agregar después de `GetByIdAsync`:

```csharp
public async Task<OperationResult<ClienteModel>> GetByUsuarioIdAsync(int idUsuario)
{
    try
    {
        var cliente = await context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario);

        if (cliente is null)
            return Failure<ClienteModel>("El cliente no existe para este usuario.");

        return new OperationResult<ClienteModel>
        {
            Data = MapToModel(cliente),
            Message = "Cliente consultado correctamente."
        };
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al consultar el cliente del usuario {IdUsuario}.", idUsuario);
        return Failure<ClienteModel>("No se pudo consultar el cliente.");
    }
}
```

### 16.2 BL: exponer cliente por usuario

**`IClienteServices.cs`**

```csharp
Task<OperationResult<ClienteModel>> GetByUsuarioIdAsync(int idUsuario);
```

**`ClienteServices.cs`**

```csharp
public Task<OperationResult<ClienteModel>> GetByUsuarioIdAsync(int idUsuario)
{
    return ExecuteAsync<ClienteModel>("Consultar cliente por usuario", null, async () =>
    {
        if (idUsuario <= 0)
            return Failure<ClienteModel>("El identificador del usuario debe ser mayor que cero.");

        return await _repository.GetByUsuarioIdAsync(idUsuario);
    });
}
```

### 16.3 DAL: verificar disponibilidad

**`IReservaRepository.cs`**

```csharp
using ReservaHoteleraApp.DAL.Models;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.DAL.Repositories.BaseDAL;

namespace ReservaHoteleraApp.DAL.Repositories.InterfacesDAL
{
    public interface IReservaRepository : IRepository<ReservaModel, ReservaAdd, ReservaUpdate, ReservaRemove>
    {
        Task<OperationResult<bool>> EstaDisponibleAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida);
    }
}
```

**`ReservaRepository.cs`** — agregar después de `GetByIdAsync`:

```csharp
public async Task<OperationResult<bool>> EstaDisponibleAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida)
{
    try
    {
        var ocupada = await context.Reservas.AsNoTracking().AnyAsync(r =>
            r.IdHabitacion == idHabitacion &&
            (r.Estado == null || r.Estado != "Cancelada") &&
            r.FechaEntrada < fechaSalida &&
            (r.FechaSalida == null || r.FechaSalida > fechaEntrada));

        return new OperationResult<bool>
        {
            Data = !ocupada,
            Message = ocupada
                ? "La habitación no está disponible en esas fechas."
                : "La habitación está disponible."
        };
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al verificar disponibilidad de la habitación {IdHabitacion}.", idHabitacion);
        return Failure<bool>("No se pudo verificar la disponibilidad.");
    }
}
```

### 16.4 BL: reglas de disponibilidad

**`IReservaServices.cs`**

```csharp
Task<OperationResult<bool>> EstaDisponibleAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida);
```

**`ReservaServices.cs`**

```csharp
public Task<OperationResult<bool>> EstaDisponibleAsync(int idHabitacion, DateTime fechaEntrada, DateTime fechaSalida)
{
    return ExecuteAsync<bool>("Verificar disponibilidad", null, async () =>
    {
        if (idHabitacion <= 0)
            return Failure<bool>("El identificador de la habitación debe ser mayor que cero.");
        if (fechaEntrada < DateTime.Today)
            return Failure<bool>("La fecha de entrada no puede ser anterior a hoy.");
        if (fechaSalida <= fechaEntrada)
            return Failure<bool>("La fecha de salida debe ser posterior a la fecha de entrada.");

        return await _repository.EstaDisponibleAsync(idHabitacion, fechaEntrada, fechaSalida);
    });
}
```

### 16.5 Web: ViewModel de reserva

**`ReservaHoteleraApp.Web\Models\ReservaViewModel.cs`**

```csharp
using System.ComponentModel.DataAnnotations;

namespace ReservaHoteleraApp.Web.Models
{
    public class ReservaViewModel
    {
        public int IdHabitacion { get; set; }
        public string? NumeroHabitacion { get; set; }
        public decimal PrecioNoche { get; set; }

        [Required(ErrorMessage = "La fecha de entrada es requerida.")]
        [DataType(DataType.Date)]
        public DateTime FechaEntrada { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "La fecha de salida es requerida.")]
        [DataType(DataType.Date)]
        public DateTime FechaSalida { get; set; } = DateTime.Today.AddDays(1);

        [Range(1, 20, ErrorMessage = "Debe indicar entre 1 y 20 personas.")]
        public int CantidadPersonas { get; set; } = 1;
    }
}
```

### 16.6 Web: ReservaController

**`ReservaHoteleraApp.Web\Controllers\ReservaController.cs`**

```csharp
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Reserva;
using ReservaHoteleraApp.Web.Models;

namespace ReservaHoteleraApp.Web.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly IReservaServices _reservaServices;
        private readonly IHabitacionServices _habitacionServices;
        private readonly IClienteServices _clienteServices;

        public ReservaController(
            IReservaServices reservaServices,
            IHabitacionServices habitacionServices,
            IClienteServices clienteServices)
        {
            _reservaServices = reservaServices;
            _habitacionServices = habitacionServices;
            _clienteServices = clienteServices;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int habitacionId)
        {
            var habitacion = await _habitacionServices.GetByIdAsync(habitacionId);
            if (!habitacion.Success || habitacion.Data is null)
            {
                TempData["Error"] = habitacion.Message;
                return RedirectToAction("Index", "Habitacion");
            }

            var model = new ReservaViewModel
            {
                IdHabitacion = habitacion.Data.Id,
                NumeroHabitacion = habitacion.Data.Numero,
                PrecioNoche = habitacion.Data.PrecioNoche,
                FechaEntrada = DateTime.Today,
                FechaSalida = DateTime.Today.AddDays(1),
                CantidadPersonas = 1
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var habitacion = await _habitacionServices.GetByIdAsync(model.IdHabitacion);
            if (!habitacion.Success || habitacion.Data is null)
            {
                ModelState.AddModelError("", "La habitación no existe.");
                return View(model);
            }

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(idClaim, out var idUsuario))
            {
                ModelState.AddModelError("", "No se pudo identificar al usuario.");
                return View(model);
            }

            var cliente = await _clienteServices.GetByUsuarioIdAsync(idUsuario);
            if (!cliente.Success || cliente.Data is null)
            {
                ModelState.AddModelError("", "Tu usuario no tiene ficha de cliente.");
                return View(model);
            }

            var disponible = await _reservaServices.EstaDisponibleAsync(
                model.IdHabitacion, model.FechaEntrada, model.FechaSalida);

            if (!disponible.Success || disponible.Data == false)
            {
                ModelState.AddModelError("", disponible.Message ?? "La habitación no está disponible.");
                return View(model);
            }

            var noches = (model.FechaSalida - model.FechaEntrada).Days;

            var reserva = new ReservaAdd
            {
                IdCliente = cliente.Data.Id,
                IdHabitacion = model.IdHabitacion,
                CantidadPersonas = model.CantidadPersonas,
                Estado = "Pendiente",
                TotalReserva = noches * habitacion.Data.PrecioNoche,
                FechaEntrada = model.FechaEntrada,
                FechaSalida = model.FechaSalida
            };

            var resultado = await _reservaServices.AddAsync(reserva);
            if (!resultado.Success)
            {
                ModelState.AddModelError("", resultado.Message ?? "No se pudo registrar la reserva.");
                return View(model);
            }

            TempData["Success"] = "Reserva registrada correctamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}
```

### 16.7 Web: vista de reserva

**`ReservaHoteleraApp.Web\Views\Reserva\Create.cshtml`**

```razor
@model ReservaViewModel

@{
    ViewData["Title"] = "Reservar habitación";
    var hoy = DateTime.Today.ToString("yyyy-MM-dd");
}

<h1>Reservar habitación @Model.NumeroHabitacion</h1>
<p>Precio por noche: @Model.PrecioNoche.ToString("C")</p>

<form asp-action="Create" method="post" class="col-md-6">
    <input type="hidden" asp-for="IdHabitacion" />
    <input type="hidden" asp-for="NumeroHabitacion" />
    <input type="hidden" asp-for="PrecioNoche" />

    <div asp-validation-summary="All" class="text-danger"></div>

    <div class="mb-3">
        <label asp-for="FechaEntrada" class="form-label"></label>
        <input asp-for="FechaEntrada" asp-format="{0:yyyy-MM-dd}" class="form-control" type="date" min="@hoy" />
        <span asp-validation-for="FechaEntrada" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="FechaSalida" class="form-label"></label>
        <input asp-for="FechaSalida" asp-format="{0:yyyy-MM-dd}" class="form-control" type="date" min="@hoy" />
        <span asp-validation-for="FechaSalida" class="text-danger"></span>
    </div>

    <div class="mb-3">
        <label asp-for="CantidadPersonas" class="form-label"></label>
        <input asp-for="CantidadPersonas" class="form-control" type="number" min="1" max="20" />
        <span asp-validation-for="CantidadPersonas" class="text-danger"></span>
    </div>

    <button type="submit" class="btn btn-primary">Confirmar reserva</button>
    <a asp-controller="Habitacion" asp-action="Details" asp-route-id="@Model.IdHabitacion"
       class="btn btn-secondary">Volver</a>
</form>

@section Scripts {
    <partial name="_ValidationScriptsPartial" />
}
```

### 16.8 Botón "Reservar" en la habitación

**`Views\Habitacion\Details.cshtml`** — antes del enlace "Editar":

```razor
@if (User.Identity?.IsAuthenticated == true)
{
    <a class="btn btn-primary"
       asp-controller="Reserva" asp-action="Create"
       asp-route-habitacionId="@Model.Id">Reservar</a>
}

@if (User.IsInRole("Administrador"))
{
    <a asp-action="Edit" asp-route-id="@Model.Id">Editar</a> <text>|</text>
}
```

Si la vista `Habitacion\Index` muestra enlaces de Editar/Eliminar, envolverlos también en `@if (User.IsInRole("Administrador"))`.

---

## 17. Fase 8 — Checklist de pruebas

- [ ] Registro crea usuario con rol `Cliente` y su ficha en `Clientes`.
- [ ] Login correcto muestra "Hola, usuario".
- [ ] Login incorrecto muestra error.
- [ ] Logout borra la sesión.
- [ ] Sin sesión, `/Hotel` y `/Hotel/Details/1` son públicos.
- [ ] Cliente no ve pestañas de Bloques, Pisos, Tipos ni Habitaciones.
- [ ] Cliente no puede abrir `/Hotel/Create` ni `/Habitacion/Create`.
- [ ] Administrador ve todo y puede crear/editar/eliminar.
- [ ] Cliente autenticado puede reservar.
- [ ] Sin sesión, `/Reserva/Create?habitacionId=1` redirige al login.
- [ ] Reserva con fecha pasada o salida menor/igual a entrada se rechaza.
- [ ] Reserva en fechas ocupadas se rechaza.
- [ ] Total de la reserva = noches × precio por noche.

---

## 18. Notas importantes para la próxima sesión

1. No hay cambios de esquema, así que no se necesitan migraciones.
2. `UpdateAsync` de usuarios no hashea. No usar el formulario de edición para cambiar contraseñas.
3. El rol nunca viene del formulario; en el registro siempre se fuerza `"Cliente"`.
4. El total y la disponibilidad se validan en el servidor.
5. Detener la depuración en Visual Studio antes de compilar para evitar el bloqueo de `bin`.
6. Si un usuario viejo no puede entrar, es porque su contraseña está en texto plano; registrarlo de nuevo.

---

## 19. Historial de avance (autenticación)

- **15/11/2026:** Se instaló `Microsoft.Extensions.Identity.Core 10.0.13` en BL.
- **15/11/2026:** Fase 1 completada: hash de contraseñas, búsqueda por nombre de usuario y validación de credenciales.
- **15/11/2026:** Build de la solución: 0 errores, 0 advertencias.
- **16/11/2026:** Fase 2 completada: cookies configuradas en `Program.cs`. Build 0 errores / 0 advertencias.
- **16/11/2026:** Fase 3 en curso: `LoginViewModel`, `RegisterViewModel` y `AccountController` completos y compilando.
- **16/11/2026:** Siguiente paso: crear `Views/Account/{Login,Register,AccessDenied}.cshtml`.
- **Pendiente:** Fases 4 a 8 (sesión en layout, roles, administrador, reservas y pruebas).

---

# Parte 3 — Sesión del 16/11/2026 (Fase 2 completada, Fase 3 en curso)

## 20. Qué se hizo en esta sesión

### 20.1 Fase 2 — `Program.cs` (COMPLETADA)

Archivo: `ReservaHoteleraApp.Web\Program.cs`

- `using Microsoft.AspNetCore.Authentication.Cookies;`
- Después de `builder.Services.AddControllersWithViews();`:

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
```

- En el pipeline, entre `UseRouting()` y `UseAuthorization()`:

```csharp
app.UseAuthentication();
```

- Verificado con `dotnet build`: 0 errores, 0 advertencias.
- Orden correcto: `UseRouting` → `UseAuthentication` → `UseAuthorization`.

### 20.2 Fase 3 — ViewModels (COMPLETADA)

- `ReservaHoteleraApp.Web\Models\LoginViewModel.cs`
  - `NombreUsuario`, `Contrasena` (`[DataType(DataType.Password)]`), `Recordarme`.
- `ReservaHoteleraApp.Web\Models\RegisterViewModel.cs`
  - `NombreUsuario`, `Contrasena` (mínimo 6), `ConfirmarContrasena` (`[Compare]`),
    `NombreCliente`, `Documento`, `Correo` (`[EmailAddress]`), `Telefono` (opcional).

### 20.3 Fase 3 — `AccountController` (COMPLETADA)

Archivo: `ReservaHoteleraApp.Web\Controllers\AccountController.cs`

- Inyección de `IUsuarioServices` e `IClienteServices`.
- `Login()` GET y `Login(LoginViewModel)` POST:
  - Valida con `ValidarCredencialesAsync`.
  - Crea claims: `NameIdentifier` (Id), `Name` (usuario), `Role`.
  - Crea la cookie con `SignInAsync`; `IsPersistent = model.Recordarme`.
  - Redirige a `Home/Index`.
- `Register()` GET y `Register(RegisterViewModel)` POST:
  - Crea el usuario con `Rol = "Cliente"` fijo.
  - Recupera el usuario con `GetByNombreUsuarioAsync`.
  - Crea la ficha de cliente; si falla, elimina el usuario (compensación).
  - `TempData["Success"]` y redirige a `Login`.
- `Logout()` POST con `SignOutAsync` y `[ValidateAntiForgeryToken]`.
- `AccessDenied()` GET.
- `[HttpPost]` y `[ValidateAntiForgeryToken]` presentes en Login, Register y Logout.
- Compilación verificada: 0 errores, 0 advertencias.

## 21. Qué falta para terminar la Fase 3 (Bloque C — vistas)

Carpeta a crear: `ReservaHoteleraApp.Web\Views\Account`

1. `Login.cshtml`
   - `@model LoginViewModel`
   - Formulario `asp-action="Login"` con `asp-validation-summary="All"`.
   - Campos `NombreUsuario`, `Contrasena`, `Recordarme`.
   - `@section Scripts { <partial name="_ValidationScriptsPartial" /> }`.
2. `Register.cshtml`
   - `@model RegisterViewModel`
   - Todos los campos del ViewModel.
   - Validación con tag helpers y partial de scripts.
3. `AccessDenied.cshtml`
   - Título, mensaje y enlace de vuelta al inicio.

El código completo de las tres vistas está en la sección 12.4, 12.5 y 12.6 de este documento.

## 22. Cómo retomar

1. Abrir `PROGRESO.md` y leer esta Parte 3.
2. Crear la carpeta `Views\Account` y la vista `Login.cshtml`.
3. Luego `Register.cshtml` y `AccessDenied.cshtml`.
4. Compilar y probar:
   - `/Account/Register` → crear una cuenta.
   - `/Account/Login` → iniciar sesión.
   - Revisar que el registro cree usuario + cliente.

## 23. Recordatorios

- No hay cambios de esquema: no se necesitan migraciones.
- Detener la depuración en Visual Studio (`Shift + F5`) antes de compilar.
- El rol nunca viene del formulario: el registro fuerza `"Cliente"`.
- Las contraseñas se hashean en BL (`UsuarioServices`); el controlador no las guarda en claro.
- `TempData["Success"]` debe escribirse exactamente así (con dos `s`).
