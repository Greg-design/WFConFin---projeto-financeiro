using Microsoft.EntityFrameworkCore;
using WFConFin.Models;

// contexto do banco de dados
namespace WFConFin.Data
{ 
    public class WFConFinDbContext : DbContext
    {

        public WFConFinDbContext(DbContextOptions<WFConFinDbContext> options) : base(options)
        {
        }

        public DbSet<Cidade> Cidade { get; set; }
        public DbSet<Conta> Conta { get; set; }
        public DbSet<Estado> Estado { get; set; }
        public DbSet<Pessoa> Pessoa { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
    }
}
