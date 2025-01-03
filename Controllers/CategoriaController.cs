﻿using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using PruebaPatrickLisby.Models;
using System.Collections.Generic;

namespace PruebaPatrickLisby.Controllers
{
    /// <summary>
    /// Controlador que maneja las operaciones relacionadas con las categorías.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : Controller
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructor que inicializa el controlador con la configuración de conexión a la base de datos.
        /// </summary>
        /// <param name="configuration">Proporciona acceso a las configuraciones de la aplicación.</param>
        public CategoriaController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Obtiene todas las categorías de la base de datos.
        /// </summary>
        /// <returns>
        /// - `200 OK`: Devuelve una lista de todas las categorías.
        /// - `500 Internal Server Error`: Si ocurre un error interno.
        /// </returns>
        [HttpGet("obtenerCategorias")]
        public IActionResult ObtenerCategorias()
        {
            try
            {
                List<Categoria> categorias = new List<Categoria>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Categorias", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        categorias.Add(new Categoria
                        {
                            idCategoria = (int)reader["idCategoria"],
                            descripcionCategoria = reader["descripcionCategoria"].ToString(),
                        });
                    }
                }
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener productos: {ex.Message}");
            }
        }
        /// <summary>
        /// Obtiene una categoría específica por su ID.
        /// </summary>
        /// <param name="idCategoria">ID de la categoría a buscar.</param>
        /// <returns>
        /// - `200 OK`: Devuelve los detalles de la categoría encontrada.
        /// - `404 Not Found`: Si la categoría no existe.
        /// - `500 Internal Server Error`: Si ocurre un error interno.
        /// </returns>
        [HttpGet("obtenerCategoriaId/{idCategoria}")]
        public IActionResult ObtenerCategoriaId(int idCategoria)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var cmd = new SqlCommand("SELECT * FROM Categorias WHERE idCategoria = @idCategoria", connection);
                    cmd.Parameters.AddWithValue("@idCategoria", idCategoria);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var categoria = new
                            {
                                idCategoria = reader["idCategoria"],
                                descripcionCategoria = reader["descripcionCategoria"]
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

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <param name="categoria">Objeto con la descripción de la categoría.</param>
        /// <returns>
        /// - `201 Created`: Si la categoría se crea exitosamente.
        /// - `500 Internal Server Error`: Si ocurre un error interno.
        /// </returns>
        [HttpPost("crearCategoria")]
        public IActionResult CrearCategoria([FromBody] Categoria categoria)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var cmd = new SqlCommand("INSERT INTO Categorias (descripcionCategoria) VALUES (@descripcionCategoria); SELECT SCOPE_IDENTITY();", connection);
                    cmd.Parameters.AddWithValue("@descripcionCategoria", (string)categoria.descripcionCategoria);

                    var idNuevo = Convert.ToInt32(cmd.ExecuteScalar());

                    return CreatedAtAction(nameof(ObtenerCategorias), new { idCategoria = idNuevo }, new { idCategoria = idNuevo, descripcionCategoria = categoria.descripcionCategoria });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza una categoría existente.
        /// </summary>
        /// <param name="idCategoria">ID de la categoría a actualizar.</param>
        /// <param name="categoria">Objeto con los datos actualizados de la categoría.</param>
        /// <returns>
        /// - `204 No Content`: Si la categoría se actualiza exitosamente.
        /// - `404 Not Found`: Si la categoría no existe.
        /// - `500 Internal Server Error`: Si ocurre un error interno.
        /// </returns>
        [HttpPut("editarCategoria/{idCategoria}")]
        public IActionResult UpdateCategoria(int idCategoria, [FromBody] Categoria categoria)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("UPDATE Categorias SET descripcionCategoria = @descripcionCategoria WHERE idCategoria = @idCategoria", connection);
                    command.Parameters.AddWithValue("@descripcionCategoria", (string)categoria.descripcionCategoria);
                    command.Parameters.AddWithValue("@idCategoria", idCategoria);

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

        /// <summary>
        /// Elimina una categoría existente.
        /// </summary>
        /// <param name="idCategoria">ID de la categoría a eliminar.</param>
        /// <returns>
        /// - `204 No Content`: Si la categoría se elimina exitosamente.
        /// - `404 Not Found`: Si la categoría no existe.
        /// - `500 Internal Server Error`: Si ocurre un error interno.
        /// </returns>
        [HttpDelete("eliminarCategoria/{idCategoria}")]
        public IActionResult EliminarCategoria(int idCategoria)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var command = new SqlCommand("DELETE FROM Categorias WHERE idCategoria = @idCategoria", connection);
                    command.Parameters.AddWithValue("@idCategoria", idCategoria);

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
