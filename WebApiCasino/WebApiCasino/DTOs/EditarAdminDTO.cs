using System.ComponentModel.DataAnnotations;

namespace WebApiCasino.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
