using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services
{
    public interface IProyectoService
    {
        Task<List<ProyectoResponseDTO>> GetProyectosDelUsuarioAsync(int usuarioId);
        Task<ProyectoDetalleDTO?> GetProyectoByIdAsync(int proyectoId, int usuarioId);
        Task<ProyectoResponseDTO> CrearProyectoAsync(CrearProyectoDTO dto, int usuarioId);
        Task<ProyectoResponseDTO?> ActualizarProyectoAsync(int proyectoId, ActualizarProyectoDTO dto, int usuarioId);
        Task<bool> EliminarProyectoAsync(int proyectoId, int usuarioId);
        Task<bool> AgregarMiembroAsync(int proyectoId, int usuarioIdTarget, int usuarioIdPropietario);
    }
}
