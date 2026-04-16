namespace TaskManagerAPI.Models
{
    public class Proyecto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public int PropietarioId { get; set; }

        
        public Usuario Propietario { get; set; } = null!;
        public ICollection<MiembroProyecto> Miembros { get; set; } = new List<MiembroProyecto>();
        public ICollection<TareaItem> Tareas { get; set; } = new List<TareaItem>();
    }
}
