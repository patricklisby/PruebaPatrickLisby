using System.Text.Json.Serialization;

namespace PruebaPatrickLisby.Models
{
    public class Producto
    {
        public int idProducto { get; set; }
        public string descripcionProducto { get; set; }
        public string detallesProducto { get; set; }
        public decimal precioProducto { get; set; }
        public int cantidadProducto { get; set; }
        public DateTime fechaPublicacion { get; set; }

        // Llave foránea
        public int idCategoria { get; set; }
        [JsonIgnore]
        public Categoria Categoria { get; set; }

        public int idCedulaUsuarioRegistra { get; set; }
        [JsonIgnore]
        public Usuario UsuarioRegistra { get; set; }

        // Relación con CarritoCompras
        [JsonIgnore]
        public ICollection<CarritoCompra> CarritosCompra { get; set; }
    }
}
