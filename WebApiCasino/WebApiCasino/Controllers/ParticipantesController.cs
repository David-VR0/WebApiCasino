using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.DTOs;
using WebApiCasino.Entidades;
namespace WebApiCasino.Controllers
{
    [ApiController]
    [Route("participantes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ParticipantesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public ParticipantesController(ApplicationDbContext context, IMapper mapper)
        {
            this.dbContext = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<GetParticipanteDTO>>> Get()
        {
            var participante = await dbContext.Participante.ToListAsync();
            return mapper.Map<List<GetParticipanteDTO>>(participante);
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ParticipanteDTO participanteDto)
        {

            var existeParticipanteMismoNombre = await dbContext.Participante.AnyAsync(x => x.Nombre == participanteDto.Nombre);

            if (existeParticipanteMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {participanteDto.Nombre}");
            }

            var participante = mapper.Map<Participante>(participanteDto);

            dbContext.Add(participante);
            await dbContext.SaveChangesAsync();

            var participanteDTO = mapper.Map<GetParticipanteDTO>(participante);

            return Ok();
        }

        [HttpPut("{id:int}")] // api/participantes/1
        public async Task<ActionResult> Put(ParticipanteDTO participanteCreacionDTO, int id)
        {
            var exist = await dbContext.Participante.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            var participante = mapper.Map<Participante>(participanteCreacionDTO);
            participante.Id = id;

            dbContext.Update(participante);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        [Authorize(Policy = "EsAdmin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Participante.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El Recurso no fue encontrado.");
            }

            dbContext.Remove(new Participante()
            {
                Id = id
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }

    }
}
