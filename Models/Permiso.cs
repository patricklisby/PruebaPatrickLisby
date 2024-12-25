using System.Text.Json.Serialization;

namespace PruebaPatrickLisby.Models
{
    public class Permiso
    {
        public int idPermiso { get; set; }
        public string descripcionPermiso { get; set; }

        // Relación con Usuarios
        [JsonIgnore]
        public ICollection<Usuario> Usuarios { get; set; }
    }
}
