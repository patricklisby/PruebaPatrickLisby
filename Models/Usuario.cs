using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PruebaPatrickLisby.Models
{
    public class Usuario
    {

        public Usuario()
        {
            Productos = new List<Producto>();
            CarritosCompra = new List<CarritoCompra>();
            Permiso = null;
        }


        public int idCedulaUsuario { get; set; }
        public string nombreUsuario { get; set; }
        public string apellidoUsuario { get; set; }
        public string correoElectronicoUsuario { get; set; }
        public string contrasenaUsuario { get; set; }
        public int estado { get; set; } = 1;// Estado por defecto

        // Llave foránea
        public int idPermiso { get; set; } = 2;// Permiso por defecto para clientes

        [JsonIgnore]
        [NotMapped]
        public Permiso? Permiso { get; set; }

        // Relación con Productos
        [JsonIgnore]
        [NotMapped]
        public ICollection<Producto>? Productos { get; set; }

        // Relación con CarritoCompras
        [JsonIgnore]
        [NotMapped]
        public ICollection<CarritoCompra>? CarritosCompra { get; set; }
       
    }
}
