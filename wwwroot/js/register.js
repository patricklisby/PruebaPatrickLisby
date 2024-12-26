document.getElementById("registroForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const idCedulaUsuario = document.getElementById("idCedulaUsuario").value;
    const nombreUsuario = document.getElementById("nombreUsuario").value;
    const apellidoUsuario = document.getElementById("apellidoUsuario").value;
    const correoElectronicoUsuario = document.getElementById("correoElectronicoUsuario").value;
    const contrasenaUsuario = document.getElementById("contrasenaUsuario").value;

    try {
        const response = await fetch("/api/Usuario/crearUsuario", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                idCedulaUsuario,
                nombreUsuario,
                apellidoUsuario,
                correoElectronicoUsuario,
                contrasenaUsuario,
            }),
        });

        if (response.ok) {
            mostrarNotificacion("Usuario registrado exitosamente.", "success");
            setTimeout(() => {
                window.location.href = "/Login"; // Redirige al login después del registro
            }, 2000);
        } else {
            const error = await response.json();
            mostrarNotificacion(error.mensaje, "error");
        }
    } catch (error) {
        mostrarNotificacion("Error al conectar con el servidor.", "error");
    }
});

function mostrarNotificacion(mensaje, tipo) {
    const container = document.getElementById("notificationContainer");
    const notification = document.createElement("div");

    notification.className = `p-4 rounded-md shadow-md text-white text-sm ${tipo === "success" ? "bg-green-500" : "bg-red-500"
        }`;
    notification.textContent = mensaje;

    container.appendChild(notification);

    setTimeout(() => {
        notification.remove();
    }, 3000); // La notificación desaparece después de 3 segundos
}
