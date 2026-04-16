namespace TaskManagerAPI.DTOs
{
    public class CrearTareaDTO
    {
        public required string Titulo { get; set; }
        public string? Descripcion { get; set; }
        public int? AsignadoAId { get; set; }
    }

    public class ActualizarTareaDTO
    {
        public required string Titulo { get; set; }
        public string? Descripcion { get; set; }
        public int? AsignadoAId { get; set; }
    }

    public class ActualizarEstadoDTO
    {
        public required string Estado { get; set; }
    }

    public class TareaResponseDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int ProyectoId { get; set; }
        public int? AsignadoAId { get; set; }
        public string? AsignadoANombre { get; set; }
    }
}
