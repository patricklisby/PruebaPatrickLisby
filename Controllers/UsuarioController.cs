using Microsoft.AspNetCore.Mvc;
using System.Data;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
        [HttpGet("getAllUsuarios")]
        public DataTable GetAllUsuarios()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Usuarios", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public void AddUsuario(string nombre, int idPermiso)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("INSERT INTO Usuarios (Nombre, IdPermiso) VALUES (@Nombre, @IdPermiso)", conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@IdPermiso", idPermiso);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUsuario(int id, string nombre, int idPermiso)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE Usuarios SET Nombre = @Nombre, IdPermiso = @IdPermiso WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@IdPermiso", idPermiso);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteUsuario(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Usuarios WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
