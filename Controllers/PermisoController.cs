using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace PruebaPatrickLisby.Controllers
{
    /// <summary>
    /// Controlador para manejar las operaciones CRUD relacionadas con los permisos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PermisoController : Controller
    {
        private readonly string _connectionString;

        /// <summary>
        /// Constructor que inicializa el controlador con la configuración de conexión a la base de datos.
        /// </summary>
        /// <param name="configuration">Proporciona acceso a las configuraciones de la aplicación.</param>
        public PermisoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        /// <summary>
        /// Obtiene todos los permisos de la base de datos.
        /// </summary>
        /// <returns>
        /// Un objeto `DataTable` con la información de todos los permisos.
        /// </returns>
        public DataTable ObtenerPermisos()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Permisos", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        /// <summary>
        /// Agrega un nuevo permiso a la base de datos.
        /// </summary>
        /// <param name="nombre">Nombre del permiso a agregar.</param>
        public void CrearPermiso(string nombre)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Permisos (Nombre) VALUES (@Nombre)", conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Actualiza un permiso existente en la base de datos.
        /// </summary>
        /// <param name="id">ID del permiso a actualizar.</param>
        /// <param name="nombre">Nuevo nombre del permiso.</param>
        public void EditarPermiso(int id, string nombre)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE Permisos SET Nombre = @Nombre WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// Elimina un permiso de la base de datos.
        /// </summary>
        /// <param name="id">ID del permiso a eliminar.</param>
        public void EliminarPermiso(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Permisos WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
