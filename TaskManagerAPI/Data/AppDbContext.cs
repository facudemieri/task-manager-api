using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Proyecto> Proyectos { get; set; }
        public DbSet<TareaItem> Tareas { get; set; }
        public DbSet<MiembroProyecto> MiembrosProyecto { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MiembroProyecto>()
                .HasKey(m => new { m.ProyectoId, m.UsuarioId });

            modelBuilder.Entity<Proyecto>()
                .HasOne(p => p.Propietario)
                .WithMany()
                .HasForeignKey(p => p.PropietarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TareaItem>()
                .HasOne(t => t.AsignadoA)
                .WithMany(u => u.TareasAsignadas)
                .HasForeignKey(t => t.AsignadoAId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TareaItem>()
                .HasOne(t => t.Proyecto)
                .WithMany(u => u.Tareas)
                .HasForeignKey(t => t.ProyectoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MiembroProyecto>()
                .HasOne(m => m.Proyecto)
                .WithMany(p => p.Miembros)
                .HasForeignKey(m => m.ProyectoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MiembroProyecto>()
                .HasOne(m => m.Usuario)
                .WithMany(u => u.MiembrosProyecto)
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
