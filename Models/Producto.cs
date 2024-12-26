using System.ComponentModel.DataAnnotations.Schema;
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
        public int estado { get; set; }
        public DateTime fechaPublicacion { get; set; }

        // Llave foránea
        public int idCategoria { get; set; }
        public int? IdImagen { get; set; }
        public int idCedulaUsuarioRegistra { get; set; }

        [JsonIgnore]
        [NotMapped]
        public Categoria? Categoria { get; set; }

 
        [JsonIgnore]
        [NotMapped]
        
        public Usuario? UsuarioRegistra { get; set; }

        // Relación con CarritoCompras
        [JsonIgnore]
        [NotMapped]
        public ICollection<CarritoCompra>? CarritosCompra { get; set; }

        //Relación con Imagenes
        [JsonIgnore]
        [NotMapped]
        public Imagenes? Imagen { get; set; }
        [NotMapped]
        public string? ImagenUrl { get; set; }
    }
}
