using System.ComponentModel.DataAnnotations;
using WebApiCasino.Validaciones;

namespace WebApiCasino.DTOs
{
    public class GetParticipanteDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
}
