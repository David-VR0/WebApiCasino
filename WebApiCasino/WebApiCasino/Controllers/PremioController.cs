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
    [Route("premios")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class PremioController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        public PremioController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        [AllowAnonymous]
        [HttpGet(Name = "obtenerPremio")]
        public async Task<ActionResult<List<PremioDTO>>> Get(int rifaid)
        {
            var premio = await dbContext.Premio.Where(premioDB => premioDB.RifaId == rifaid).ToListAsync();

            if (premio == null)
            {
                return NotFound();
            }

           return mapper.Map<List<PremioDTO>>(premio);
        }
        [HttpPost]
        public async Task<ActionResult> Post(int rifaId, PremioCreacionDTO premioCreacionDTO)
        {

            var existeRifa = await dbContext.Rifa.AnyAsync(rifaDB => rifaDB.Id == rifaId);
            if (!existeRifa)
            {
                return NotFound("No existe rifa");
            }
            var existePremio = await dbContext.Premio.AnyAsync(rifaDB => (rifaDB.Rango == premioCreacionDTO.Rango) && (rifaDB.Id == rifaId));
            if (existePremio)
            {
                return NotFound("Ya hay un premio con el mismo rango");
            }

            var premio = mapper.Map<Premio>(premioCreacionDTO);

            premio.RifaId = rifaId;
            dbContext.Add(premio);
            await dbContext.SaveChangesAsync();

            var premioDTO = mapper.Map<PremioDTO>(premio);

            return CreatedAtRoute("obtenerPremio", new { id = premio.Id, rifaId = rifaId }, premioDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int rifaId, int id, PremioCreacionDTO premioCreacionDTO)
        {
            var existeRifa = await dbContext.Rifa.AnyAsync(vetDB => vetDB.Id == rifaId);
            if (!existeRifa)
            {
                return NotFound();
            }

            var existePremio = await dbContext.Premio.AnyAsync(premioDB => premioDB.Id == id);
            if (!existePremio)
            {
                return NotFound();
            }
            
            var premio = mapper.Map<Premio>(premioCreacionDTO);
            premio.Id = id;
            premio.RifaId = rifaId;
            dbContext.Update(premio);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
