using System.ComponentModel.DataAnnotations;
namespace WebApiCasino.Entidades
{
    public class Premio 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        [Range(1, 100, ErrorMessage = "El campo no se encuentra dentro del rango")]
        public int Rango { get; set; }
        public int RifaId { get; set; }
        public Rifa Rifa { get; set; }



    }
}

