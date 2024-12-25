using System.Text.Json.Serialization;

namespace PruebaPatrickLisby.Models
{
    public class Usuario
    {
        public int idCedulaUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public string apellidoUsuario { get; set; }
        public string correoElectronicoUsuario { get; set; }
        public string contrasenaUsuario { get; set; }

        // Llave foránea
        public int idPermiso { get; set; }
        [JsonIgnore]
        public Permiso Permiso { get; set; }

        // Relación con Productos
        [JsonIgnore]
        public ICollection<Producto> Productos { get; set; }

        // Relación con CarritoCompras
        [JsonIgnore]
        public ICollection<CarritoCompra> CarritosCompra { get; set; }
    }
}
