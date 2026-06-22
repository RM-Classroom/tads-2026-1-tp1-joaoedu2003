using Microsoft.EntityFrameworkCore;

public class LocadoraContext : DbContext
{
    public DbSet<Veiculo> Veiculos { get; set; }
    public DbSet<Fabricante> Fabricantes { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Aluguel> Alugueis { get; set; }
    public DbSet<Categoria> Categorias { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=localhost;Database=LocadoraDB;User Id=sa;Password=j2@@338$$$!&;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.cpf)
            .IsUnique();

        modelBuilder.Entity<Aluguel>()
            .Property(a => a.valorTotal)
            .HasPrecision(18, 2);
    }
}
