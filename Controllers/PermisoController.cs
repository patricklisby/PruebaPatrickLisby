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
    public class PermisoController : Controller
    {
        private readonly string _connectionString;

        public PermisoController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public DataTable GetAllPermisos()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Permisos", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public void AddPermiso(string nombre)
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

        public void UpdatePermiso(int id, string nombre)
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

        public void DeletePermiso(int id)
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
