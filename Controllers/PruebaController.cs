using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

[Route("api/[controller]")]
[ApiController]
public class PruebaController : ControllerBase
{
    private readonly SqlConnection _connection;

    public PruebaController(SqlConnection connection)
    {
        _connection = connection;
    }

    [HttpGet("test-connection")]
    public IActionResult TestConnection()
    {
        try
        {
            _connection.Open();
            return Ok("Conexión exitosa a la base de datos.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al conectar: {ex.Message}");
        }
        finally
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }
    }
}
