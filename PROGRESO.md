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
- [x] Controlador y vista de **Reserva** (acción `Create`; ver Fase 7 y Parte 5).
- [x] `Reserva/Index` («Mis reservas») con cancelación — ver §32.
- [ ] Crear controladores y vistas para Estadía, Pago y Cliente
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
**Estado actual:** Fase 7 completada y verificada (reservas del cliente y «Mis reservas», §32), más el registro del usuario `Cliente` `Rafael1313` (§33). Siguiente: Fase 8 (checklist) — falta el CRUD como Administrador. Ver Parte 5.
**Cómo retomar:** leer la Parte 5 (secciones 29 a 31), la Parte 4 (24 a 28) y la sección 16.

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

## 12. Fase 3 — Registro y login (COMPLETADA — 17/11/2026)

**Estado:** ViewModels, `AccountController` y las tres vistas de `Views/Account`
(`Login`, `Register`, `AccessDenied`) creados. Build 0 errores / 0 advertencias.
Detalle de cierre en la Parte 4 (sección 24).

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

## 13. Fase 4 — Sesión en _Layout.cshtml (COMPLETADA — 17/11/2026)

**Estado:** bloque de sesión agregado en `_Layout.cshtml`. Ver cierre en la Parte 4 (sección 26).

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

## 14. Fase 5 — Autorización por roles (COMPLETADA — 17/11/2026)

**Estado:** atributos `[Authorize(Roles = "Administrador")]` aplicados en los 5
controladores. Ver cierre en la Parte 4 (sección 27).

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

## 15. Fase 6 — Crear el administrador (COMPLETADA — 17/11/2026)

**Estado:** el usuario `Aleguzz21` fue promovido a `Administrador` en `HotelDB`.
Ver cierre en la Parte 4 (sección 28).

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

## 16. Fase 7 — Reservas del cliente (COMPLETADA — 17/11/2026)

> Cerrada y verificada en ejecución. Detalle final en la Parte 5 (secciones 29 a 31).
> El código de abajo es el plan original; se aplicó con dos ajustes: el redirect del
> POST va a `Habitacion/Index` (la home no muestra `TempData`) y se quitó
> `r.Estado == null` de la consulta de disponibilidad (`Reserva.Estado` no es anulable).

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

> Actualizado el 17/11/2026 tras verificar la Fase 7. `[x]` = probado en ejecución.

- [x] Registro crea usuario con rol `Cliente` y su ficha en `Clientes` (`Rafael1313`, 17/11/2026).
- [x] Login correcto muestra "Hola, usuario".
- [x] Login incorrecto muestra error.
- [x] Logout borra la sesión.
- [x] Sin sesión, `/Hotel` y `/Hotel/Details/1` son públicos.
- [x] Cliente no ve pestañas de Bloques, Pisos, Tipos ni Habitaciones.
- [x] Cliente no puede abrir `/Hotel/Create` ni `/Habitacion/Create`.
- [ ] Administrador ve todo y puede crear/editar/eliminar.
- [x] Cliente autenticado puede reservar (`Rafael1313`, total 800.00, 05→07/07/2027).
- [x] Sin sesión, `/Reserva/Create?habitacionId=1` redirige al login.
- [x] Reserva con fecha pasada o salida menor/igual a entrada se rechaza.
- [x] Reserva en fechas ocupadas se rechaza.
- [x] Total de la reserva = noches × precio por noche.
- [x] Cantidad de personas fuera de 1–20 se rechaza.
- [x] El botón "Reservar" no aparece sin sesión.
- [x] «Mis reservas» muestra hotel, habitación, fechas, noches, personas, total y estado.
- [x] Cancelar una reserva libera sus fechas para volver a reservar.

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
- **17/11/2026:** Fase 3 completada: creadas las vistas `Login`, `Register` y `AccessDenied`. Build 0 errores / 0 advertencias.
- **17/11/2026:** Fase 4 completada: menú con sesión en `_Layout.cshtml`. Home y Login verificados en ejecución (200).
- **17/11/2026:** Fase 5 completada: 15 atributos `[Authorize(Roles = "Administrador")]`. Redirecciones verificadas (302 a Login).
- **17/11/2026:** Fase 6 completada: `Aleguzz21` promovido a `Administrador` en `HotelDB` (verificado con SELECT).
- **Pendiente:** Fase 8 (checklist, sobre todo con un usuario `Cliente`) y `Reserva/Index`.

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

## 21. Qué faltaba para terminar la Fase 3 (Bloque C — vistas) — COMPLETADO 17/11/2026

> Cerrado: las tres vistas existen. Ver Parte 4 (sección 24).

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

---

# Parte 4 — Sesión del 17/11/2026 (Fase 3 completada)

## 24. Cierre de la Fase 3 — vistas de cuenta

Carpeta creada: `ReservaHoteleraApp.Web\Views\Account`

| Archivo | Estado | Notas |
|---|---|---|
| `Login.cshtml` | Creada | `@model LoginViewModel`, `asp-validation-summary="All"`, campos Usuario/Contraseña/Recordarme |
| `Register.cshtml` | Creada | `@model RegisterViewModel`, los 7 campos del ViewModel |
| `AccessDenied.cshtml` | Creada | Mensaje y enlace de vuelta al inicio |

Decisiones:

- Se usó `asp-validation-summary="All"` para que también se vea el error que el
  controlador agrega con `ModelState.AddModelError` (credenciales incorrectas).
- El token antiforgery lo genera el tag helper `<form>`, porque las acciones POST
  tienen `[ValidateAntiForgeryToken]`.
- Se mantuvo el estilo de `Hotel/Create.cshtml` (`row`, `col-md-5`, `form-group mb-3`).
- Se usó `Html.RenderPartialAsync("_ValidationScriptsPartial")` dentro de
  `@section Scripts`, igual que en las vistas existentes.

Verificación:

- `dotnet build` de la solución: 0 errores, 0 advertencias.
- Al cerrar la Fase 4 se ejecutó la app: Home y `/Account/Login` respondieron 200.
  La prueba completa de registro + login sigue pendiente de hacerse a mano.

Prueba manual pendiente:

1. Ejecutar con F5 y abrir `http://localhost:5030/Account/Register`.
2. Crear una cuenta; debe redirigir a `/Account/Login`.
3. Iniciar sesión; debe redirigir a `Home/Index`.
4. Enviar el login vacío: deben aparecer los mensajes de `[Required]`.
5. Enviar credenciales incorrectas: debe aparecer el mensaje del controlador.

Nota: el enlace al login ya se ve en el menú desde la Fase 4 (sección 25).
Antes de eso se entraba por URL directa.

## 25. Fase 4 completada — sesión en el menú

Archivo: `ReservaHoteleraApp.Web\Views\Shared\_Layout.cshtml`

Cambios:

- `<ul class="booking-nav-actions">` ahora es condicional:
  - Sin sesión: "Alta gratuita" (Register) e "Iniciar sesión" (Login).
  - Con sesión: "Hola, {usuario}", "Registra tu alojamiento" solo si es
    `Administrador`, y botón "Cerrar sesión" (form POST a `Account/Logout`).
  - "Ver hoteles" queda siempre visible.
- Las pestañas Bloques, Pisos, Tipos de Habitación y Habitaciones se muestran solo
  si `User.IsInRole("Administrador")`. Inicio y Hoteles quedan públicos.
- El formulario de logout lleva `method="post"`; el tag helper genera el token
  antiforgery que exige `[ValidateAntiForgeryToken]` en `Logout`.
- Se conservaron `asp-area=""` y las clases `booking-*` del diseño actual.

Verificación en ejecución (17/11/2026):

- `dotnet build`: 0 errores, 0 advertencias.
- `GET /`: 200 y contiene "Iniciar sesión" y "Alta gratuita" sin sesión.
- `GET /Account/Login`: 200, contiene el input `NombreUsuario` y el token
  `__RequestVerificationToken`.

Importante: ocultar pestañas **no es seguridad**. Cualquiera puede escribir la URL
directa. La protección real es la Fase 5 con `[Authorize(Roles = "Administrador")]`.

Prueba manual pendiente:

1. Sin sesión, la home debe mostrar "Alta gratuita" e "Iniciar sesión".
2. Registrarse e iniciar sesión: el menú debe mostrar "Hola, usuario" y "Cerrar sesión".
3. Pulsar "Cerrar sesión": debe volver a mostrar los enlaces de invitado.
4. Las pestañas de administración se activan con el usuario administrador (Fase 6, ya hecho).

## 26. Fase 5 completada — autorización por roles

Archivos modificados (todos en `ReservaHoteleraApp.Web\Controllers`):

| Archivo | Atributos | Alcance |
|---|---|---|
| `HotelController.cs` | 6 | Create GET/POST, Edit GET/POST, Delete GET, DeleteConfirmed |
| `HabitacionController.cs` | 6 | Create GET/POST, Edit GET/POST, Delete GET, DeleteConfirmed |
| `BloqueController.cs` | 1 | Clase completa |
| `PisoController.cs` | 1 | Clase completa |
| `TipoHabitacionController.cs` | 1 | Clase completa |

Se agregó `using Microsoft.AspNetCore.Authorization;` en los cinco archivos.

Quedan públicos:

- `HotelController.Index` y `HotelController.Details`.
- `HabitacionController.Index` y `HabitacionController.Details`.
- `HomeController`, `AccountController`.

Verificación en ejecución (17/11/2026, sin sesión):

- `GET /Hotel/Create` → **302** a `/Account/Login?ReturnUrl=%2FHotel%2FCreate`.
- `GET /Bloque` → **302** a `/Account/Login?ReturnUrl=%2FBloque`.
- `GET /Hotel` → **200**.
- `GET /Account/Login` → **200**.
- `dotnet build`: 0 errores, 0 advertencias.

Prueba manual pendiente:

1. Sin sesión, `/Hotel/Create` debe llevar al login.
2. Con un cliente registrado, `/Hotel/Create` debe llevar a `/Account/AccessDenied`.
3. El caso "administrador entra normal" ya se puede probar con `Aleguzz21` (Fase 6).

Nota: el rol se lee del *claim* guardado en la cookie al iniciar sesión. Si cambias el
rol de un usuario en la base de datos, debe cerrar sesión y volver a entrar.

## 27. Fase 6 completada — usuario administrador

Verificado en la base `HotelDB` (SQL Server `localhost`, autenticación Windows):

| Id | NombreUsuario | Rol | Activo |
|---|---|---|---|
| 1 | `Aleguzz21` | `Administrador` | 1 |

Procedimiento usado:

```sql
USE HotelDB;
GO
UPDATE Usuarios
SET Rol = 'Administrador'
WHERE NombreUsuario = 'Aleguzz21';
GO
```

Notas:

- El rol **no** se cambia desde la web: el registro siempre crea `Cliente`.
- Después de cambiar el rol en la BD hay que **cerrar sesión y volver a entrar**,
  porque el rol vive en un *claim* de la cookie.
- No se modificó la contraseña; el hash sigue igual.
- Sin cambios de esquema ni migraciones.

## 28. Próximo paso (cumplido el 17/11/2026)

La Fase 7 se completó. Detalle en la Parte 5 (secciones 29 a 31).

---

# Parte 5 — Sesión del 17/11/2026 (Fase 7 completada)

## 29. Cierre de la Fase 7 — reservas del cliente

**Estado:** COMPLETADA y verificada en ejecución.
**Build:** 0 errores, 0 advertencias.
**Base de datos:** sin cambios de esquema ni migraciones.

### Archivos tocados

| Capa | Archivo | Cambio |
|---|---|---|
| DAL | `Repositories\InterfacesDAL\IClienteRepository.cs` | `GetByUsuarioIdAsync` |
| DAL | `Repositories\ImplementationsDAL\ClienteRepository.cs` | idem |
| BL | `Services\InterfacesBL\IClienteServices.cs` | idem |
| BL | `Services\ImplementationsBL\ClienteServices.cs` | idem |
| DAL | `Repositories\InterfacesDAL\IReservaRepository.cs` | `EstaDisponibleAsync` |
| DAL | `Repositories\ImplementationsDAL\ReservaRepository.cs` | idem |
| BL | `Services\InterfacesBL\IReservaServices.cs` | idem |
| BL | `Services\ImplementationsBL\ReservaServices.cs` | idem |
| Web | `Models\ReservaViewModel.cs` | nuevo |
| Web | `Controllers\ReservaController.cs` | nuevo (GET + POST) |
| Web | `Views\Reserva\Create.cshtml` | nueva |
| Web | `Views\Habitacion\Details.cshtml` | botón Reservar; Editar solo Administrador |
| Web | `Views\Habitacion\Index.cshtml` | Editar/Eliminar solo Administrador |

### Decisiones y diferencias con el plan original

1. **`r.Estado != "Cancelada"`** — `Reserva.Estado` es `string` no anulable, así que el
   `r.Estado == null` del borrador sobraba.
2. **Redirect del POST a `Habitacion/Index`** en vez de `Home/Index`: la home no muestra
   `TempData`, así que el mensaje de éxito no se vería.
3. **Precio y total se recalculan en el servidor** con `habitacion.Data.PrecioNoche`.
   Los campos ocultos `PrecioNoche` y `NumeroHabitacion` solo sirven para redibujar el
   formulario cuando hay un error.
4. **`[Authorize]` a nivel de clase** en `ReservaController` (sin roles): cliente y
   administrador pueden reservar; sin sesión redirige a `/Account/Login`.
5. No se registró nada nuevo en `Program.cs`: los servicios ya estaban en
   `BL\DependencyInjection.cs`.

### Verificación en ejecución (17/11/2026)

| Prueba | Resultado |
|---|---|
| `GET /Reserva/Create?habitacionId=1` sin sesión | 302 a `/Account/Login?ReturnUrl=...` |
| `GET /Habitacion/Details/1` sin sesión | 200 y **sin** botón Reservar |
| Login `Aleguzz21` / contraseña registrada | sesión activa |
| `GET /Reserva/Create?habitacionId=1` con sesión | 200, habitación 109, precio 400 |
| Reserva 10→13/03/2027, 2 personas | éxito, «Reserva registrada correctamente» |
| Repetir las mismas fechas | «no está disponible en esas fechas» |
| Entrada 01/01/2020 | «no puede ser anterior a hoy» |
| Salida igual a entrada | «debe ser posterior a la fecha de entrada» |
| 99 personas | `[Range]`: «entre 1 y 20 personas» |

Fila creada en `HotelDB.Reservas`:

| Id | IdCliente | IdHabitacion | Personas | Estado | Total | Entrada | Salida |
|---|---|---|---|---|---|---|---|
| 1 | 1 | 1 | 2 | Pendiente | 1200.00 | 2027-03-10 | 2027-03-13 |

Total = 3 noches × 400. Los 4 intentos inválidos no crearon filas.

> La reserva de prueba (Id 1) quedó en la BD como evidencia. Para borrarla:
> `DELETE FROM Reservas WHERE Id = 1;`

### Errores de aprendizaje de la sesión

- `CS0111` → método pegado dos veces en `ClienteRepository`. Solución: buscar el nombre
  con `Ctrl + F` antes de pegar.
- `CS0308` → Visual Studio agregó `using System.ClientModel.Primitives;`, que tiene un
  `OperationResult` no genérico. Solución: usar siempre
  `using ReservaHoteleraApp.DAL.Models;` y no aceptar el `using` equivocado.

## 30. Pendientes al cierre de la Fase 7

- [x] `Reserva/Index`: listar reservas del usuario autenticado (y todas para el admin) — ver §32.
- [x] Cancelar reserva (`Estado = "Cancelada"`), que ya libera la habitación — ver §32.
- [ ] `Estadia`, `Pago` y `Cliente`: controladores y vistas (existen en BL/DAL).
- [ ] Validación de fechas en servidor al **editar** una reserva.
- [ ] Condición de carrera: verificar disponibilidad + insertar no es atómico. A futuro,
      transacción o restricción en la base.
- [ ] Unificar estilos de botones, tablas, formularios y alertas.
- [ ] Prueba manual con un usuario **Cliente** (no solo administrador): registro, login,
      pestañas ocultas y `/Hotel/Create` → `AccessDenied`.
- [ ] Etiquetas `[Display(Name = "...")]` en `ReservaViewModel` para que los labels no
      salgan como `FechaEntrada`.

## 31. Historial de avance (reservas)

- **17/11/2026:** DAL y BL de cliente por usuario (`GetByUsuarioIdAsync`).
- **17/11/2026:** DAL y BL de disponibilidad por fechas (`EstaDisponibleAsync`).
- **17/11/2026:** `ReservaViewModel`, `ReservaController` (GET/POST) y `Views\Reserva\Create.cshtml`.
- **17/11/2026:** botón Reservar en `Habitacion/Details`; Editar/Eliminar solo administrador.
- **17/11/2026:** build 0/0, 9 pruebas de ejecución y verificación en base de datos.
- **17/11/2026:** `Reserva/Index` («Mis reservas») y cancelación que libera fechas (ver §32).
- **17/11/2026:** verificación completa con usuario `Cliente` (`Rafael1313`): permisos, reserva y cancelación ajena bloqueada.
- **17/11/2026:** incidente de usuario duplicado resuelto (se borró el huérfano Id 2); registro nuevo `Rafael1313` (Usuario 8 / Cliente 3).
- **Siguiente:** Fase 8 (checklist) — queda comprobar CRUD completo como administrador.

## 32. Ampliación posterior — «Mis reservas» (`Reserva/Index`)

**Estado:** COMPLETADA y verificada en ejecución (17/11/2026). Build 0/0.

### Qué se agregó

| Capa | Archivo | Cambio |
|---|---|---|
| DAL | `Models\Objects\Reserva\ReservaDetalleModel.cs` | nuevo: reserva + hotel/bloque/piso/habitación/tipo y `Noches` |
| DAL | `Repositories\InterfacesDAL\IReservaRepository.cs` | `GetByClienteIdAsync`, `GetAllConDetalleAsync` |
| DAL | `Repositories\ImplementationsDAL\ReservaRepository.cs` | `ConsultaDetalle()` con `Include` de Cliente y Habitacion→Piso→Bloque→Hotel→TipoHabitacion; `MapToDetalle` |
| BL | `Services\InterfacesBL\IReservaServices.cs` | 3 métodos nuevos; se quitó un `using` sin uso |
| BL | `Services\ImplementationsBL\ReservaServices.cs` | `GetByClienteIdAsync`, `GetAllConDetalleAsync`, `CancelarAsync` |
| Web | `Controllers\ReservaController.cs` | `Index`, `Cancelar` (POST) y `ObtenerIdClienteAsync` |
| Web | `Views\Reserva\Index.cshtml` | tabla con hotel, habitación, piso/tipo, fechas, noches, personas, total, estado y botón Cancelar |
| Web | `Views\Shared\_Layout.cshtml` | enlace «Mis reservas» para usuarios con sesión |

### Reglas

- El **administrador ve todas** las reservas; el cliente solo las suyas (filtra por `IdCliente`).
- El hotel **no se guarda** en `Reservas`: se obtiene por
  `Habitación → Piso → Bloque → Hotel`.
- Cancelar **no borra**: pone `Estado = "Cancelada"`. La consulta de disponibilidad ya
  ignora las canceladas, por eso las fechas quedan libres.
- `CancelarAsync(idReserva, idCliente, esAdministrador)` valida en BL que la reserva sea
  del cliente (salvo admin) y que no esté ya cancelada.

### Verificación en ejecución

| Prueba | Resultado |
|---|---|
| `GET /Reserva` sin sesión | 302 al login |
| `GET /Reserva` como administrador | 200; muestra «The Marriot», piso 2, «Suite Presidencial», Pendiente y botón Cancelar |
| Enlace «Mis reservas» en el menú | presente con sesión |
| `POST /Reserva/Cancelar` (Id 1) | «Reserva cancelada correctamente» y badge Cancelada |
| Cancelar dos veces la misma | «La reserva ya está cancelada» |
| Reservar otra vez 10→13/03/2027 | éxito: la cancelación liberó las fechas |

Estado final en `HotelDB.Reservas`:

| Id | Estado | Total | Entrada | Salida |
|---|---|---|---|---|
| 1 | Cancelada | 1200.00 | 2027-03-10 | 2027-03-13 |
| 2 | Pendiente | 1200.00 | 2027-03-10 | 2027-03-13 |
| 3 | Pendiente | 800.00 | 2027-07-05 | 2027-07-07 |

### Verificación con un usuario `Cliente` (`Rafael1313`)

| Prueba | Resultado |
|---|---|
| Login `Rafael1313` (contraseña definida en el registro) | sesión activa |
| Menú | «Mis reservas» y «Cerrar sesión»; **sin** pestañas de admin ni «Registra tu alojamiento» |
| `GET /Reserva` sin reservas | estado vacío |
| `GET /Hotel/Create`, `/Habitacion/Create` y `/Bloque` | «Acceso denegado» |
| Reservar habitación 109, 05→07/07/2027, 2 personas | éxito; total 800.00 |
| `GET /Reserva` | The Marriot, 2 noches, Pendiente; columna Cliente oculta |
| Intentar cancelar la reserva Id 2 (de otro cliente) | «No puedes cancelar una reserva que no es tuya» |
| Logout + `GET /Reserva` | 302 al login |

> Las 3 filas son datos de prueba. Para limpiarlas:
> `DELETE FROM Reservas WHERE Id IN (1, 2, 3);`

## 33. Registro del usuario Cliente y estado final (17/11/2026)

### Incidente resuelto: nombre de usuario duplicado

- Al registrarse con `Rafael1313` la app respondió: «No se pudo registrar el usuario.
  Verifique datos duplicados o referencias que no existen.»
- Causa: `IX_Usuarios_NombreUsuario` es un índice **único** y ya existía un usuario
  `Rafael1313` (Id 2) de un intento anterior.
- Ese usuario estaba **huérfano**: sin ficha en `Clientes`, sin reservas y sin registros
  de actividad. Se eliminó con `DELETE FROM Usuarios WHERE Id = 2;`.
- El registro nuevo creó:
  - `Usuarios` Id 8 → `Rafael1313`, rol `Cliente`, activo.
  - `Clientes` Id 3 → `Rafael Elpidio Guzman`, ligado a `IdUsuario = 8`.

### Comportamiento esperado en el formulario

- Cuando el POST falla y se vuelve a mostrar la vista, los campos de contraseña salen
  **vacíos**: ASP.NET nunca rellena `input type="password"`. No es un error; hay que
  volver a escribirlas.
- El mensaje de duplicado viene del `DbUpdateException` del repositorio, no de una
  validación previa. Mejora futura: consultar `GetByNombreUsuarioAsync` antes de insertar
  y devolver un mensaje más claro.

### Credenciales de prueba

- `Aleguzz21` → rol `Administrador`, con ficha de cliente (Id 1).
- `Rafael1313` → rol `Cliente`, con ficha de cliente (Id 3).
- Las contraseñas **no** se guardan en este documento ni en el repositorio.

### Estado actual de la base de datos

- `Usuarios`: Id 1 `Aleguzz21` (Administrador) e Id 8 `Rafael1313` (Cliente).
- `Clientes`: Id 1 (usuario 1) e Id 3 (usuario 8).
- `Reservas`: 3 filas de prueba (ver tabla de §32).

Para limpiar solo las reservas de prueba:

```sql
USE HotelDB;
DELETE FROM Reservas WHERE Id IN (1, 2, 3);
```

### Cómo retomar la próxima sesión

1. Detener la app en Visual Studio y compilar:
   `dotnet build ReservaHoteleraApp.slnx`.
2. Rutas para probar:
   - Públicas: `/`, `/Hotel`, `/Habitacion`, `/Habitacion/Details/1`.
   - Con sesión: `/Reserva/Create?habitacionId=1` y `/Reserva` («Mis reservas»).
   - Cuenta: `/Account/Login`, `/Account/Register`.
3. Pendiente del checklist: CRUD completo como **Administrador** (crear/editar/eliminar
   Hotel, Habitación, Bloque, Piso y Tipo de Habitación) y validación de fechas al
   editar una reserva.



## 34. Selección de habitaciones disponibles y reserva desde la web (17/11/2026)

### Lo que pidió el usuario

> «Hacer las reservas en la página web, seleccionar las habitaciones disponibles para
> reservarlas y ver las reservas.»

Se cerró el flujo completo: el cliente elige fechas → la web muestra **solo las
habitaciones libres** en ese rango → pulsa «Reservar» con las fechas precargadas → confirma
→ sus reservas aparecen en «Mis reservas».

### Cambios por capa

| Capa | Archivo | Cambio |
|---|---|---|
| DAL modelo | `Models\Objects\Habitacion\HabitacionModel.cs` | Nuevas propiedades `BloqueNombre`, `HotelNombre`, `Ciudad`. |
| DAL | `Repositories\ImplementationsDAL\HabitacionRepository.cs` | `GetAll`/`GetById` ahora incluyen `Piso.Bloque.Hotel`; nuevo `GetDisponiblesAsync(entrada, salida)`; `MapToModel` rellena hotel/bloque/ciudad. |
| DAL ifaz | `Repositories\InterfacesDAL\IHabitacionRepository.cs` | Declara `GetDisponiblesAsync`. |
| BL ifaz | `Services\InterfacesBL\IHabitacionServices.cs` | Declara `GetDisponiblesAsync`. |
| BL | `Services\ImplementationsBL\HabitacionServices.cs` | Valida fechas (entrada ≥ hoy, salida > entrada) y delega en el repositorio. |
| Web | `Controllers\HabitacionController.cs` | `Index(string? fechaEntrada, string? fechaSalida, bool verTodas)` filtra por fechas; `verTodas=true` (admin) muestra inventario completo. |
| Web | `Views\Habitacion\Index.cshtml` | Formulario de fechas, columnas Hotel/Ciudad, badge de estado solo en «Ver todas», botón «Reservar» por fila con `habitacionId` + fechas, «Mis reservas». |
| Web | `Models\ReservaViewModel.cs` | Nuevos campos `HotelNombre`, `PisoNumero`, `TipoHabitacionNombre`. |
| Web | `Controllers\ReservaController.cs` | `Create` GET acepta `fechaEntrada`/`fechaSalida` y precarga datos del hotel; bloquea habitaciones en «Mantenimiento» en GET y POST. |
| Web | `Views\Reserva\Create.cshtml` | Cabecera con hotel, piso y tipo; `hidden` para los nuevos campos. |
| Web | `Views\Habitacion\Details.cshtml` | Filas «Hotel» (con ciudad) y «Bloque». |

### Regla de disponibilidad usada

Una habitación está libre para `[entrada, salida)` cuando:

1. `Estado != "Mantenimiento"`, y
2. no existe una reserva con `Estado != "Cancelada"` que se cruce:
   `reserva.FechaEntrada < salida AND (reserva.FechaSalida IS NULL OR reserva.FechaSalida > entrada)`.

La comparación es de rango semiabierto: quien sale el día 13 libera la noche del 13.

### Verificación automatizada (PowerShell + `Invoke-WebRequest`)

| Prueba | Resultado |
|---|---|
| Anónimo en `/Habitacion` | sin botón «Reservar» ni «Mis reservas» |
| Cliente `Rafael1313`, 10→13/03/2027 | 003 visible; 109 excluida (reserva pendiente); 401 excluida (mantenimiento) |
| Sin fechas (hoy→mañana) | 401 excluida |
| `GET /Reserva/Create?habitacionId=3` | redirige con «La habitación está en mantenimiento» |
| `GET /Reserva/Create?habitacionId=2&fechaEntrada=2027-07-05&fechaSalida=2027-07-07` | hotel «Embassy Suites», fechas precargadas |
| `POST /Reserva/Create` (habitación 2, 05→07/07/2027, 2 personas) | «Reserva registrada correctamente», total 400.00 |
| `GET /Habitacion` mismas fechas tras reservar | 003 ya no aparece |
| `GET /Reserva` | muestra 05/07/2027 y «Embassy Suites by Hilton Santo Domingo» |
| Admin `Aleguzz21`, `?verTodas=true` | incluye 401 + botones Editar/Eliminar; enlace «Volver a disponibles» |
| Admin filtrado 10→13/03/2027 | 109 excluida |

> La reserva creada en la prueba se eliminó con
> `DELETE FROM Reservas WHERE IdCliente = 3 AND IdHabitacion = 2 AND FechaEntrada = '2027-07-05';`
> La BD quedó otra vez con 3 filas.

### Nota sobre el estado de la BD

La reserva Id 3 (cliente 3, 109, 05→07/07/2027) quedó en **Cancelada** tras la prueba de
cancelación de §32. Por eso el 109 sí aparece como disponible en esas fechas; no es un
fallo del filtro.

### Cómo probarlo a mano

1. `dotnet run --project ReservaHoteleraApp.Web` (o F5 en Visual Studio).
2. Entrar como `Rafael1313` → pestaña «Habitaciones».
3. Elegir fechas y pulsar «Buscar disponibles».
4. «Reservar» → revisar hotel/fechas precargadas → «Confirmar reserva».
5. «Mis reservas» para ver el estado y cancelar si hace falta.
6. Como `Aleguzz21`, el enlace «Ver todas» devuelve el inventario completo (con 401 en
   mantenimiento y acciones de edición).

### Pendientes que siguen abiertos

- CRUD completo como Administrador desde la web (Hotel, Habitación, Bloque, Piso, Tipo).
- Validar fechas al **editar** una reserva (hoy no existe esa pantalla).
- El campo `Habitaciones.Estado` («Ocupada») es informativo y no bloquea la reserva: la
  ocupación real la decide el cruce de fechas con `Reservas`.
- `FechaSalida` es `DateTime?` en la BD; el flujo web siempre la guarda con valor.

---

# Parte 6 — Capa API (22/11/2026)

**Rama:** `api/AlexGuzman`
**Estado:** COMPLETADA y verificada. Build 0 errores / 0 advertencias, pruebas 6/6.
**Siguiente paso:** commit + push de la rama y PDF con capturas de Swagger/endpoints.

## 35. Objetivo

Crear la capa `ReservaHoteleraApp.Api` con un controlador por cada entidad del
proyecto, usando el mismo patrón del profe (`SGM.Api`) y el mismo estilo de los
controladores Web (comentarios en español, `var respuesta`,
`if (respuesta.Success == false)` primero), sin tocar la Web.

## 36. Controladores creados (11)

Carpeta: `ReservaHoteleraApp.Api\Controllers`

| Controlador | Endpoints |
|---|---|
| `HotelController` | `GET /api/Hotel`, `GET /api/Hotel/Buscar?destino=`, `GET /api/Hotel/{id}`, `POST CreateHotel`, `POST ModifyHotel`, `POST DisableHotel` |
| `BloqueController` | `GET /api/Bloque`, `GET /api/Bloque/{id}`, `POST CreateBloque`, `POST ModifyBloque`, `POST DisableBloque` |
| `PisoController` | `GET /api/Piso`, `GET /api/Piso/{id}`, `POST CreatePiso`, `POST ModifyPiso`, `POST DisablePiso` |
| `TipoHabitacionController` | `GET /api/TipoHabitacion`, `GET /api/TipoHabitacion/{id}`, `POST Create/Modify/DisableTipoHabitacion` |
| `HabitacionController` | `GET /api/Habitacion`, `GET /api/Habitacion/Disponibles?fechaEntrada=&fechaSalida=`, `GET /api/Habitacion/{id}`, `POST Create/Modify/DisableHabitacion` |
| `ClienteController` | `GET /api/Cliente`, `GET /api/Cliente/{id}`, `GET /api/Cliente/PorUsuario/{idUsuario}`, `POST Create/Modify/DisableCliente` |
| `UsuarioController` | `GET /api/Usuario`, `GET /api/Usuario/{id}`, `GET /api/Usuario/PorNombre/{nombreUsuario}`, `POST ValidarCredenciales`, `POST Create/Modify/DisableUsuario` |
| `ReservaController` | `GET /api/Reserva`, `GET /api/Reserva/ConDetalle`, `GET /api/Reserva/Cliente/{idCliente}`, `GET /api/Reserva/Disponible?idHabitacion=&fechaEntrada=&fechaSalida=`, `GET /api/Reserva/{id}`, `POST Cancelar`, `POST Create/Modify/DisableReserva` |
| `EstadiaController` | `GET /api/Estadia`, `GET /api/Estadia/{id}`, `POST Create/Modify/DisableEstadia` |
| `PagoController` | `GET /api/Pago`, `GET /api/Pago/{id}`, `POST Create/Modify/DisablePago` |
| `RegistroActividadController` | `GET /api/RegistroActividad`, `GET /api/RegistroActividad/{id}`, `POST Create/Modify/DisableRegistroActividad` |

## 37. Cambios de infraestructura

| Archivo | Cambio |
|---|---|
| `ReservaHoteleraApp.Api\Program.cs` | `AddEndpointsApiExplorer()` + `AddSwaggerGen()`; `UseSwagger()` + `UseSwaggerUI()` dentro del bloque `IsDevelopment()`. |
| `ReservaHoteleraApp.Api\ReservaHoteleraApp.Api.csproj` | Se quitó `Microsoft.AspNetCore.OpenApi 10.0.13` (conflicto `NU1107` de `Microsoft.OpenApi`) y se agregó `Swashbuckle.AspNetCore 11.0.1`. |
| `ReservaHoteleraApp.Api\Properties\launchSettings.json` | `launchBrowser: true` y `launchUrl: "swagger"`. |
| `ReservaHoteleraApp.Api\WeatherForecast.cs` y `Controllers\WeatherForecastController.cs` | Eliminados (eran de la plantilla). |
| `ReservaHoteleraApp.DAL\Repositories\ImplementationsDAL\HotelRepository.cs` | `AddAsync` ahora valida `null` y llama a `ValidateHotel` antes de insertar (igual que los otros 10 repositorios). |

Clases simples dentro de los controladores (sin `record`):
`UsuarioRespuesta`, `ValidarCredencialesRequest` (en `UsuarioController`) y
`CancelarReservaRequest` (en `ReservaController`).

## 38. Seguridad

- `UsuarioController` nunca devuelve `Contrasena`: `GetAll`, `GetById`,
  `PorNombre` y `ValidarCredenciales` usan `UsuarioRespuesta`
  (`Id`, `NombreUsuario`, `Rol`, `Activo`).
- La API no tiene autenticación ni autorización (igual que el ejemplo del profe);
  el login se valida con `POST /api/Usuario/ValidarCredenciales`.

## 39. Verificación (22/11/2026)

| Prueba | Resultado |
|---|---|
| `dotnet build ReservaHoteleraApp.slnx` | 0 errores, 0 advertencias |
| `dotnet test ReservaHoteleraApp.slnx` | 6/6 correctas |
| `GET /swagger/index.html` (Development) | 200 |
| Rutas en `swagger/v1/swagger.json` | 64 (11 controladores) |
| GET de las 11 entidades y rutas extra | 200 |
| `GET /api/Usuario` | 200, campos `id, nombreUsuario, rol, activo` (sin contraseña) |
| `GET /api/Hotel/999999` | 400 «El hotel no existe.» |
| POST con cuerpo vacío (`CreateHotel`, `ModifyHotel`, `DisableHotel`, `CreateHabitacion`, `CreatePago`) | 400 |
| `POST /api/Usuario/ValidarCredenciales` con usuario inexistente | 400 |
| `POST /api/Reserva/Cancelar` con reserva inexistente | 400 |
| Web (`/` y `/Hotel`) tras los cambios | 200 (la Web no se modificó) |

## 40. Cómo ejecutar la API

Desde `C:\Users\DELL\source\repos\ReservaHoteleraApp`:

```powershell
dotnet run --project ReservaHoteleraApp.Api
```

Se abre `http://localhost:5161/swagger` (puerto y apertura configurados en
`launchSettings.json`).

Notas:
- Swagger solo se activa con `ASPNETCORE_ENVIRONMENT=Development`.
- Si se ejecuta el `.dll` directo, hay que hacerlo desde la carpeta
  `ReservaHoteleraApp.Api` para que encuentre `appsettings.json` (cadena `HotelDb`).
- Detener la depuración en Visual Studio antes de compilar (bloqueo de `bin`).

## 41. Pendientes

- [ ] `git add` + `git commit` + `git push` de la rama `api/AlexGuzman`.
- [ ] PDF con capturas de Swagger y de los endpoints (el video no se hará).
- [ ] (Opcional) `[ProducesResponseType]` para documentar códigos en Swagger.

## 42. Historial de avance (API)

- **22/11/2026:** 11 controladores de la API creados y ajustados al estilo de la Web.
- **22/11/2026:** Swagger configurado con Swashbuckle 11.0.1; plantilla `WeatherForecast` eliminada.
- **22/11/2026:** `HotelRepository.AddAsync` blindado (null + `ValidateHotel`).
- **22/11/2026:** Verificado build (0/0), pruebas (6/6), Swagger (200, 64 rutas), GET/POST y Web (200).
