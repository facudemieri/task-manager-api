namespace TaskManagerAPI.Models
{
    public enum EstadoTarea
    {
        Pendiente,
        EnProgreso,
        Completada
    }

    public class TareaItem
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public EstadoTarea Estado { get; set; } = EstadoTarea.Pendiente;
        public int ProyectoId { get; set; }
        public int? AsignadoAId { get; set; }
        
        public Proyecto Proyecto { get; set; } = null!;
        public Usuario? AsignadoA { get; set; }
    }
}
