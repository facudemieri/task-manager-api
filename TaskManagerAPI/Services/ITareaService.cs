using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services
{
    public interface ITareaService
    {
        Task<PagedResultDTO<TareaResponseDTO>> GetTareasDelProyectoAsync(int proyectoId, int usuarioId, TareaFiltrosDTO filtros);
        Task<TareaResponseDTO?> GetTareaByIdAsync(int proyectoId, int tareaId, int usuarioId);
        Task<TareaResponseDTO?> CrearTareaAsync(int proyectoId, CrearTareaDTO dto, int usuarioId);
        Task<TareaResponseDTO?> ActualizarTareaAsync(int proyectoId, int tareaId, ActualizarTareaDTO dto, int usuarioId);
        Task<TareaResponseDTO?> ActualizarEstadoAsync(int proyectoId, int tareaId, ActualizarEstadoDTO dto, int usuarioId);
        Task<bool> EliminarTareaAsync(int proyectoId, int tareaId, int usuarioId);
    }
}
