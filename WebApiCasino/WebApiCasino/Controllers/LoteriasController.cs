using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.DTOs;
using WebApiCasino.Entidades;

namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("rifas/{rifaId:int}/loteria")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LoteriasController: ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        public LoteriasController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;

        }
        [HttpGet]
        public async Task<ActionResult<List<LoteriaDTO>>> Get( int rifaId)
        {
            var existeRifa = await dbContext.Rifa.AnyAsync(rifaDB => rifaDB.Id == rifaId);
            if (!existeRifa)
            {
                return BadRequest("No existe rifa");
            }

            var result = new List<LoteriaDTO>();

            for (int i = 1; i <= 54; i++)
            {
                var existeLoteria = await dbContext.ParticipanteRifa.AnyAsync(loteriaDB => loteriaDB.NumeroLoteria == i && loteriaDB.RifaId==rifaId);
                if (!existeLoteria)
                {
                    result.Add(new LoteriaDTO()
                    {
                       NumeroLoteria = i,   
                    });
                }
            }

            return mapper.Map<List<LoteriaDTO>>(result);
        }
        [HttpGet("nombre")]
        
        public async Task<ActionResult<List<LoteriaDTO>>> Get(string nombreRifa, int rifaId)
        {
            var existeRifa = await dbContext.Rifa.AnyAsync(rifaDB => rifaDB.Nombre == nombreRifa);
            if (!existeRifa)
            {
                return NotFound();
            }

            var result = new List<LoteriaDTO>();

            for (int i = 1; i <= 54; i++)
            {
                var existeLoteria = await dbContext.ParticipanteRifa.AnyAsync(loteriaDB => loteriaDB.NumeroLoteria == i);
                if (!existeLoteria)
                {
                    result.Add(new LoteriaDTO()
                    {
                        NumeroLoteria = i,
                    });
                }
            }

            return mapper.Map<List<LoteriaDTO>>(result);
        }
        [HttpPost("TomarNumero")]
        public async Task<ActionResult> Put(int rifaId, int Participanteid, LoteriaCreacionDTO loteriaCreacionDTO)
        {
            var existeRifa = await dbContext.Rifa.AnyAsync(rifaDB => rifaDB.Id == rifaId);
            if (!existeRifa)
            {
                return NotFound("No existe rifa");
            }

            if (loteriaCreacionDTO.NumeroLoteria>54 || loteriaCreacionDTO.NumeroLoteria<0)
            {
                return BadRequest("Numero de loteria invalido");
            }

            var existeLoteria = await dbContext.ParticipanteRifa.AnyAsync(
                loteriaDB => (loteriaDB.NumeroLoteria == loteriaCreacionDTO.NumeroLoteria && loteriaDB.RifaId == rifaId)
                || (loteriaDB.ParticipanteId== Participanteid && loteriaDB.RifaId == rifaId)
                );
            if (existeLoteria)
            {
                return BadRequest("Numero no disponible");
            }

            var loteria = mapper.Map<ParticipanteRifa>(loteriaCreacionDTO);
            loteria.ParticipanteId= Participanteid;
            loteria.RifaId = rifaId; 
            dbContext.Add(loteria);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        

        
    }
}
