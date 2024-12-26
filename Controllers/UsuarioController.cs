using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Extensions.Configuration;
using PruebaPatrickLisby.Models;

namespace PruebaPatrickLisby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : Controller
    {
        private readonly string _connectionString;

        public UsuarioController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("obtenerUsuarios")]
        public IActionResult ObtenerUsuarios()
        {
            try
            {
                List<Usuario> usuarios = new List<Usuario>();
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Usuarios", conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        usuarios.Add(new Usuario
                        {
                            idCedulaUsuario = (int)reader["idCedulaUsuario"],
                            nombreUsuario = reader["nombreUsuario"].ToString(),
                            apellidoUsuario = reader["apellidoUsuario"].ToString(),
                            correoElectronicoUsuario = reader["correoElectronicoUsuario"].ToString(),
                            contrasenaUsuario = reader["contrasenaUsuario"].ToString(),
                            estado = (int)reader["estado"],
                            idPermiso = (int)reader["idPermiso"]
                        });
                    }
                }
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener usuarios: {ex.Message}");
            }
        }

        [HttpGet("obtenerUsuarioId/{idCedulaUsuario}")]
        public IActionResult ObtenerUsuarioId(int idCedulaUsuario)
        {
            try
            {
                Usuario usuario = null;
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Usuarios WHERE idCedulaUsuario = @idCedulaUsuario", conn);
                    cmd.Parameters.AddWithValue("@idCedulaUsuario", idCedulaUsuario);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            idCedulaUsuario = (int)reader["idCedulaUsuario"],
                            nombreUsuario = reader["nombreUsuario"].ToString(),
                            apellidoUsuario = reader["apellidoUsuario"].ToString(),
                            correoElectronicoUsuario = reader["correoElectronicoUsuario"].ToString(),
                            contrasenaUsuario = reader["contrasenaUsuario"].ToString(),
                            estado = (int)reader["estado"],
                            idPermiso = (int)reader["idPermiso"]
                        };
                    }
                    else
                    {
                        return NotFound(new { message = "Usuario no encontrado con el ID proporcionado." });
                    }
                }
                return usuario == null ? NotFound(new { message = "Usuario no encontrado." }) : Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener usuario: {ex.Message}");
            }
        }

        [HttpPost("crearUsuario")]
        public IActionResult CrearUsuario([FromForm] Usuario usuario, [FromForm] IFormFile? imagen)
        {
            try
            {
                int? nuevoIdImagen = null;

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

                    // Encriptar la contraseña
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuario.contrasenaUsuario);

                    // Insertar el usuario en la tabla Usuarios
                    string insertarUsuarioSql = @"
                INSERT INTO Usuarios (
                    idCedulaUsuario, 
                    nombreUsuario, 
                    apellidoUsuario, 
                    correoElectronicoUsuario, 
                    contrasenaUsuario,
                    estado,
                    idPermiso,
                    idImagen
                )
                VALUES (
                    @idCedulaUsuario, 
                    @nombreUsuario, 
                    @apellidoUsuario, 
                    @correoElectronicoUsuario, 
                    @contrasenaUsuario, 
                    @estado,
                    @idPermiso,
                    @idImagen
                )";

                    using (var cmdUsuario = new SqlCommand(insertarUsuarioSql, conn))
                    {
                        cmdUsuario.Parameters.AddWithValue("@idCedulaUsuario", usuario.idCedulaUsuario);
                        cmdUsuario.Parameters.AddWithValue("@nombreUsuario", usuario.nombreUsuario ?? (object)DBNull.Value);
                        cmdUsuario.Parameters.AddWithValue("@apellidoUsuario", usuario.apellidoUsuario ?? (object)DBNull.Value);
                        cmdUsuario.Parameters.AddWithValue("@correoElectronicoUsuario", usuario.correoElectronicoUsuario ?? (object)DBNull.Value);
                        cmdUsuario.Parameters.AddWithValue("@contrasenaUsuario", hashedPassword);
                        cmdUsuario.Parameters.AddWithValue("@estado", 1); // Estado por defecto
                        cmdUsuario.Parameters.AddWithValue("@idPermiso", usuario.idPermiso);
                        cmdUsuario.Parameters.AddWithValue("@idImagen", nuevoIdImagen ?? (object)DBNull.Value);

                        cmdUsuario.ExecuteNonQuery();
                    }
                }

                return Ok(new { mensaje = "Usuario creado exitosamente", idImagen = nuevoIdImagen });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al crear usuario: {ex.Message}" });
            }
        }


        [HttpPut("editarUsuario/{idCedulaUsuario}")]
        public IActionResult EditarUsuario(int idCedulaUsuario, [FromForm] Usuario usuario, [FromForm] IFormFile? imagen)
        {
            try
            {
                int? nuevoIdImagen = null;

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Obtener información actual del usuario
                    string obtenerUsuarioSql = @"
                SELECT idImagen, contrasenaUsuario 
                FROM Usuarios 
                WHERE idCedulaUsuario = @idCedulaUsuario";

                    int? idImagenActual = null;
                    string contrasenaActual;

                    using (var obtenerCmd = new SqlCommand(obtenerUsuarioSql, conn))
                    {
                        obtenerCmd.Parameters.AddWithValue("@idCedulaUsuario", idCedulaUsuario);

                        using (var reader = obtenerCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                idImagenActual = reader["idImagen"] as int?;
                                contrasenaActual = reader["contrasenaUsuario"].ToString();
                            }
                            else
                            {
                                return NotFound(new { mensaje = "Usuario no encontrado." });
                            }
                        }
                    }

                    // Gestionar la nueva imagen (si fue enviada)
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

                    // Verificar si la contraseña se actualizó
                    string contrasenaHashFinal = !string.IsNullOrWhiteSpace(usuario.contrasenaUsuario)
                        ? BCrypt.Net.BCrypt.HashPassword(usuario.contrasenaUsuario)
                        : contrasenaActual;

                    // Actualizar el usuario
                    string sql = @"
                UPDATE Usuarios
                SET nombreUsuario = @nombreUsuario,
                    apellidoUsuario = @apellidoUsuario,
                    correoElectronicoUsuario = @correoElectronicoUsuario,
                    contrasenaUsuario = @contrasenaUsuario,
                    estado = @estado,
                    idPermiso = @idPermiso,
                    idImagen = @idImagen
                WHERE idCedulaUsuario = @idCedulaUsuario";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idCedulaUsuario", idCedulaUsuario);
                        cmd.Parameters.AddWithValue("@nombreUsuario", usuario.nombreUsuario ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@apellidoUsuario", usuario.apellidoUsuario ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@correoElectronicoUsuario", usuario.correoElectronicoUsuario ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@contrasenaUsuario", contrasenaHashFinal);
                        cmd.Parameters.AddWithValue("@estado", usuario.estado);
                        cmd.Parameters.AddWithValue("@idPermiso", usuario.idPermiso);
                        cmd.Parameters.AddWithValue("@idImagen", nuevoIdImagen ?? (object)DBNull.Value);

                        var rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { mensaje = "Actualización exitosa", nuevaImagenId = nuevoIdImagen });
                        }
                        else
                        {
                            return NotFound(new { mensaje = "Usuario no encontrado." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al editar usuario: {ex.Message}" });
            }
        }


        [HttpPost("eliminarUsuario/{idUsuario}")]
        public IActionResult EliminarUsuario(int idUsuario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var cmd = new SqlCommand(
                        "UPDATE Usuario SET estado = 0 WHERE idUsuario = @idUsuario",
                        connection
                    );

                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);

                    var rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        // Retornamos 204 (No Content) indicando éxito sin contenido adicional
                        return NoContent();
                    }
                    else
                    {
                        return NotFound(new { mensaje = "Usuario no encontrado." });
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
