using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PruebaPatrickLisby.Models
{
    public class Categoria
    {
        public int idCategoria { get; set; }
        public string descripcionCategoria { get; set; }

        // Relación con Productos
        [JsonIgnore]
        [NotMapped]
        public ICollection<Producto>? Productos { get; set; }
    }
}
