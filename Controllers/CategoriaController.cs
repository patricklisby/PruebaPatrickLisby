using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PruebaPatrickLisby.Models;

namespace PruebaPatrickLisby.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : Controller
    {
        private readonly string _connectionString;

        public CategoriasController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("getAllCategorias")]
        public IActionResult GetProductos()
        {
            try
            {
                List<Producto> productos = new List<Producto>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Categorias", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        productos.Add(new Producto
                        {
                            idProducto = (int)reader["idCategoria"],
                            descripcionProducto = reader["DescripcionCategoria"].ToString(),
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
        [HttpGet("{id}")]
        public IActionResult GetCategoria(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("SELECT * FROM Categorias WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var categoria = new
                            {
                                Id = reader["Id"],
                                Nombre = reader["Nombre"]
                            };
                            return Ok(categoria);
                        }
                        else
                        {
                            return NotFound(new { mensaje = "Categoría no encontrada." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }

        [HttpPost("crearCategoria")]
        public IActionResult CreateCategoria([FromBody] dynamic categoria)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("INSERT INTO Categorias (DescripcionCategoria) VALUES (@DescripcionCategoria); SELECT SCOPE_IDENTITY();", connection);
                    command.Parameters.AddWithValue("@DescripcionCategoria", (string)categoria.DescripcionCategoria);

                    var idNuevo = Convert.ToInt32(command.ExecuteScalar());

                    return CreatedAtAction(nameof(GetCategoria), new { idCategoria = idNuevo }, new { idCategoria = idNuevo, DescripcionCategoria = categoria.DescripcionCategoria });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategoria(int id, [FromBody] dynamic categoria)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("UPDATE Categorias SET DescripcionCategoria = @DescripcionCategoria WHERE idCategoria = @idCategoria", connection);
                    command.Parameters.AddWithValue("@DescripcionCategoria", (string)categoria.DescripcionCategoria);
                    command.Parameters.AddWithValue("@Id", id);

                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound(new { mensaje = "Categoría no encontrada." });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategoria(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("DELETE FROM Categorias WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Id", id);

                    var rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return NoContent();
                    }
                    else
                    {
                        return NotFound(new { mensaje = "Categoría no encontrada." });
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
