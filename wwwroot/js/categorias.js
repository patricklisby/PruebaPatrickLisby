document.addEventListener("DOMContentLoaded", () => {
    const apiCategoriaBaseUrl = "/api/Categoria";
    const modalContainer = document.getElementById("modalContainer");
    const modalContent = document.getElementById("modalContent");
    const alertContainer = document.getElementById("alertContainer");

    // Función para mostrar alertas
    function mostrarAlerta(tipo, mensaje) {
        const alert = document.createElement("div");
        alert.className = `p-4 rounded-md shadow-md text-white text-sm ${
            tipo === "success" ? "bg-green-500" : "bg-red-500"
        }`;
        alert.textContent = mensaje;

        alertContainer.appendChild(alert);

        setTimeout(() => {
            alert.remove();
        }, 3000);
    }

    // Abrir modal
    function abrirModal(content) {
        modalContent.innerHTML = content;
        modalContainer.classList.remove("hidden");

        const closeModalButton = document.getElementById("closeModal");
        if (closeModalButton) {
            closeModalButton.addEventListener("click", () => {
                modalContainer.classList.add("hidden");
            });
        }
    }

    // Cargar categorías en una tabla
    async function cargarCategorias() {
        try {
            const response = await fetch(`${apiCategoriaBaseUrl}/obtenerCategorias`);
            if (!response.ok) throw new Error("Error al cargar las categorías.");

            const categorias = await response.json();

            const categoriaTableBody = document.getElementById("categoriaTableBody");
            if (!categoriaTableBody) {
                console.warn("Elemento 'categoriaTableBody' no encontrado en el DOM.");
                return;
            }

            categoriaTableBody.innerHTML = ""; // Limpiar tabla

            if (categorias.length === 0) {
                categoriaTableBody.innerHTML = `<tr><td colspan="2" class="text-center text-gray-500">No hay categorías disponibles.</td></tr>`;
                return;
            }

            categorias.forEach((categoria) => {
                const row = document.createElement("tr");
                row.innerHTML = `
                <td class="px-4 py-2">${categoria.descripcionCategoria}</td>
                <td class="px-4 py-2 text-right">
                    <button data-id="${categoria.idCategoria}" class="bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600 mr-2 btnEditarCategoria">Editar</button>
                    <button data-id="${categoria.idCategoria}" class="bg-red-500 text-white px-4 py-2 rounded-md hover:bg-red-600 btnEliminarCategoria">Eliminar</button>
                </td>
            `;
                categoriaTableBody.appendChild(row);

                // Asignar eventos
                row.querySelector(".btnEditarCategoria").addEventListener("click", () => {
                    cargarVistaEditarCategoria(categoria.idCategoria);
                });
                row.querySelector(".btnEliminarCategoria").addEventListener("click", () => {
                    eliminarCategoria(categoria.idCategoria);
                });
            });
        } catch (error) {
            console.error(error);
            mostrarAlerta("error", "Error al cargar las categorías.");
        }
    }

    // Cargar formulario de creación de categoría
    function cargarVistaCrearCategoria() {
        const form = `
            <h2 class="text-lg font-bold mb-4">Crear Categoría</h2>
            <form id="crearCategoriaForm">
                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700">Descripción</label>
                    <input type="text" name="descripcionCategoria" id="descripcionCategoria" class="w-full border-gray-300 rounded-md shadow-sm" required>
                </div>
                <div class="flex justify-end">
                    <button type="submit" class="bg-green-500 text-white px-4 py-2 rounded-md hover:bg-green-600">Guardar</button>
                </div>
            </form>
        `;

        abrirModal(form);

        const formElement = document.getElementById("crearCategoriaForm");
        formElement.addEventListener("submit", async (e) => {
            e.preventDefault();
            const descripcionCategoria = document.getElementById("descripcionCategoria").value;

            try {
                const response = await fetch(`${apiCategoriaBaseUrl}/crearCategoria`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({ descripcionCategoria }),
                });

                if (response.ok) {
                    mostrarAlerta("success", "Categoría creada exitosamente.");
                    modalContainer.classList.add("hidden");
                    cargarCategorias();
                    window.cargarProductos(); // Recargar productos
                } else {
                    const error = await response.json();
                    mostrarAlerta("error", error.mensaje);
                }
            } catch (error) {
                console.error(error);
                mostrarAlerta("error", "Error al crear la categoría.");
            }
        });
    }

    // Cargar formulario de edición de categoría
    async function cargarVistaEditarCategoria(idCategoria) {
        try {
            const response = await fetch(`${apiCategoriaBaseUrl}/obtenerCategoriaId/${idCategoria}`);
            if (!response.ok) throw new Error("Error al obtener la categoría.");

            const categoria = await response.json();

            const form = `
                <h2 class="text-lg font-bold mb-4">Editar Categoría</h2>
                <form id="editarCategoriaForm">
                    <div class="mb-4">
                        <label class="block text-sm font-medium text-gray-700">Descripción</label>
                        <input type="text" name="descripcionCategoria" id="descripcionCategoria" class="w-full border-gray-300 rounded-md shadow-sm" value="${categoria.descripcionCategoria}" required>
                    </div>
                    <div class="flex justify-end">
                        <button type="submit" class="bg-blue-500 text-white px-4 py-2 rounded-md hover:bg-blue-600">Guardar</button>
                    </div>
                </form>
            `;

            abrirModal(form);

            const formElement = document.getElementById("editarCategoriaForm");
            formElement.addEventListener("submit", async (e) => {
                e.preventDefault();
                const descripcionCategoria = document.getElementById("descripcionCategoria").value;

                try {
                    const response = await fetch(`${apiCategoriaBaseUrl}/editarCategoria/${idCategoria}`, {
                        method: "PUT",
                        headers: {
                            "Content-Type": "application/json",
                        },
                        body: JSON.stringify({ descripcionCategoria }),
                    });

                    if (response.ok) {
                        mostrarAlerta("success", "Categoría editada exitosamente.");
                        modalContainer.classList.add("hidden");
                        cargarCategorias();
                        window.cargarProductos(); // Recargar productos
                    } else {
                        mostrarAlerta("error", "Error al editar la categoría.");
                    }
                } catch (error) {
                    console.error(error);
                    mostrarAlerta("error", "Error al editar la categoría.");
                }
            });
        } catch (error) {
            console.error(error);
            mostrarAlerta("error", "Error al cargar los datos de la categoría.");
        }
    }

    // Eliminar categoría
    async function eliminarCategoria(idCategoria) {
        try {
            const response = await fetch(`${apiCategoriaBaseUrl}/eliminarCategoria/${idCategoria}`, {
                method: "DELETE",
            });

            if (response.ok) {
                mostrarAlerta("success", "Categoría eliminada exitosamente.");
                cargarCategorias();
            } else {
                mostrarAlerta("error", "Error al eliminar la categoría.");
            }
        } catch (error) {
            console.error(error);
            mostrarAlerta("error", "Error al eliminar la categoría.");
        }
    }

    // Inicializar eventos
    document.getElementById("btnGestionarCategorias").addEventListener("click", () => {
        cargarVistaGestionarCategorias();
    });

    // Cargar vista principal de categorías
    function cargarVistaGestionarCategorias() {
        const content = `
            <h2 class="text-lg font-bold mb-4">Gestión de Categorías</h2>
            <button id="btnCrearCategoria" class="bg-green-500 text-white px-4 py-2 rounded-md hover:bg-green-600 mb-4">Crear Nueva Categoría</button>
            <table class="w-full border-collapse border border-gray-300">
                <thead>
                    <tr class="bg-gray-100">
                        <th class="border border-gray-300 px-4 py-2 text-left">Descripción</th>
                        <th class="border border-gray-300 px-4 py-2 text-right">Acciones</th>
                    </tr>
                </thead>
                <tbody id="categoriaTableBody">
                    <!-- Categorías dinámicas aquí -->
                </tbody>
            </table>
        `;

        abrirModal(content);

        document.getElementById("btnCrearCategoria").addEventListener("click", cargarVistaCrearCategoria);

        cargarCategorias();
    }
});
