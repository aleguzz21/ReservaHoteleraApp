using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReservaHoteleraApp.BL.Services.Interfaces;
using ReservaHoteleraApp.DAL.Models.Objects.Bloque;
using ReservaHoteleraApp.DAL.Models.Objects.Habitacion;
using ReservaHoteleraApp.DAL.Models.Objects.Hotel;
using ReservaHoteleraApp.DAL.Models.Objects.Piso;
using ReservaHoteleraApp.DAL.Models.Objects.TipoHabitacion;
using ReservaHoteleraApp.Web.Models;

namespace ReservaHoteleraApp.Web.Controllers
{
    // Controlador del módulo "Hoteles".
    // Recibe las peticiones del navegador, llama al servicio y devuelve una vista.
    // El controlador NO habla directamente con la base de datos.
    public class HotelController : Controller
    {
        // Servicio de negocio de los hoteles.
        private readonly IHotelServices _hotelServices;

        // Servicios de todo lo que se configura dentro de un hotel.
        private readonly IBloqueServices _bloqueServices;
        private readonly IPisoServices _pisoServices;
        private readonly ITipoHabitacionServices _tipoHabitacionServices;
        private readonly IHabitacionServices _habitacionServices;

        // ASP.NET Core nos entrega los servicios ya creados (inyección de dependencias).
        public HotelController(
            IHotelServices hotelServices,
            IBloqueServices bloqueServices,
            IPisoServices pisoServices,
            ITipoHabitacionServices tipoHabitacionServices,
            IHabitacionServices habitacionServices)
        {
            _hotelServices = hotelServices;
            _bloqueServices = bloqueServices;
            _pisoServices = pisoServices;
            _tipoHabitacionServices = tipoHabitacionServices;
            _habitacionServices = habitacionServices;
        }

        // ---------------------------------------------------------
        // LISTAR HOTELES
        // GET: /Hotel
        // También recibe los datos del buscador de la página de inicio.
        // ---------------------------------------------------------
        public async Task<IActionResult> Index(string? destino, DateTime? fechaEntrada, DateTime? fechaSalida, int? adultos, int? ninos, int? habitaciones)
        {
            // 1. Pedimos los hoteles al servicio.
            //    La búsqueda por destino es una regla de negocio y vive en la capa BL.
            var respuesta = await _hotelServices.BuscarAsync(destino);

            // 2. Si hubo un error, guardamos el mensaje para mostrarlo en la vista.
            if (respuesta.Success == false)
            {
                TempData["Error"] = respuesta.Message;
            }

            // 3. Obtenemos la lista.
            //    Si viene vacía, creamos una lista vacía para que la vista no falle.
            var hoteles = respuesta.Data;
            if (hoteles == null)
            {
                hoteles = new List<HotelModel>();
            }

            // 4. Guardamos el destino y los demás datos del buscador por si la vista los necesita.
            if (string.IsNullOrWhiteSpace(destino) == false)
            {
                ViewData["Destino"] = destino.Trim();
            }

            ViewData["FechaEntrada"] = fechaEntrada;
            ViewData["FechaSalida"] = fechaSalida;
            ViewData["Adultos"] = adultos;
            ViewData["Ninos"] = ninos;
            ViewData["Habitaciones"] = habitaciones;

            // 5. Enviamos la lista a la vista Views/Hotel/Index.cshtml.
            return View(hoteles);
        }

        // ---------------------------------------------------------
        // DETALLES DE UN HOTEL
        // GET: /Hotel/Details/5
        // ---------------------------------------------------------
        public async Task<IActionResult> Details(int id)
        {
            var respuesta = await _hotelServices.GetByIdAsync(id);

            // Si el hotel no existe o hubo un error, volvemos a la lista.
            if (respuesta.Success == false || respuesta.Data == null)
            {
                TempData["Error"] = respuesta.Message;
                return RedirectToAction("Index");
            }

            return View(respuesta.Data);
        }

        // ---------------------------------------------------------
        // CREAR HOTEL (mostrar formulario)
        // GET: /Hotel/Create
       // ---------------------------------------------------------
       [Authorize(Roles ="Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // ---------------------------------------------------------
        // CREAR HOTEL (guardar formulario)
        // POST: /Hotel/Create        
        // ---------------------------------------------------------
        [Authorize(Roles ="Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Protege contra ataques CSRF.
        public async Task<IActionResult> Create(HotelAdd hotel)
        {
            // Si el formulario tiene errores de validación, lo mostramos otra vez.
            if (ModelState.IsValid == false)
            {
                return View(hotel);
            }

            var respuesta = await _hotelServices.AddAsync(hotel);

            // Si el servicio no pudo guardar, mostramos el error en el formulario.
            if (respuesta.Success == false)
            {
                string? mensajeDelServicio = respuesta.Message;
                string mensaje = "No se pudo registrar el hotel.";

                if (string.IsNullOrEmpty(mensajeDelServicio) == false)
                {
                    mensaje = mensajeDelServicio;
                }

                ModelState.AddModelError("", mensaje);
                return View(hotel);
            }

            // Si todo salió bien, mostramos el mensaje y volvemos a la lista.
            TempData["Success"] = respuesta.Message;
            return RedirectToAction("Index");
        }

        // ---------------------------------------------------------
        // EDITAR HOTEL (mostrar formulario)
        // GET: /Hotel/Edit/5        
        // ---------------------------------------------------------
        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            var respuesta = await _hotelServices.GetByIdAsync(id);

            if (respuesta.Success == false || respuesta.Data == null)
            {
                TempData["Error"] = respuesta.Message;
                return RedirectToAction("Index");
            }

            // Copiamos los datos del hotel al objeto que usa el formulario de edición.
            var hotelParaEditar = new HotelUpdate();
            hotelParaEditar.Id = respuesta.Data.Id;
            hotelParaEditar.Nombre = respuesta.Data.Nombre;
            hotelParaEditar.Direccion = respuesta.Data.Direccion;
            hotelParaEditar.Telefono = respuesta.Data.Telefono;
            hotelParaEditar.Ciudad = respuesta.Data.Ciudad;

            return View(hotelParaEditar);
        }

        // ---------------------------------------------------------
        // EDITAR HOTEL (guardar cambios)
        // POST: /Hotel/Edit
        // ---------------------------------------------------------
        [Authorize(Roles ="Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HotelUpdate hotel)
        {
            if (ModelState.IsValid == false)
            {
                return View(hotel);
            }

            var respuesta = await _hotelServices.UpdateAsync(hotel);

            if (respuesta.Success == false)
            {
                string? mensajeDelServicio = respuesta.Message;
                string mensaje = "No se pudo actualizar el hotel.";

                if (string.IsNullOrEmpty(mensajeDelServicio) == false)
                {
                    mensaje = mensajeDelServicio;
                }

                ModelState.AddModelError("", mensaje);
                return View(hotel);
            }

            TempData["Success"] = respuesta.Message;
            return RedirectToAction("Index");
        }

        // ---------------------------------------------------------
        // ELIMINAR HOTEL (mostrar confirmación)
        // GET: /Hotel/Delete/5
        // ---------------------------------------------------------
        [Authorize(Roles ="Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var respuesta = await _hotelServices.GetByIdAsync(id);

            if (respuesta.Success == false || respuesta.Data == null)
            {
                TempData["Error"] = respuesta.Message;
                return RedirectToAction("Index");
            }

            return View(respuesta.Data);
        }

        // ---------------------------------------------------------
        // ELIMINAR HOTEL (confirmado)
        // POST: /Hotel/Delete
        // Se llama "DeleteConfirmed" porque el GET ya se llama "Delete".
        // ---------------------------------------------------------
        [Authorize(Roles ="Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Creamos el objeto que necesita el servicio para eliminar.
            var hotelParaEliminar = new HotelRemove();
            hotelParaEliminar.Id = id;

            var respuesta = await _hotelServices.DeleteAsync(hotelParaEliminar);

            if (respuesta.Success == false)
            {
                TempData["Error"] = respuesta.Message;
            }
            else
            {
                TempData["Success"] = respuesta.Message;
            }

            return RedirectToAction("Index");
        }

        // ---------------------------------------------------------
        // CONFIGURACIÓN UNIFICADA DEL HOTEL
        // GET: /Hotel/Configurar/5
        // Muestra en una sola página los bloques, pisos, tipos de
        // habitación y habitaciones del hotel, con sus formularios.
        // ---------------------------------------------------------
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Configurar(int id, string? editar, int? editarId)
        {
            // 1. Armamos el modelo con el hotel y todas sus listas.
            var configuracion = await ConstruirConfiguracionAsync(id);

            // 2. Si el hotel no existe, volvemos a la lista.
            if (configuracion == null)
            {
                TempData["Error"] = "El hotel no existe.";
                return RedirectToAction("Index");
            }

            // 3. Si la URL pide editar algo, precargamos ese formulario.
            if (editarId.HasValue)
            {
                if (editar == "bloque")
                {
                    var bloque = configuracion.Bloques.FirstOrDefault(b => b.Id == editarId.Value);
                    if (bloque != null)
                    {
                        configuracion.BloqueForm = new BloqueConfigForm
                        {
                            Id = bloque.Id,
                            Nombre = bloque.Nombre,
                            Descripcion = bloque.Descripcion
                        };
                        configuracion.SeccionEdicion = "bloque";
                    }
                }
                else if (editar == "piso")
                {
                    var piso = configuracion.Pisos.FirstOrDefault(p => p.Id == editarId.Value);
                    if (piso != null)
                    {
                        configuracion.PisoForm = new PisoConfigForm
                        {
                            Id = piso.Id,
                            Numero = piso.Numero,
                            IdBloque = piso.IdBloque,
                            Descripcion = piso.Descripcion
                        };
                        configuracion.SeccionEdicion = "piso";
                    }
                }
                else if (editar == "tipo")
                {
                    var tipo = configuracion.Tipos.FirstOrDefault(t => t.Id == editarId.Value);
                    if (tipo != null)
                    {
                        configuracion.TipoForm = new TipoHabitacionConfigForm
                        {
                            Id = tipo.Id,
                            Nombre = tipo.Nombre,
                            Descripcion = tipo.Descripcion,
                            Capacidad = tipo.Capacidad,
                            Precio = tipo.Precio
                        };
                        configuracion.SeccionEdicion = "tipo";
                    }
                }
                else if (editar == "habitacion")
                {
                    var habitacion = configuracion.Habitaciones.FirstOrDefault(h => h.Id == editarId.Value);
                    if (habitacion != null)
                    {
                        configuracion.HabitacionForm = new HabitacionConfigForm
                        {
                            Id = habitacion.Id,
                            Numero = habitacion.Numero,
                            IdPiso = habitacion.IdPiso,
                            IdTipoHabitacion = habitacion.IdTipoHabitacion,
                            PrecioNoche = habitacion.PrecioNoche,
                            Estado = habitacion.Estado
                        };
                        configuracion.SeccionEdicion = "habitacion";
                    }
                }
            }

            // 4. Enviamos todo a Views/Hotel/Configurar.cshtml.
            return View(configuracion);
        }

        // ---------------------------------------------------------
        // GUARDAR BLOQUE (crear o editar)
        // POST: /Hotel/GuardarBloque
        // ---------------------------------------------------------
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarBloque(int hotelId, BloqueConfigForm BloqueForm)
        {
            if (ModelState.IsValid == false)
            {
                return await VolverConErrorAsync(hotelId, configuracion => configuracion.BloqueForm = BloqueForm, BloqueForm.Id > 0 ? "bloque" : null);
            }

            var respuesta = BloqueForm.Id > 0
                ? await _bloqueServices.UpdateAsync(new BloqueUpdate
                {
                    Id = BloqueForm.Id,
                    Nombre = BloqueForm.Nombre,
                    IdHotel = hotelId,
                    Descripcion = BloqueForm.Descripcion
                })
                : await _bloqueServices.AddAsync(new BloqueAdd
                {
                    Nombre = BloqueForm.Nombre,
                    IdHotel = hotelId,
                    Descripcion = BloqueForm.Descripcion
                });

            GuardarMensaje(respuesta.Success, respuesta.Message, "No se pudo guardar el bloque.");
            return RedirectToAction(nameof(Configurar), new { id = hotelId });
        }

        // ---------------------------------------------------------
        // GUARDAR PISO (crear o editar)
        // POST: /Hotel/GuardarPiso
        // ---------------------------------------------------------
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarPiso(int hotelId, PisoConfigForm PisoForm)
        {
            if (ModelState.IsValid == false)
            {
                return await VolverConErrorAsync(hotelId, configuracion => configuracion.PisoForm = PisoForm, PisoForm.Id > 0 ? "piso" : null);
            }

            var respuesta = PisoForm.Id > 0
                ? await _pisoServices.UpdateAsync(new PisoUpdate
                {
                    Id = PisoForm.Id,
                    Numero = PisoForm.Numero,
                    IdBloque = PisoForm.IdBloque,
                    Descripcion = PisoForm.Descripcion
                })
                : await _pisoServices.AddAsync(new PisoAdd
                {
                    Numero = PisoForm.Numero,
                    IdBloque = PisoForm.IdBloque,
                    Descripcion = PisoForm.Descripcion
                });

            GuardarMensaje(respuesta.Success, respuesta.Message, "No se pudo guardar el piso.");
            return RedirectToAction(nameof(Configurar), new { id = hotelId });
        }

        // ---------------------------------------------------------
        // GUARDAR TIPO DE HABITACIÓN (crear o editar)
        // POST: /Hotel/GuardarTipo
        // ---------------------------------------------------------
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarTipo(int hotelId, TipoHabitacionConfigForm TipoForm)
        {
            if (ModelState.IsValid == false)
            {
                return await VolverConErrorAsync(hotelId, configuracion => configuracion.TipoForm = TipoForm, TipoForm.Id > 0 ? "tipo" : null);
            }

            var respuesta = TipoForm.Id > 0
                ? await _tipoHabitacionServices.UpdateAsync(new TipoHabitacionUpdate
                {
                    Id = TipoForm.Id,
                    Nombre = TipoForm.Nombre,
                    Descripcion = TipoForm.Descripcion,
                    Capacidad = TipoForm.Capacidad,
                    Precio = TipoForm.Precio,
                    IdHotel = hotelId
                })
                : await _tipoHabitacionServices.AddAsync(new TipoHabitacionAdd
                {
                    Nombre = TipoForm.Nombre,
                    Descripcion = TipoForm.Descripcion,
                    Capacidad = TipoForm.Capacidad,
                    Precio = TipoForm.Precio,
                    IdHotel = hotelId
                });

            GuardarMensaje(respuesta.Success, respuesta.Message, "No se pudo guardar el tipo de habitación.");
            return RedirectToAction(nameof(Configurar), new { id = hotelId });
        }

        // ---------------------------------------------------------
        // GUARDAR HABITACIÓN (crear o editar)
        // POST: /Hotel/GuardarHabitacion
        // ---------------------------------------------------------
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarHabitacion(int hotelId, HabitacionConfigForm HabitacionForm)
        {
            if (ModelState.IsValid == false)
            {
                return await VolverConErrorAsync(hotelId, configuracion => configuracion.HabitacionForm = HabitacionForm, HabitacionForm.Id > 0 ? "habitacion" : null);
            }

            var respuesta = HabitacionForm.Id > 0
                ? await _habitacionServices.UpdateAsync(new HabitacionUpdate
                {
                    Id = HabitacionForm.Id,
                    Numero = HabitacionForm.Numero,
                    IdPiso = HabitacionForm.IdPiso,
                    IdTipoHabitacion = HabitacionForm.IdTipoHabitacion,
                    PrecioNoche = HabitacionForm.PrecioNoche,
                    Estado = HabitacionForm.Estado
                })
                : await _habitacionServices.AddAsync(new HabitacionAdd
                {
                    Numero = HabitacionForm.Numero,
                    IdPiso = HabitacionForm.IdPiso,
                    IdTipoHabitacion = HabitacionForm.IdTipoHabitacion,
                    PrecioNoche = HabitacionForm.PrecioNoche,
                    Estado = HabitacionForm.Estado
                });

            GuardarMensaje(respuesta.Success, respuesta.Message, "No se pudo guardar la habitación.");
            return RedirectToAction(nameof(Configurar), new { id = hotelId });
        }

        // ---------------------------------------------------------
        // ELIMINAR BLOQUE
        // POST: /Hotel/EliminarBloque
        // ---------------------------------------------------------
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarBloque(int hotelId, int id)
        {
            var respuesta = await _bloqueServices.DeleteAsync(new BloqueRemove { Id = id });
            GuardarMensaje(respuesta.Success, respuesta.Message, "No se pudo eliminar el bloque.");
            return RedirectToAction(nameof(Configurar), new { id = hotelId });
        }

        // ---------------------------------------------------------
        // ELIMINAR PISO
        // POST: /Hotel/EliminarPiso
        // ---------------------------------------------------------
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPiso(int hotelId, int id)
        {
            var respuesta = await _pisoServices.DeleteAsync(new PisoRemove { Id = id });
            GuardarMensaje(respuesta.Success, respuesta.Message, "No se pudo eliminar el piso.");
            return RedirectToAction(nameof(Configurar), new { id = hotelId });
        }

        // ---------------------------------------------------------
        // ELIMINAR TIPO DE HABITACIÓN
        // POST: /Hotel/EliminarTipo
        // ---------------------------------------------------------
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarTipo(int hotelId, int id)
        {
            var respuesta = await _tipoHabitacionServices.DeleteAsync(new TipoHabitacionRemove { Id = id });
            GuardarMensaje(respuesta.Success, respuesta.Message, "No se pudo eliminar el tipo de habitación.");
            return RedirectToAction(nameof(Configurar), new { id = hotelId });
        }

        // ---------------------------------------------------------
        // ELIMINAR HABITACIÓN
        // POST: /Hotel/EliminarHabitacion
        // ---------------------------------------------------------
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarHabitacion(int hotelId, int id)
        {
            var respuesta = await _habitacionServices.DeleteAsync(new HabitacionRemove { Id = id });
            GuardarMensaje(respuesta.Success, respuesta.Message, "No se pudo eliminar la habitación.");
            return RedirectToAction(nameof(Configurar), new { id = hotelId });
        }

        // ---------------------------------------------------------
        // Métodos de apoyo (privados)
        // ---------------------------------------------------------

        // Construye el modelo de la página de configuración con el hotel
        // y solo los bloques, pisos y habitaciones que le pertenecen.
        private async Task<HotelConfiguracionViewModel?> ConstruirConfiguracionAsync(int hotelId)
        {
            var hotelRespuesta = await _hotelServices.GetByIdAsync(hotelId);
            if (hotelRespuesta.Success == false || hotelRespuesta.Data == null)
            {
                return null;
            }

            var bloquesRespuesta = await _bloqueServices.GetAllAsync();
            var bloques = (bloquesRespuesta.Data ?? new List<BloqueModel>())
                .Where(b => b.IdHotel == hotelId)
                .ToList();

            var bloqueIds = bloques.Select(b => b.Id).ToHashSet();

            var pisosRespuesta = await _pisoServices.GetAllAsync();
            var pisos = (pisosRespuesta.Data ?? new List<PisoModel>())
                .Where(p => bloqueIds.Contains(p.IdBloque))
                .ToList();

            var pisoIds = pisos.Select(p => p.Id).ToHashSet();

            // Los tipos de habitación ahora pertenecen a un hotel: mostramos solo los de este hotel.
            var tiposRespuesta = await _tipoHabitacionServices.GetAllAsync();
            var tipos = (tiposRespuesta.Data ?? new List<TipoHabitacionModel>())
                .Where(t => t.IdHotel == hotelId)
                .ToList();

            var habitacionesRespuesta = await _habitacionServices.GetAllAsync();
            var habitaciones = (habitacionesRespuesta.Data ?? new List<HabitacionModel>())
                .Where(h => pisoIds.Contains(h.IdPiso))
                .ToList();

            return new HotelConfiguracionViewModel
            {
                Hotel = hotelRespuesta.Data,
                Bloques = bloques,
                Pisos = pisos,
                Tipos = tipos,
                Habitaciones = habitaciones
            };
        }

        // Vuelve a mostrar la configuración conservando el formulario con errores.
        private async Task<IActionResult> VolverConErrorAsync(
            int hotelId,
            Action<HotelConfiguracionViewModel> aplicarFormulario,
            string? seccionEdicion)
        {
            var configuracion = await ConstruirConfiguracionAsync(hotelId);
            if (configuracion == null)
            {
                TempData["Error"] = "El hotel no existe.";
                return RedirectToAction("Index");
            }

            aplicarFormulario(configuracion);
            configuracion.SeccionEdicion = seccionEdicion;
            return View("Configurar", configuracion);
        }

        // Guarda en TempData el mensaje que verá el usuario tras el POST.
        private void GuardarMensaje(bool success, string? mensajeServicio, string mensajePorDefecto)
        {
            string mensaje = mensajePorDefecto;
            if (string.IsNullOrEmpty(mensajeServicio) == false)
            {
                mensaje = mensajeServicio;
            }

            if (success)
            {
                TempData["Success"] = mensaje;
            }
            else
            {
                TempData["Error"] = mensaje;
            }
        }
    }
}
