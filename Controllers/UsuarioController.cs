using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Data;
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
                    int estado = 1;
                    int idPermiso = 2;

                    //Encriptar la contrasena
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(usuario.contrasenaUsuario);

                    SqlCommand cmd = new SqlCommand(@"
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
                    )",
                        conn);

                    cmd.Parameters.AddWithValue("@idCedulaUsuario", usuario.idCedulaUsuario);
                    cmd.Parameters.AddWithValue("@nombreUsuario", usuario.nombreUsuario);
                    cmd.Parameters.AddWithValue("@apellidoUsuario", usuario.apellidoUsuario);
                    cmd.Parameters.AddWithValue("@correoElectronicoUsuario", usuario.correoElectronicoUsuario);
                    cmd.Parameters.AddWithValue("@contrasenaUsuario", hashedPassword);
                    cmd.Parameters.AddWithValue("@estado", estado);
                    cmd.Parameters.AddWithValue("@idPermiso", idPermiso);
                    cmd.ExecuteNonQuery();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear usuario: {ex.Message}");
            }
        }

        [HttpPut("editarUsuario/{idCedulaUsuario}")]
        public IActionResult EditarUsuario(int idCedulaUsuario, [FromBody] Usuario usuario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Verificar si la contraseña se actualizó
                    string nuevaContrasenaHash = null;
                    if (!string.IsNullOrWhiteSpace(usuario.contrasenaUsuario))
                    {
                        nuevaContrasenaHash = BCrypt.Net.BCrypt.HashPassword(usuario.contrasenaUsuario);
                    }

                    // Obtener la contraseña actual si no se envió una nueva
                    string obtenerContrasenaSql = @"
                SELECT contrasenaUsuario 
                FROM Usuarios 
                WHERE idCedulaUsuario = @idCedulaUsuario";

                    string contrasenaActual;
                    using (var obtenerCmd = new SqlCommand(obtenerContrasenaSql, connection))
                    {
                        obtenerCmd.Parameters.AddWithValue("@idCedulaUsuario", idCedulaUsuario);

                        var resultado = obtenerCmd.ExecuteScalar();
                        if (resultado == null)
                        {
                            return NotFound(new { mensaje = "Usuario no encontrado." });
                        }
                        contrasenaActual = resultado.ToString();
                    }

                    // Usar la contraseña actual si no se actualizó
                    string contrasenaHashFinal = nuevaContrasenaHash ?? contrasenaActual;

                    // Actualizar el usuario
                    string sql = @"
                UPDATE Usuarios
                SET nombreUsuario            = @nombreUsuario,
                    apellidoUsuario          = @apellidoUsuario,
                    correoElectronicoUsuario = @correoElectronicoUsuario,
                    contrasenaUsuario        = @contrasenaUsuario,
                    estado                   = @estado,
                    idPermiso                = @idPermiso
                WHERE idCedulaUsuario = @idCedulaUsuario";

                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@idCedulaUsuario", idCedulaUsuario);
                        cmd.Parameters.AddWithValue("@nombreUsuario", usuario.nombreUsuario);
                        cmd.Parameters.AddWithValue("@apellidoUsuario", usuario.apellidoUsuario);
                        cmd.Parameters.AddWithValue("@correoElectronicoUsuario", usuario.correoElectronicoUsuario);
                        cmd.Parameters.AddWithValue("@contrasenaUsuario", contrasenaHashFinal);
                        cmd.Parameters.AddWithValue("@estado", usuario.estado);
                        cmd.Parameters.AddWithValue("@idPermiso", usuario.idPermiso);

                        var rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return StatusCode(204, new { mensaje = "Actualización exitosa" });
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
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }

        [HttpPost("eliminarUsuario/{idCedulaUsuario}")]
        public IActionResult EliminarProducto(int idCedulaUsuario)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Realizamos la "baja lógica" en lugar de un DELETE
                    var cmd = new SqlCommand(
                        "UPDATE Usuarios SET estado = 0 WHERE idCedulaUsuario = @idCedulaUsuario",
                        connection
                    );

                    cmd.Parameters.AddWithValue("@idCedulaUsuario", idCedulaUsuario);

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
