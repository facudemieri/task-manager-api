namespace TaskManagerAPI.DTOs
{
    public class CrearProyectoDTO
    {
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
    }

    public class ActualizarProyectoDTO
    {
        public required string Nombre { get; set; }
        public string? Descripcion { get; set; }
    }

    public class ProyectoResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public DateTime CreadoEn { get; set; }
        public int PropietarioId { get; set; }
        public string PropietarioNombre { get; set; } = string.Empty;
    }

    public class MiembroDTO
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime UnidoEn { get; set; }
    }

    public class ProyectoDetalleDTO : ProyectoResponseDTO
    {
        public List<MiembroDTO> Miembros { get; set; } = new();
    }

    public class AgregarMiembroDTO
    {
        public int UsuarioId { get; set; }
    }
}
