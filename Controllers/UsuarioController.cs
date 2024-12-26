using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public IActionResult CrearUsuario([FromBody] Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

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
                idPermiso
            )
            VALUES (
                @idCedulaUsuario, 
                @nombreUsuario, 
                @apellidoUsuario, 
                @correoElectronicoUsuario, 
                @contrasenaUsuario, 
                @estado,
                @idPermiso
            )";

                    using (var cmdUsuario = new SqlCommand(insertarUsuarioSql, conn))
                    {
                        cmdUsuario.Parameters.AddWithValue("@idCedulaUsuario", usuario.idCedulaUsuario);
                        cmdUsuario.Parameters.AddWithValue("@nombreUsuario", usuario.nombreUsuario ?? (object)DBNull.Value);
                        cmdUsuario.Parameters.AddWithValue("@apellidoUsuario", usuario.apellidoUsuario ?? (object)DBNull.Value);
                        cmdUsuario.Parameters.AddWithValue("@correoElectronicoUsuario", usuario.correoElectronicoUsuario ?? (object)DBNull.Value);
                        cmdUsuario.Parameters.AddWithValue("@contrasenaUsuario", hashedPassword);
                        cmdUsuario.Parameters.AddWithValue("@estado", 1); // Estado por defecto
                        cmdUsuario.Parameters.AddWithValue("@idPermiso", 2); // Permiso por defecto para clientes

                        cmdUsuario.ExecuteNonQuery();
                    }
                }

                return Ok(new { mensaje = "Usuario creado exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al crear usuario: {ex.Message}" });
            }
        }

        [HttpPut("editarUsuario/{idCedulaUsuario}")]
        public IActionResult EditarUsuario(int idCedulaUsuario, [FromForm] Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Obtener la contraseña actual
                    string obtenerUsuarioSql = @"
                    SELECT contrasenaUsuario 
                    FROM Usuarios 
                    WHERE idCedulaUsuario = @idCedulaUsuario";

                    string contrasenaActual;

                    using (var obtenerCmd = new SqlCommand(obtenerUsuarioSql, conn))
                    {
                        obtenerCmd.Parameters.AddWithValue("@idCedulaUsuario", idCedulaUsuario);

                        using (var reader = obtenerCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                contrasenaActual = reader["contrasenaUsuario"].ToString();
                            }
                            else
                            {
                                return NotFound(new { mensaje = "Usuario no encontrado." });
                            }
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
                        idPermiso = @idPermiso
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

                        var rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok(new { mensaje = "Actualización exitosa" });
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
    }
}
