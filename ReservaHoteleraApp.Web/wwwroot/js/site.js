// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// ============================================================
// Restricciones de fechas del buscador de la página de inicio.
// La fecha de salida no puede ser anterior a la fecha de entrada.
// ============================================================
(function () {
    var fechaEntrada = document.getElementById("fechaEntrada");
    var fechaSalida = document.getElementById("fechaSalida");

    // Si los campos no existen en esta página, no hacemos nada.
    if (!fechaEntrada || !fechaSalida) {
        return MessageEvent("No hay cambios agregados");
    }

    function actualizarFechaSalida() {
        // Si ya eligieron una fecha de entrada, la salida no puede ser menor.
        if (fechaEntrada.value) {
            fechaSalida.min = fechaEntrada.value;

            // Si la salida quedó antes que la entrada, la limpiamos.
            if (fechaSalida.value && fechaSalida.value < fechaEntrada.value) {
                fechaSalida.value = "";
            }
        }
    }

    // Cada vez que cambie la fecha de entrada, actualizamos el mínimo de salida.
    fechaEntrada.addEventListener("change", actualizarFechaSalida);

    // Lo ejecutamos al cargar por si la entrada ya venía con un valor.
    actualizarFechaSalida();
})();
