using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using PruebaPatrickLisby.Models;

namespace PruebaPatrickLisby.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiLoginController : ControllerBase
    {
        private readonly string _connectionString;

        public ApiLoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginRequest loginRequest)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Buscar el usuario por correo
                    string sql = @"
                SELECT idCedulaUsuario, nombreUsuario, apellidoUsuario, correoElectronicoUsuario, contrasenaUsuario, estado, idPermiso
                FROM Usuarios
                WHERE correoElectronicoUsuario = @correoElectronicoUsuario";

                    Usuario usuario = null;
                    using (var cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@correoElectronicoUsuario", loginRequest.Correo);

                        using (var reader = cmd.ExecuteReader())
                        {
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
                        }
                    }

                    if (usuario == null)
                    {
                        return Unauthorized(new { mensaje = "Credenciales inválidas." });
                    }

                    if (usuario.estado == 0)
                    {
                        return Unauthorized(new { mensaje = "El usuario está inactivo." });
                    }

                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Contrasena, usuario.contrasenaUsuario);
                    if (!isPasswordValid)
                    {
                        return Unauthorized(new { mensaje = "Credenciales inválidas." });
                    }

                    // Guardar información del usuario en la sesión
                    HttpContext.Session.SetString("UserId", usuario.idCedulaUsuario.ToString());
                    HttpContext.Session.SetString("UserName", usuario.nombreUsuario);
                    HttpContext.Session.SetString("UserEmail", usuario.correoElectronicoUsuario);

                    return Ok(new
                    {
                        id = usuario.idCedulaUsuario,
                        nombre = usuario.nombreUsuario,
                        apellido = usuario.apellidoUsuario,
                        correo = usuario.correoElectronicoUsuario,
                        idPermiso = usuario.idPermiso
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor.", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                // Limpiar la sesión del usuario
                HttpContext.Session.Clear();

                return Ok(new { mensaje = "Sesión cerrada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al cerrar sesión.", error = ex.Message });
            }
        }
    }
}
