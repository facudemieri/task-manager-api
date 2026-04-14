namespace TaskManagerAPI.Models
{
    public class MiembroProyecto
    {
        public int ProyectoId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime UnidoEn { get; set; } = DateTime.UtcNow;
       
        public Proyecto Proyecto { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}
