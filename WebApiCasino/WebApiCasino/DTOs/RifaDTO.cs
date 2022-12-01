using System.ComponentModel.DataAnnotations;
using WebApiCasino.Entidades;
using WebApiCasino.Validaciones;

namespace WebApiCasino.DTOs
{
    public class RifaDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")] //
        [StringLength(maximumLength: 150, ErrorMessage = "El campo {0} solo puede tener hasta 150 caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        public List<Premio> Premios { get; set; }
    }
}
