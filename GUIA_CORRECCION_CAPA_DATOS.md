# Guía para corregir la capa de datos - ReservaHoteleraApp

Fecha de creación: 06/11/2026
Proyecto: `C:\Users\DELL\source\repos\ReservaHoteleraApp`
Repositorio: `https://github.com/aleguzz21/ReservaHoteleraApp`
Commit revisado: `c722c5d` (`Organizacion Actualizacion`)

Esta guía es para aprender y hacer la corrección paso a paso, sin copiar todo de golpe.

---

## 1. Diagnóstico de la revisión del maestro

El maestro dejó dos comentarios, ambos con calificación 2/3:

### Comentario 1 (5/11/2026 22:56)

> "Veo que se confundió entre los objetos de negocios y los modelos de datos. Entity Framework trabaja con los modelos de datos no con los objetos de negocios, cuando se esté integrando le va a dar error."

### Comentario 2 (5/11/2026 23:44)

> "Su capa de datos no está desarrollada a prueba de fallo."

### Qué encontré en el proyecto

- El proyecto compila con 0 errores.
- Las 4 pruebas de `ReservaHoteleraApp.BL.Tests` pasan.
- El problema no es de compilación, sino de diseño y manejo de errores.

La separación de clases existe:

- `DAL\Models\Objects` contiene DTOs: `ClienteAdd`, `ClienteUpdate`, `ClienteRemove`, `ClienteModel`, etc.
- `DAL\Models\Result` contiene las entidades que usa EF: `Cliente`, `Reserva`, `Habitacion`, etc.
- `HotelDbContext.cs:13-20` usa `Models.Result`, no `Models.Objects`.
- Las configuraciones y migraciones también usan `Models.Result`.

Por eso el comentario 1 no coincide exactamente con el commit actual: o el maestro revisó una versión anterior, o se refiere a que la capa de negocio todavía recibe entidades de EF.

Ejemplo de acoplamiento actual:

- `ClienteRepository.cs:20` devuelve `OperationResult<List<Cliente>>` (la entidad).
- `ClienteServices.cs:6` importa `Models.Result`.
- `ClienteServices.cs:26-30` mapea la entidad a `ClienteModel`.

El comentario 2 sí aplica claramente:

- Los 8 repositorios tienen 0 `try/catch`.
- Todos llaman `SaveChangesAsync()` sin protección.
- Una excepción de base de datos puede escapar del DAL.
- La BL atrapa excepciones en `ServiceBase.cs:19-34`, pero el DAL no las maneja ni las registra.

Ejemplos de fallos reales que pueden ocurrir:

- Documento o correo duplicado por índice único.
- Reserva con `IdCliente` o `IdHabitacion` inexistente.
- Eliminar un cliente que tiene reservas.
- Base de datos caída o timeout.
- Cualquier error inesperado de SQL Server.

---

## 2. Qué pide realmente el maestro

1. Separar responsabilidades:
   - `Models.Objects` = objetos de negocio/DTOs.
   - `Models.Result` = modelos de datos/entidades de EF.
   - EF solo debe trabajar con `Models.Result`.
   - Los `Objects` no deben usarse como entidades ni en `DbSet`, configuraciones o migraciones.

2. Blindar la capa de datos:
   - Cada método del repositorio debe tener `try/catch`.
   - Capturar `DbUpdateException` para duplicados y FKs.
   - Capturar errores de conexión y excepciones generales.
   - Devolver `OperationResult.Failure<T>("mensaje claro")`.
   - Registrar el error con `ILogger`.
   - Manejar casos como "no se puede eliminar porque tiene registros relacionados".
   - Usar transacción cuando se modifiquen varias tablas.

3. Que la BL no dependa de entidades de EF:
   - Lo ideal es que el repositorio entregue `ClienteModel` o listas de modelos.
   - La BL solo debería ver objetos de negocio y `OperationResult`.

4. Mantener el patrón del maestro:
   - Repositorios con `OperationResult`.
   - Servicios con `ExecuteAsync`, validaciones y mapeo.
   - Pruebas unitarias.

---

## 3. Conceptos que hay que dominar

### Objects vs Result

| Carpeta | Qué es | Quién la usa |
|---|---|---|
| `DAL\Models\Objects` | Objetos de negocio/DTOs: `ClienteAdd`, `ClienteUpdate`, `ClienteRemove`, `ClienteModel` | BL, Web/API |
| `DAL\Models\Result` | Modelos de datos/entidades: `Cliente`, `Reserva`, etc. | EF: `DbContext`, `Configurations`, migraciones |
| `DAL\Repositories` | Traduce entre `Objects` y `Result` | BL |

Regla:

> EF solo conoce `Models.Result`. La BL solo debería conocer `Models.Objects` y `OperationResult`.

El repositorio es el traductor:

1. Recibe un `ClienteAdd`.
2. Crea un `Cliente` (entidad).
3. Lo guarda con EF.
4. Devuelve un `OperationResult`.

### Qué significa "a prueba de fallo"

Significa que un método del repositorio nunca debe lanzar una excepción hacia afuera. Siempre debe devolver una caja `OperationResult<T>`:

- `Success = true` si todo salió bien.
- `Success = false` y un `Message` claro si algo falló.

Fallos que hay que controlar:

- Datos inválidos.
- Registro que no existe.
- Documento/correo duplicado.
- FK inexistente.
- Borrar un cliente que tiene reservas.
- Base de datos caída o timeout.
- Cualquier error inesperado.

---

## 4. Anatomía de un método robusto

Todo método del repositorio debe seguir este orden:

1. Validar la entrada.
2. Ejecutar la operación dentro de `try`.
3. Capturar errores esperados: `DbUpdateException`.
4. Capturar errores inesperados: `Exception`.
5. Registrar el error con `ILogger`.
6. Devolver `Failure<T>("mensaje para el usuario")`.

Esqueleto:

```csharp
public async Task<OperationResult<bool>> AddAsync(ClienteAdd cliente)
{
    try
    {
        if (cliente is null)
            return Failure<bool>("El objeto ClienteAdd es requerido.");

        // validaciones
        // crear entidad
        // guardar

        return new OperationResult<bool>
        {
            Data = true,
            Message = "Cliente registrado correctamente."
        };
    }
    catch (DbUpdateException ex)
    {
        logger.LogError(ex, "Error de base de datos al registrar cliente.");
        return Failure<bool>("No se pudo guardar el cliente. Revise datos duplicados o referencias inexistentes.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error inesperado al registrar cliente.");
        return Failure<bool>("Ocurrió un error inesperado al guardar el cliente.");
    }
}
```

Reglas:

- No hacer `throw`.
- No devolver `ex.Message` al usuario.
- El detalle técnico va al log.
- El usuario recibe un mensaje entendible.

---

## 5. Plan de trabajo paso a paso

### Fase 1: preparar el repositorio de ejemplo

Archivo:

`ReservaHoteleraApp.DAL\Repositories\ImplementationsDAL\ClienteRepository.cs`

1. Agregar el campo:

```csharp
private readonly ILogger<ClienteRepository> logger;
```

2. Agregar el parámetro al constructor:

```csharp
public ClienteRepository(HotelDbContext context, ILogger<ClienteRepository> logger)
{
    this.context = context;
    this.logger = logger;
}
```

3. Agregar los `using` necesarios:

```csharp
using Microsoft.Extensions.Logging;
```

4. Si el compilador se queja de `ILogger`, agregar al `.csproj` del DAL:

```xml
<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.13" />
```

### Fase 2: blindar AddAsync

1. Envolver todo el cuerpo de `AddAsync` en `try/catch`.
2. Mantener las validaciones actuales.
3. Agregar `catch (DbUpdateException ex)`.
4. Agregar `catch (Exception ex)`.
5. Compilar:

```powershell
dotnet build
```

6. Probar los casos de la sección 6.

### Fase 3: blindar el resto de ClienteRepository

Hacer uno por uno, compilando después de cada método:

- `UpdateAsync`
- `DeleteAsync`
- `GetAllAsync`
- `GetByIdAsync`

En `GetByIdAsync`, considerar devolver `OperationResult.Failure` en vez de `null` cuando no exista, para mantener el mismo patrón.

### Fase 4: replicar en los otros 7 repositorios

Orden sugerido:

1. `UsuarioRepository`
2. `TipoHabitacionRepository`
3. `HabitacionRepository`
4. `ReservaRepository`
5. `EstadiaRepository`
6. `PagoRepository`
7. `RegistroActividadRepository`

En cada uno:

- Agregar `ILogger<T>`.
- Envolver cada método en `try/catch`.
- Capturar `DbUpdateException`.
- Capturar `Exception`.
- Devolver `Failure<T>` con mensajes claros.
- Compilar después de cada repositorio.

### Fase 5: separación completa (opcional, pero recomendada)

Objetivo: que la BL no importe `Models.Result`.

Opciones:

1. Que `GetAllAsync` del repositorio devuelva `OperationResult<List<ClienteModel>>` y haga el mapeo dentro del DAL.
2. Que `GetByIdAsync` devuelva `OperationResult<ClienteModel>` directamente.
3. Quitar `using ReservaHoteleraApp.DAL.Models.Result;` de las implementaciones de BL.
4. Dejar el mapeo entidad -> modelo en el DAL.

Antes de hacerlo, confirmar con el maestro si prefiere ese enfoque, porque su repositorio de ejemplo devuelve entidades en `GetAllAsync`.

### Fase 6: pruebas

Agregar pruebas para:

- Agregar cliente válido.
- Agregar cliente duplicado.
- Agregar cliente con usuario inexistente.
- Actualizar cliente inexistente.
- Eliminar cliente con reservas.
- Consultar cliente inexistente.
- Error de conexión (si se puede simular).

---

## 6. Cómo probar que la capa de datos es "a prueba de fallo"

1. Registrar un cliente normal:
   - Debe devolver `Success = true`.

2. Registrar dos clientes con el mismo `Documento`:
   - Debe devolver `Success = false`.
   - No debe lanzar una excepción.

3. Registrar un cliente con `IdUsuario = 999`:
   - Debe devolver `Success = false`.

4. Intentar eliminar un cliente con reservas:
   - Debe devolver `Success = false` con un mensaje claro.

5. Detener LocalDB o usar una cadena de conexión inválida:
   - Debe devolver `Success = false`.
   - No debe tumbar la aplicación.

6. Actualizar un cliente que no existe:
   - Debe devolver `Success = false`.

7. Consultar un cliente que no existe:
   - Debe devolver `Success = false` o un resultado controlado.

---

## 7. Checklist general

### ClienteRepository

- [ ] `ILogger` agregado.
- [ ] `AddAsync` con `try/catch`.
- [ ] `UpdateAsync` con `try/catch`.
- [ ] `DeleteAsync` con `try/catch`.
- [ ] `GetAllAsync` con `try/catch`.
- [ ] `GetByIdAsync` con `try/catch`.
- [ ] Compila.
- [ ] Pruebas manuales hechas.

### UsuarioRepository

- [ ] `ILogger` agregado.
- [ ] Métodos con `try/catch`.
- [ ] Compila.

### TipoHabitacionRepository

- [ ] `ILogger` agregado.
- [ ] Métodos con `try/catch`.
- [ ] Compila.

### HabitacionRepository

- [ ] `ILogger` agregado.
- [ ] Métodos con `try/catch`.
- [ ] Compila.

### ReservaRepository

- [ ] `ILogger` agregado.
- [ ] Métodos con `try/catch`.
- [ ] Compila.

### EstadiaRepository

- [ ] `ILogger` agregado.
- [ ] Métodos con `try/catch`.
- [ ] Compila.

### PagoRepository

- [ ] `ILogger` agregado.
- [ ] Métodos con `try/catch`.
- [ ] Compila.

### RegistroActividadRepository

- [ ] `ILogger` agregado.
- [ ] Métodos con `try/catch`.
- [ ] Compila.

---

## 8. Comandos útiles

```powershell
# Compilar
dotnet build C:\Users\DELL\source\repos\ReservaHoteleraApp\ReservaHoteleraApp.slnx

# Ejecutar pruebas
dotnet test C:\Users\DELL\source\repos\ReservaHoteleraApp\ReservaHoteleraApp.BL.Tests\ReservaHoteleraApp.BL.Tests.csproj

# Ver estado de Git
git -C C:\Users\DELL\source\repos\ReservaHoteleraApp status

# Ver últimos commits
git -C C:\Users\DELL\source\repos\ReservaHoteleraApp log --oneline -10
```

---

## 9. Reglas de oro mientras practicas

- No usar `catch { }` vacío.
- No usar `catch (Exception) { }` sin log.
- No devolver `ex.Message` al usuario.
- Hacer un método a la vez.
- Compilar después de cada método.
- No copiar y pegar sin entender.
- Si algo no cuadra, preguntar antes de seguir.

---

## 10. Primer ejercicio para retomar

Cuando vuelvas, empieza por:

1. Abrir `ClienteRepository.cs`.
2. Agregar `ILogger<ClienteRepository>`.
3. Blindar solo `AddAsync`.
4. Ejecutar `dotnet build`.
5. Probar un cliente válido y un cliente duplicado.

Cuando termines, revisar:

- ¿El método devuelve `OperationResult` en todos los caminos?
- ¿Hay algún `throw`?
- ¿El log guarda el error completo?
- ¿El mensaje al usuario es entendible?
- ¿La aplicación sigue funcionando si la base falla?

---

## 11. Nota importante

El proyecto ya tiene una base correcta: capas, DTOs, entidades, `OperationResult`, migraciones, DI y pruebas. No hay que rehacerlo. El trabajo principal es blindar los repositorios y, si se decide, mover el mapeo de entidades al DAL para que la BL no dependa de EF.

---

## 12. Estado de avance (09/11/2026)

Trabajo completado y subido a GitHub.

- Repositorio: `https://github.com/aleguzz21/ReservaHoteleraApp`
- Rama: `master`
- Commit: `f951206` (`actualizacion`)
- Verificado: el commit local y `origin/master` son el mismo (`f951206`).

### Checklist final

- [x] Paquete `Microsoft.Extensions.Logging.Abstractions` agregado al DAL.
- [x] Los 8 repositorios tienen `ILogger`.
- [x] Los 8 repositorios tienen `try/catch` en `GetAll`, `GetById`, `Add`, `Update` y `Delete`.
- [x] Se captura `DbUpdateException` (duplicados y FK inexistentes).
- [x] Se captura `Exception` (conexión caída, timeout, errores inesperados).
- [x] Todos los caminos devuelven `OperationResult` (sin `throw`).
- [x] `GetByIdAsync` devuelve `Failure` cuando el registro no existe (ya no `null`).
- [x] `IRepository` y las 8 interfaces devuelven `XModel` (objetos de negocio), no entidades de EF.
- [x] El mapeo entidad -> modelo se movió al DAL.
- [x] La BL ya no usa `Models.Result` (0 ocurrencias).
- [x] EF (Data, Configurations, Migrations) solo usa `Models.Result` (0 ocurrencias de `Models.Objects`).
- [x] Pruebas agregadas para casos de fallo (repositorio que devuelve `Failure`).
- [x] `dotnet build`: 0 errores, 0 advertencias.
- [x] `dotnet test`: 6 de 6 pruebas correctas.

### Cómo quedaron los comentarios del maestro

1. "Confundió objetos de negocio con modelos de datos": resuelto. EF trabaja solo con `Models.Result` y la BL solo con `Models.Objects`; el repositorio es el traductor.
2. "La capa de datos no está a prueba de fallo": resuelto. Los 8 repositorios capturan errores y devuelven `Failure` con mensajes claros, registrando el detalle en el log.

### Principios respetados

- Responsabilidad única: repositorio = datos + mapeo; servicio = reglas de negocio.
- Inversión de dependencias: la BL depende de interfaces (`IXRepository`), no de EF.
- Interfaz genérica reutilizable: `IRepository<TModel, TAdd, TUpdate, TRemove>`.
- Una interfaz por entidad (segregación de interfaces).
