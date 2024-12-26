namespace PruebaPatrickLisby.Models
{
    public class Imagenes
    {
        public Imagenes()
        {
            fechaSubida = DateTime.Now; // Fecha actual por defecto
        }

        public int idImagen { get; set; } 
        public string urlImagen { get; set; } 
        public DateTime fechaSubida { get; set; } 

      
       
    }
}
