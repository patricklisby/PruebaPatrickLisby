const apiBaseUrl = '/api/Producto';
const productosContainer = document.getElementById("productosContainer");
const modalContainer = document.getElementById("modalContainer");
const modalContent = document.getElementById("modalContent");

// Cargar productos
async function cargarProductos() {
    try {
        const response = await fetch(`${apiBaseUrl}/obtenerProductos`);
        if (!response.ok) {
            throw new Error("Error al cargar los productos.");
        }

        const productos = await response.json();
        productosContainer.innerHTML = ''; // Limpiar contenedor

        if (productos.length === 0) {
            productosContainer.innerHTML = '<p class="text-gray-500">No hay productos disponibles.</p>';
            return;
        }

        productos.forEach((producto) => {
            const productoCard = crearProductoCard(producto);
            productosContainer.appendChild(productoCard);
        });
    } catch (error) {
        console.error(error);
    }
}

// Crear tarjeta de producto
function crearProductoCard(producto) {
    const card = document.createElement("div");
    card.className = "bg-white rounded-lg shadow-md p-4";

    const imagenUrl = producto.ImagenUrl || "/imagenes/default.png";
    card.innerHTML = `
        <div class="w-full h-48 bg-gray-200 rounded-t-lg overflow-hidden">
            <img src="${imagenUrl}" alt="${producto.descripcionProducto}" class="object-cover w-full h-full">
        </div>
        <div class="mt-4">
            <h2 class="text-lg font-bold">${producto.descripcionProducto}</h2>
            <p class="text-sm text-gray-600">${producto.detallesProducto}</p>
            <p class="text-gray-800 font-bold">Precio: ₡${producto.precioProducto.toFixed(2)}</p>
        </div>
        <div class="mt-4 flex justify-between">
            <button data-id="${producto.idProducto}" class="bg-yellow-500 text-white px-4 py-2 rounded-md hover:bg-yellow-600" onclick="cargarVistaEditar(${producto.idProducto})">
                Editar
            </button>
        </div>
    `;

    return card;
}

// Abrir modal
function abrirModal(content) {
    modalContent.innerHTML = content;
    modalContainer.classList.remove("hidden");
}

// Cerrar modal
function cerrarModal() {
    modalContainer.classList.add("hidden");
}

// Cargar vista de crear producto
async function cargarVistaCrear() {
    const form = `
        <h2 class="text-lg font-bold mb-4">Crear Producto</h2>
        <form id="crearProductoForm">
            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700">Descripción</label>
                <input type="text" id="descripcionProducto" class="w-full border-gray-300 rounded-md shadow-sm">
            </div>
            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700">Detalles</label>
                <textarea id="detallesProducto" class="w-full border-gray-300 rounded-md shadow-sm"></textarea>
            </div>
            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700">Precio</label>
                <input type="number" id="precioProducto" class="w-full border-gray-300 rounded-md shadow-sm">
            </div>
            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700">Cantidad</label>
                <input type="number" id="cantidadProducto" class="w-full border-gray-300 rounded-md shadow-sm">
            </div>
            <button type="submit" class="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700">Guardar</button>
        </form>
    `;

    abrirModal(form);

    document.getElementById("crearProductoForm").addEventListener("submit", async (e) => {
        e.preventDefault();

        const producto = {
            descripcionProducto: document.getElementById("descripcionProducto").value,
            detallesProducto: document.getElementById("detallesProducto").value,
            precioProducto: parseFloat(document.getElementById("precioProducto").value),
            cantidadProducto: parseInt(document.getElementById("cantidadProducto").value),
        };

        try {
            const response = await fetch(`${apiBaseUrl}/crearProducto`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(producto),
            });

            if (response.ok) {
                alert("Producto creado exitosamente.");
                cerrarModal();
                cargarProductos();
            } else {
                throw new Error("Error al crear el producto.");
            }
        } catch (error) {
            console.error(error);
        }
    });
}

// Cargar vista de editar producto
async function cargarVistaEditar(idProducto) {
    try {
        const response = await fetch(`${apiBaseUrl}/obtenerProductoId/${idProducto}`);
        if (!response.ok) {
            throw new Error("Error al obtener el producto.");
        }

        const producto = await response.json();

        const form = `
            <h2 class="text-lg font-bold mb-4">Editar Producto</h2>
            <form id="editarProductoForm">
                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700">Descripción</label>
                    <input type="text" id="descripcionProducto" class="w-full border-gray-300 rounded-md shadow-sm" value="${producto.descripcionProducto}">
                </div>
                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700">Detalles</label>
                    <textarea id="detallesProducto" class="w-full border-gray-300 rounded-md shadow-sm">${producto.detallesProducto}</textarea>
                </div>
                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700">Precio</label>
                    <input type="number" id="precioProducto" class="w-full border-gray-300 rounded-md shadow-sm" value="${producto.precioProducto}">
                </div>
                <div class="mb-4">
                    <label class="block text-sm font-medium text-gray-700">Cantidad</label>
                    <input type="number" id="cantidadProducto" class="w-full border-gray-300 rounded-md shadow-sm" value="${producto.cantidadProducto}">
                </div>
                <button type="submit" class="bg-indigo-600 text-white px-4 py-2 rounded-md hover:bg-indigo-700">Guardar</button>
            </form>
        `;

        abrirModal(form);

        document.getElementById("editarProductoForm").addEventListener("submit", async (e) => {
            e.preventDefault();

            const productoEditado = {
                descripcionProducto: document.getElementById("descripcionProducto").value,
                detallesProducto: document.getElementById("detallesProducto").value,
                precioProducto: parseFloat(document.getElementById("precioProducto").value),
                cantidadProducto: parseInt(document.getElementById("cantidadProducto").value),
            };

            try {
                const response = await fetch(`${apiBaseUrl}/editarProducto/${idProducto}`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(productoEditado),
                });

                if (response.ok) {
                    alert("Producto editado exitosamente.");
                    cerrarModal();
                    cargarProductos();
                } else {
                    throw new Error("Error al editar el producto.");
                }
            } catch (error) {
                console.error(error);
            }
        });
    } catch (error) {
        console.error(error);
    }
}
