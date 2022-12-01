using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiCasino.Entidades;

namespace WebApiCasino
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ParticipanteRifa>()
                .HasKey(x => new { x.ParticipanteId, x.RifaId, x.NumeroLoteria });
        }

        public DbSet<Participante> Participante { get; set; }
        public DbSet<Rifa> Rifa { get; set; }
        public DbSet<Premio> Premio { get; set; }
        public DbSet<ParticipanteRifa> ParticipanteRifa { get; set; }
    }
}
