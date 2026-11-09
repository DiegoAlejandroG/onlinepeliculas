using api.Entidades;
using Microsoft.EntityFrameworkCore;

namespace api
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

        public DbSet<Genero> Generos { get; set; }
        public DbSet<Productora> Productoras { get; set; }
    }
}
