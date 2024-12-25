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
        [HttpGet("getAllProductos")]
        public IActionResult GetProductos()
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

        [HttpGet("getProducto/{id}")]
        public IActionResult GetProducto(int id)
        {
            try
            {
                Producto producto = null;
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Productos WHERE IdProducto = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        producto = new Producto
                        {
                            IdProducto = (int)reader["IdProducto"],
                            DescripcionProducto = reader["DescripcionProducto"].ToString(),
                            DetallesProducto = reader["DetallesProducto"].ToString(),
                            PrecioProducto = (decimal)reader["PrecioProducto"],
                            CantidadProducto = (int)reader["CantidadProducto"],
                            FechaPublicacion = (DateTime)reader["FechaPublicacion"],
                            IdCategoria = (int)reader["IdCategoria"],
                            IdCedulaUsuarioRegistra = (int)reader["IdCedulaUsuarioRegistra"]
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
        public IActionResult CreateProducto([FromBody] Producto producto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Productos (descripcionProducto, detallesProducto, IdCategoria) VALUES (@descripcionProducto, @Precio, @IdCategoria)", conn);
                cmd.Parameters.AddWithValue("@descripcionProducto", producto.descripcion);
                cmd.Parameters.AddWithValue("@Precio", producto.Precio);
                cmd.Parameters.AddWithValue("@IdCategoria", producto.IdCategoria);
                cmd.ExecuteNonQuery();
            }
            return Ok();
        }

        public void UpdateProducto(int id, string nombre, int idCategoria)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("UPDATE Productos SET Nombre = @Nombre, IdCategoria = @IdCategoria WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@IdCategoria", idCategoria);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteProducto(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("DELETE FROM Productos WHERE Id = @Id", conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

}
