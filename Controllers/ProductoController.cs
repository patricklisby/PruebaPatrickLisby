using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using PruebaPatrickLisby.Models;
using System.Collections.Generic;

namespace PruebaPatrickLisby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : Controller
    {
        private readonly string _connectionString;

        public ProductoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        [HttpGet("obtenerProductos")]
        public IActionResult ObtenerProductos()
        {
            try
            {
                List<Producto> productos = new List<Producto>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Productos", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
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
                            idCedulaUsuarioRegistra = (int)reader["idCedulaUsuarioRegistra"]
                        });
                    }
                }
                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener productos: {ex.Message}");
            }
        }

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
                    else {
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

        [HttpPost("crearProducto")]
        public IActionResult CrearProducto([FromBody] Producto producto)
        {
            producto.fechaPublicacion = DateTime.Now;//Tomar la fecha actual

            int idCedulaUsuarioRegistra = 1;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Productos (
                    descripcionProducto, 
                    detallesProducto, 
                    precioProducto, 
                    cantidadProducto, 
                    fechaPublicacion, 
                    idCategoria, 
                    idCedulaUsuarioRegistra,
                    estado
                    )
                    VALUES (
                    @descripcionProducto, 
                    @detallesProducto, 
                    @precioProducto, 
                    @cantidadProducto, 
                    @fechaPublicacion, 
                    @idCategoria, 
                    @idCedulaUsuarioRegistra,
                    @estado
                    )",
                    conn);

                cmd.Parameters.AddWithValue("@descripcionProducto", producto.descripcionProducto);
                cmd.Parameters.AddWithValue("@detallesProducto", producto.detallesProducto);
                cmd.Parameters.AddWithValue("@precioProducto", producto.precioProducto);
                cmd.Parameters.AddWithValue("@cantidadProducto", producto.cantidadProducto);
                cmd.Parameters.AddWithValue("@fechaPublicacion", producto.fechaPublicacion);
                cmd.Parameters.AddWithValue("@idCategoria", producto.idCategoria);
                cmd.Parameters.AddWithValue("@idCedulaUsuarioRegistra", idCedulaUsuarioRegistra);
                cmd.Parameters.AddWithValue("@estado", producto.estado);
                cmd.ExecuteNonQuery();
            }
            return Ok();
        }

        [HttpPut("editarProducto/{idProducto}")]
        public IActionResult EditarProducto(int idProducto, [FromBody] Producto producto)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Ajusta los nombres de columna exactamente como están en tu tabla
                    // Corrige "descriptcionProducto" -> "descripcionProducto"
                    // Ajusta la sentencia para que coincidan los nombres de parámetros
                    string sql = @"
                UPDATE Productos
                   SET descripcionProducto       = @descripcionProducto,
                       detallesProducto         = @detallesProducto,
                       precioProducto           = @precioProducto,
                       cantidadProducto         = @cantidadProducto,
                       estado                   = @estado,
                       fechaPublicacion         = @fechaPublicacion,
                       idCategoria              = @idCategoria,
                       idCedulaUsuarioRegistra  = @idCedulaUsuarioRegistra
                      
                 WHERE idProducto = @idProducto
            ";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        // Usamos el id de la ruta, no el del body, para evitar desajustes
                        cmd.Parameters.AddWithValue("@idProducto", idProducto);

                        // Mapea el resto de parámetros desde el body dynamic
                        // Asegúrate de que tu JSON contenga estas propiedades
                        cmd.Parameters.AddWithValue("@descripcionProducto", (string)producto.descripcionProducto);
                        cmd.Parameters.AddWithValue("@detallesProducto", (string)producto.detallesProducto);
                        cmd.Parameters.AddWithValue("@precioProducto", (decimal)producto.precioProducto);
                        cmd.Parameters.AddWithValue("@cantidadProducto", (int)producto.cantidadProducto);
                        cmd.Parameters.AddWithValue("@estado", producto.estado);

                        // y en la BD se llama "fechaPublicacion":
                        cmd.Parameters.AddWithValue("@fechaPublicacion", (DateTime)producto.fechaPublicacion);

                        cmd.Parameters.AddWithValue("@idCategoria", (int)producto.idCategoria);
                        cmd.Parameters.AddWithValue("@idCedulaUsuarioRegistra", (int)producto.idCedulaUsuarioRegistra);

                        // Ejecutamos la sentencia
                        var rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // 204 No Content es un estado común para PUT exitoso
                            return StatusCode(204, new { mensaje = "Actualización éxitosa" });
                        }
                        else
                        {
                            return NotFound(new { mensaje = "Producto no encontrado." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }


        [HttpPost("eliminarProducto/{idProducto}")]
        public IActionResult EliminarProducto(int idProducto)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Realizamos la "baja lógica" en lugar de un DELETE
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
