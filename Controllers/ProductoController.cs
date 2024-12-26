using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.IO;
using PruebaPatrickLisby.Models;

namespace PruebaPatrickLisby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : Controller
    {
        private readonly string _connectionString;
        /// <summary>
        /// Constructor que inicializa la conexión a la base de datos.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación que contiene la cadena de conexión.</param>
        public ProductoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Obtiene una lista de todos los productos con su información asociada.
        /// </summary>
        /// <returns>Una lista de productos o un mensaje de error si falla la operación.</returns>
        [HttpGet("obtenerProductos")]
        public IActionResult ObtenerProductos()
        {
            try
            {
                List<Producto> productos = new List<Producto>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    string query = @"
            SELECT p.idProducto, 
                   p.descripcionProducto, 
                   p.detallesProducto, 
                   p.precioProducto, 
                   p.cantidadProducto, 
                   p.fechaPublicacion, 
                   p.idCategoria, 
                   p.estado, 
                   p.idCedulaUsuarioRegistra, 
                   p.idImagen,
                   i.urlImagen,
                   c.descripcionCategoria as descripcionCategoria
            FROM Productos p
            LEFT JOIN Imagenes i ON p.idImagen = i.idImagen
            LEFT JOIN Categorias c ON c.idCategoria = p.idCategoria";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader["urlImagen"].ToString());
                            productos.Add(new Producto
                            {
                                idProducto = (int)reader["idProducto"],
                                descripcionProducto = reader["descripcionProducto"].ToString(),
                                detallesProducto = reader["detallesProducto"].ToString(),
                                precioProducto = (decimal)reader["precioProducto"],
                                cantidadProducto = (int)reader["cantidadProducto"],
                                fechaPublicacion = (DateTime)reader["fechaPublicacion"],
                                idCategoria = (int)reader["idCategoria"],
                                estado = (int)reader["estado"],
                                idCedulaUsuarioRegistra = (int)reader["idCedulaUsuarioRegistra"],
                                IdImagen = (int)reader["idImagen"],
                                ImagenUrl = reader["urlImagen"].ToString(),
                                descripcionCategoria = reader["descripcionCategoria"].ToString()
                            });
                        }
                    }
                }
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener productos: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene los detalles de un producto específico por su ID.
        /// </summary>
        /// <param name="idProducto">ID del producto a buscar.</param>
        /// <returns>El producto encontrado o un mensaje de error si no se encuentra.</returns>
        [HttpGet("obtenerProductoId/{idProducto}")]
        public IActionResult ObtenerProductoId(int idProducto)
        {
            try
            {
                Producto producto = null;
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE idProducto = @idProducto", conn);
                    cmd.Parameters.AddWithValue("@idProducto", idProducto);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        producto = new Producto
                        {
                            idProducto = (int)reader["IdProducto"],
                            descripcionProducto = reader["DescripcionProducto"].ToString(),
                            detallesProducto = reader["DetallesProducto"].ToString(),
                            precioProducto = (decimal)reader["PrecioProducto"],
                            cantidadProducto = (int)reader["CantidadProducto"],
                            fechaPublicacion = (DateTime)reader["FechaPublicacion"],
                            idCategoria = (int)reader["IdCategoria"],
                            estado = (int)reader["estado"],
                            idCedulaUsuarioRegistra = (int)reader["IdCedulaUsuarioRegistra"]
                        };
                    }
                    else
                    {
                        return NotFound(new { message = "Producto no encontrado con el ID proporcionado." });
                    }
                }
                return producto == null ? NotFound(new { message = "Producto no encontrado." }) : Ok(producto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener producto: {ex.Message}");
            }
        }

        /// <summary>
        /// Crea un nuevo producto en la base de datos y opcionalmente guarda una imagen asociada.
        /// </summary>
        /// <param name="producto">Modelo del producto a crear.</param>
        /// <param name="imagen">Archivo de imagen asociado al producto (opcional).</param>
        /// <returns>Mensaje de éxito o error si ocurre algún problema.</returns>
        [HttpPost("crearProducto")]
        public IActionResult CrearProducto([FromForm] Producto producto, [FromForm] IFormFile? imagen)
        {
            try
            {
                // Depurar sesión
                Console.WriteLine($"SessionUserId: {HttpContext.Session.GetString("SessionUserId")}");
                Console.WriteLine($"SessionUserName: {HttpContext.Session.GetString("SessionUserName")}");

                // Recuperar el ID del usuario desde la sesión
                int usuarioId = int.Parse(HttpContext.Session.GetString("SessionUserId"));
                Console.WriteLine($"ID del usuario desde la sesión: {usuarioId}");

                producto.idCedulaUsuarioRegistra = usuarioId;

                int? nuevoIdImagen = null;

                var permitidos = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(imagen.FileName).ToLower();

                if (!permitidos.Contains(extension))
                {
                    return BadRequest(new { mensaje = "Tipo de archivo no permitido. Solo se aceptan imágenes." });
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Gestionar la imagen si se proporciona
                    if (imagen != null)
                    {
                        try
                        {
                            // Crear carpeta si no existe
                            string carpetaImagenes = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
                            if (!Directory.Exists(carpetaImagenes))
                            {
                                Directory.CreateDirectory(carpetaImagenes);
                            }

                            // Guardar la imagen
                            string nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagen.FileName);
                            string rutaFisica = Path.Combine(carpetaImagenes, nombreArchivo);

                            using (var stream = new FileStream(rutaFisica, FileMode.Create))
                            {
                                imagen.CopyTo(stream);
                            }

                            // Generar la URL de la imagen para la base de datos
                            string urlImagen = $"/imagenes/{nombreArchivo}".Replace("\\", "/");

                            // Insertar la imagen en la tabla Imagenes y obtener el ID generado
                            string insertarImagenSql = @"
                        INSERT INTO Imagenes (urlImagen)
                        OUTPUT INSERTED.idImagen
                        VALUES (@urlImagen)";

                            using (var cmdImagen = new SqlCommand(insertarImagenSql, conn))
                            {
                                cmdImagen.Parameters.AddWithValue("@urlImagen", urlImagen);
                                nuevoIdImagen = (int)cmdImagen.ExecuteScalar();
                            }
                        }
                        catch (Exception imgEx)
                        {
                            return StatusCode(500, new { mensaje = $"Error al guardar la imagen: {imgEx.Message}" });
                        }
                    }

                    // Insertar el producto en la tabla Productos
                    string insertarProductoSql = @"
                INSERT INTO Productos (
                    descripcionProducto,
                    detallesProducto,
                    precioProducto,
                    cantidadProducto,
                    fechaPublicacion,
                    idCategoria,
                    idCedulaUsuarioRegistra,
                    estado,
                    idImagen
                )
                OUTPUT INSERTED.idProducto
                VALUES (
                    @descripcionProducto,
                    @detallesProducto,
                    @precioProducto,
                    @cantidadProducto,
                    @fechaPublicacion,
                    @idCategoria,
                    @idCedulaUsuarioRegistra,
                    @estado,
                    @idImagen
                )";

                    using (var cmdProducto = new SqlCommand(insertarProductoSql, conn))
                    {
                        cmdProducto.Parameters.AddWithValue("@descripcionProducto", producto.descripcionProducto);
                        cmdProducto.Parameters.AddWithValue("@detallesProducto", producto.detallesProducto ?? (object)DBNull.Value);
                        cmdProducto.Parameters.AddWithValue("@precioProducto", producto.precioProducto);
                        cmdProducto.Parameters.AddWithValue("@cantidadProducto", producto.cantidadProducto);
                        cmdProducto.Parameters.AddWithValue("@fechaPublicacion", DateTime.Now);
                        cmdProducto.Parameters.AddWithValue("@idCategoria", producto.idCategoria);
                        cmdProducto.Parameters.AddWithValue("@idCedulaUsuarioRegistra", producto.idCedulaUsuarioRegistra);
                        cmdProducto.Parameters.AddWithValue("@estado", producto.estado);
                        cmdProducto.Parameters.AddWithValue("@idImagen", nuevoIdImagen ?? (object)DBNull.Value);

                        int idProducto = (int)cmdProducto.ExecuteScalar();

                        return Ok(new { mensaje = "Producto creado exitosamente", idProducto, idImagen = nuevoIdImagen });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al crear producto: {ex.Message}" });
            }
        }
        /// <summary>
        /// Edita un  producto en la base de datos y opcionalmente guarda o modifica una imagen asociada.
        /// </summary>
        /// <param name="idProducto">Id del producto a editar.</param>
        /// <param name="producto">Modelo del producto a editar.</param>
        /// <param name="imagen">Archivo de imagen asociado al producto (opcional).</param>
        /// <returns>Mensaje de éxito o error si ocurre algún problema.</returns>
        [HttpPut("editarProducto/{idProducto}")]
        public IActionResult EditarProducto(int idProducto, [FromForm] Producto producto, [FromForm] IFormFile? imagen)
        {
            try
            {
                // Recuperar el ID del usuario desde la sesión
                int usuarioId = int.Parse(HttpContext.Session.GetString("SessionUserId"));
                Console.WriteLine($"ID del usuario desde la sesión: {usuarioId}");

                producto.idCedulaUsuarioRegistra = usuarioId;

                int? nuevoIdImagen = null;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Obtener información actual del producto
                    string obtenerProductoSql = @"
            SELECT idImagen
            FROM Productos
            WHERE idProducto = @idProducto";

                    int? idImagenActual = null;

                    using (var obtenerCmd = new SqlCommand(obtenerProductoSql, conn))
                    {
                        obtenerCmd.Parameters.AddWithValue("@idProducto", idProducto);

                        using (var reader = obtenerCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idImagenActual = reader["idImagen"] as int?;
                            }
                            else
                            {
                                return NotFound(new { mensaje = "Producto no encontrado." });
                            }
                        }
                    }

                    // Gestionar la nueva imagen (si fue enviada)
                    if (imagen != null)
                    {
                        try
                        {
                            var permitidos = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                            var extension = Path.GetExtension(imagen.FileName).ToLower();

                            if (!permitidos.Contains(extension))
                            {
                                return BadRequest(new { mensaje = "Tipo de archivo no permitido. Solo se aceptan imágenes." });
                            }

                            // Crear carpeta si no existe
                            string carpetaImagenes = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imagenes");
                            if (!Directory.Exists(carpetaImagenes))
                            {
                                Directory.CreateDirectory(carpetaImagenes);
                            }

                            // Guardar la nueva imagen
                            string nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagen.FileName);
                            string rutaFisica = Path.Combine(carpetaImagenes, nombreArchivo);

                            using (var stream = new FileStream(rutaFisica, FileMode.Create))
                            {
                                imagen.CopyTo(stream);
                            }

                            // Generar la URL de la nueva imagen
                            string urlImagen = $"/imagenes/{nombreArchivo}".Replace("\\", "/");

                            // Insertar la nueva imagen en la tabla Imagenes
                            string insertarImagenSql = @"
                    INSERT INTO Imagenes (urlImagen)
                    OUTPUT INSERTED.idImagen
                    VALUES (@urlImagen)";

                            using (var insertarCmd = new SqlCommand(insertarImagenSql, conn))
                            {
                                insertarCmd.Parameters.AddWithValue("@urlImagen", urlImagen);
                                nuevoIdImagen = (int)insertarCmd.ExecuteScalar();
                            }

                            // Eliminar la imagen anterior si existe
                            if (idImagenActual.HasValue)
                            {
                                string obtenerUrlImagenSql = "SELECT urlImagen FROM Imagenes WHERE idImagen = @idImagen";
                                string? urlImagenAnterior = null;

                                using (var obtenerUrlCmd = new SqlCommand(obtenerUrlImagenSql, conn))
                                {
                                    obtenerUrlCmd.Parameters.AddWithValue("@idImagen", idImagenActual.Value);
                                    urlImagenAnterior = obtenerUrlCmd.ExecuteScalar()?.ToString();
                                }

                                if (urlImagenAnterior != null)
                                {
                                    string rutaFisicaAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", urlImagenAnterior.TrimStart('/'));
                                    if (System.IO.File.Exists(rutaFisicaAnterior))
                                    {
                                        System.IO.File.Delete(rutaFisicaAnterior);
                                    }

                                    // Eliminar el registro de la imagen anterior en la base de datos
                                    string eliminarImagenSql = "DELETE FROM Imagenes WHERE idImagen = @idImagen";
                                    using (var eliminarCmd = new SqlCommand(eliminarImagenSql, conn))
                                    {
                                        eliminarCmd.Parameters.AddWithValue("@idImagen", idImagenActual.Value);
                                        eliminarCmd.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                        catch (Exception imgEx)
                        {
                            return StatusCode(500, new { mensaje = $"Error al gestionar la imagen: {imgEx.Message}" });
                        }
                    }
                    else
                    {
                        // Mantener la imagen actual si no se proporciona una nueva
                        nuevoIdImagen = idImagenActual;
                    }

                    // Actualizar el producto
                    string actualizarProductoSql = @"
            UPDATE Productos
            SET descripcionProducto = @descripcionProducto,
                detallesProducto = @detallesProducto,
                precioProducto = @precioProducto,
                cantidadProducto = @cantidadProducto,
                estado = @estado,
                idCategoria = @idCategoria,
                idCedulaUsuarioRegistra = @idCedulaUsuarioRegistra,
                idImagen = @idImagen
            WHERE idProducto = @idProducto";

                    using (var cmdProducto = new SqlCommand(actualizarProductoSql, conn))
                    {
                        cmdProducto.Parameters.AddWithValue("@idProducto", idProducto);
                        cmdProducto.Parameters.AddWithValue("@descripcionProducto", producto.descripcionProducto);
                        cmdProducto.Parameters.AddWithValue("@detallesProducto", producto.detallesProducto ?? (object)DBNull.Value);
                        cmdProducto.Parameters.AddWithValue("@precioProducto", producto.precioProducto);
                        cmdProducto.Parameters.AddWithValue("@cantidadProducto", producto.cantidadProducto);
                        cmdProducto.Parameters.AddWithValue("@estado", producto.estado);
                        cmdProducto.Parameters.AddWithValue("@idCategoria", producto.idCategoria);
                        cmdProducto.Parameters.AddWithValue("@idCedulaUsuarioRegistra", producto.idCedulaUsuarioRegistra);
                        cmdProducto.Parameters.AddWithValue("@idImagen", nuevoIdImagen ?? (object)DBNull.Value);

                        int rowsAffected = cmdProducto.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            return NotFound(new { mensaje = "Producto no encontrado." });
                        }

                        return Ok(new { mensaje = "Producto actualizado exitosamente", nuevaImagenId = nuevoIdImagen });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al editar producto: {ex.Message}" });
            }
        }


        /// <summary>
        /// Realiza una eliminación lógica de un producto, cambiando su estado a 0.
        /// </summary>
        /// <param name="idProducto">ID del producto a eliminar.</param>
        /// <returns>Mensaje indicando el éxito o error de la operación.</returns>
        [HttpPost("eliminarProducto/{idProducto}")]
        public IActionResult EliminarProducto(int idProducto)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var cmd = new SqlCommand(
                        "UPDATE Productos SET estado = 0 WHERE idProducto = @idProducto",
                        connection
                    );

                    cmd.Parameters.AddWithValue("@idProducto", idProducto);

                    var rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Retornamos 204 (No Content) indicando éxito sin contenido adicional
                        return NoContent();
                    }
                    else
                    {
                        return NotFound(new { mensaje = "Producto no encontrado." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }
    }


}
