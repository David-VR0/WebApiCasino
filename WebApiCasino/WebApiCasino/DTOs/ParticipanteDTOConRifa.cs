namespace WebApiCasino.DTOs
{
    public class ParticipanteDTOConRifa : GetParticipanteDTO
    {
        public List<RifaDTO> Rifas { get; set; }
    }
}
