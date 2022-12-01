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
    [Route("rifas")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class RifasController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;
        public RifasController(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        [HttpGet]
        [HttpGet("/listadoRifa")]
        [AllowAnonymous]
        public async Task<ActionResult<List<RifaDTO>>> GetAll()
        {
            var listaRifa = await dbContext.Rifa.ToListAsync();

            if(listaRifa == null)
            {
                return NotFound("No hay rifas");
            }

            return mapper.Map<List<RifaDTO>>(listaRifa);
        }
        [HttpPost]
        public async Task<ActionResult> Post(RifaCreacionDTO rifaCreacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;
            var rifa = mapper.Map<Rifa>(rifaCreacionDTO);
            rifa.UsuarioId = usuarioId;
            dbContext.Add(rifa);
            await dbContext.SaveChangesAsync();

            return Ok();
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, RifaCreacionDTO rifaCreacionDTO)
        {
            var rifaDB = await dbContext.Rifa.FirstOrDefaultAsync(x => x.Id == id);

            if (rifaDB == null)
            {
                return NotFound();
            }

            rifaDB = mapper.Map(rifaCreacionDTO, rifaDB);

            
            dbContext.Update(rifaDB);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Rifa.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El Recurso no fue encontrado.");
            }

            dbContext.Remove(new Rifa { Id = id });
            await dbContext.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("/ObtenerGanador")]
        public async Task<ActionResult<Object>> GetGanador(int rifaId)
        {
            var existeRifa = await dbContext.Rifa.AnyAsync(rifaDB => rifaDB.Id == rifaId);
            if (!existeRifa)
            {
                return NotFound("No existe rifa");
            }

            var listaParticipantes = await dbContext.ParticipanteRifa.Where(rifaDB => rifaDB.RifaId == rifaId).ToListAsync();
            var listaPremios = await dbContext.Premio.Where(rifaDB => rifaDB.RifaId == rifaId).OrderBy(x => x.Rango).ToListAsync();
            if (listaParticipantes.Count == 0)
            {
                return BadRequest("No hay participantes");
            }
            
            if (listaPremios.Count==0)
            {
                return BadRequest("No hay premios");
            }

            if (listaParticipantes.Count < listaPremios.Count)
            {
                return BadRequest("No se puede realizar la rifa, hay menos participantes que premios");
            }

            Random random = new Random();

            var boletoGanador = random.Next(listaParticipantes.Count);

            var name = await dbContext.Participante.FirstOrDefaultAsync(x => x.Id == listaParticipantes[boletoGanador].ParticipanteId);
            var result = new
            {
                Ganador = name.Nombre,
                NumeroLoteria = listaParticipantes[boletoGanador].NumeroLoteria,
                Premio = listaPremios.Last().Nombre,
                GanadorNum = listaPremios.Last().Rango
            };

            dbContext.Premio.Remove(listaPremios.Last());
            await dbContext.SaveChangesAsync();
            if (listaPremios.Count==1)
            {
                dbContext.Remove(new Rifa { Id = rifaId });
                await dbContext.SaveChangesAsync();
            }
            return result;
        }

        
    }

}

