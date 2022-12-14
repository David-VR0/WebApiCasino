using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using WebApiCasino.Validaciones;

namespace WebApiCasino.Entidades
{
    public class Rifa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")] //
        [StringLength(maximumLength: 150, ErrorMessage = "El campo {0} solo puede tener hasta 150 caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        public string UsuarioId { get; set; }

        public IdentityUser Usuario { get; set; }

        public List<Premio> Premios { get; set; }
        
        public List<ParticipanteRifa> ParticipanteRifas { get; set; }
    }
}
