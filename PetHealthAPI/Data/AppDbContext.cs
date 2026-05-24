using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Models;

namespace PetHealthAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Tutor> Tutores { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Vacina> Vacinas { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<Medicamento> Medicamentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tutor
            modelBuilder.Entity<Tutor>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Nome).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Email).IsRequired().HasMaxLength(150);
                entity.Property(t => t.Telefone).IsRequired().HasMaxLength(20);
            });

            // Pet — relacionamento com Tutor
            modelBuilder.Entity<Pet>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Peso).HasPrecision(6, 2); // ex: 9999.99 kg
                entity.HasOne(p => p.Tutor)
                      .WithMany(t => t.Pets)
                      .HasForeignKey(p => p.TutorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Vacina — relacionamento com Pet
            modelBuilder.Entity<Vacina>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.HasOne(v => v.Pet)
                      .WithMany(p => p.Vacinas)
                      .HasForeignKey(v => v.PetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Consulta — relacionamento com Pet
            modelBuilder.Entity<Consulta>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Custo).HasPrecision(10, 2); // ex: 99999999.99
                entity.HasOne(c => c.Pet)
                      .WithMany(p => p.Consultas)
                      .HasForeignKey(c => c.PetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Medicamento — relacionamento com Pet
            modelBuilder.Entity<Medicamento>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.HasOne(m => m.Pet)
                      .WithMany(p => p.Medicamentos)
                      .HasForeignKey(m => m.PetId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
