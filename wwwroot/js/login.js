export async function logout() {
    try {
        const response = await fetch("/api/ApiLogin/logout", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (response.ok) {
            window.location.href = "/Login/LoginForm"; // Redirigir al formulario de login
        } else {
            console.error("Error al cerrar sesión.");
            alert("Hubo un problema al cerrar sesión.");
        }
    } catch (error) {
        console.error("Error de red:", error);
        alert("No se pudo conectar al servidor.");
    }
}
