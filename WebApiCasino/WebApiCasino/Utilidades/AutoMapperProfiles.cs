using AutoMapper;
using WebApiCasino.DTOs;
using WebApiCasino.Entidades;
namespace WebApiCasino.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ParticipanteDTO, Participante>();
            CreateMap<Participante, GetParticipanteDTO>();
            CreateMap<Participante, ParticipanteDTOConRifa>()
                .ForMember(ParticipanteDTO => ParticipanteDTO.Rifas, opciones => opciones.MapFrom(MapParticipanteDTORifas));
            CreateMap<RifaCreacionDTO, Rifa>();
            CreateMap<Rifa, RifaDTO>();
            CreateMap<Rifa, RifaDTOConParticipante>()
                .ForMember(rifaDTO => rifaDTO.Participantes, opciones => opciones.MapFrom(MapRifasDTOParticipantes));
            CreateMap<PremioCreacionDTO, Premio>();
            CreateMap<Premio, PremioDTO>();
            CreateMap<LoteriaCreacionDTO, ParticipanteRifa>();
        }
        private List<RifaDTO> MapParticipanteDTORifas(Participante participante, GetParticipanteDTO getParticipanteDTO)
        {
            var result = new List<RifaDTO>();
            if (participante.ParticipanteRifas == null) { return result; }

            foreach (var participanteRifa in participante.ParticipanteRifas)
            {
                result.Add(new RifaDTO()
                {
                    Id = participanteRifa.RifaId,
                    Nombre = participanteRifa.Rifa.Nombre
                });
            }
            return result;
        }
        private List<GetParticipanteDTO> MapRifasDTOParticipantes(Rifa rifa, RifaDTO rifaDTO)
        {
            var result = new List<GetParticipanteDTO>();

            if (rifa.ParticipanteRifas == null)
            {
                return result;
            }

            foreach (var participanteRifa in rifa.ParticipanteRifas)
            {
                result.Add(new GetParticipanteDTO()
                {
                    Id = participanteRifa.ParticipanteId,
                    Nombre = participanteRifa.Participante.Nombre
                });
            }
            return result;
        }
    }
}
