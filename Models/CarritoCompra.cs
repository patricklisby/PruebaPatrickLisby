using System.Text.Json.Serialization;

namespace PruebaPatrickLisby.Models
{
    public class CarritoCompra
    {
        public int idCarrito { get; set; }

        // Llave foránea
        public int idProducto { get; set; }
        [JsonIgnore]
        public Producto Producto { get; set; }

        public int idCedulaUsuarioCompra { get; set; }
        [JsonIgnore]
        public Usuario UsuarioCompra { get; set; }

        public DateTime fechaSeleccion { get; set; }
    }
}
