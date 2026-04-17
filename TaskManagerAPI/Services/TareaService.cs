using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public class TareaService : ITareaService
    {
        private readonly AppDbContext _contexto;

        public TareaService(AppDbContext context)
        {
            _contexto = context;
        }

        private async Task<bool> UsuarioTieneAccesoAsync(int proyectoId, int usuarioId)
        {
            return await _contexto.Proyectos
                .AnyAsync(p => p.Id == proyectoId &&
                    (p.PropietarioId == usuarioId ||
                     p.Miembros.Any(m => m.UsuarioId == usuarioId)));
        }

        private static TareaResponseDTO MapearTarea(TareaItem tarea)
        {
            return new TareaResponseDTO
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                Estado = tarea.Estado.ToString(),
                ProyectoId = tarea.ProyectoId,
                AsignadoAId = tarea.AsignadoAId,
                AsignadoANombre = tarea.AsignadoA?.Nombre
            };
        }

        public async Task<PagedResultDTO<TareaResponseDTO>> GetTareasDelProyectoAsync(int proyectoId, int usuarioId, TareaFiltrosDTO filtros)
        {
            if (!await UsuarioTieneAccesoAsync(proyectoId, usuarioId))
                return new PagedResultDTO<TareaResponseDTO>();

            var query = _contexto.Tareas
                .Include(t => t.AsignadoA)
                .Where(t => t.ProyectoId == proyectoId);

            if (!string.IsNullOrEmpty(filtros.Estado))
            {
                if (!Enum.TryParse<EstadoTarea>(filtros.Estado, out var estado))
                    return new PagedResultDTO<TareaResponseDTO>();

                query = query.Where(t => t.Estado == estado);
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((filtros.Page - 1) * filtros.PageSize)
                .Take(filtros.PageSize)
                .Select(t => new TareaResponseDTO
                {
                    Id = t.Id,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    Estado = t.Estado.ToString(),
                    ProyectoId = t.ProyectoId,
                    AsignadoAId = t.AsignadoAId,
                    AsignadoANombre = t.AsignadoA != null ? t.AsignadoA.Nombre : null
                })
                .ToListAsync();

            return new PagedResultDTO<TareaResponseDTO>
            {
                Items = items,
                TotalItems = totalItems,
                Page = filtros.Page,
                PageSize = filtros.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalItems / filtros.PageSize)
            };
        }

        public async Task<TareaResponseDTO?> GetTareaByIdAsync(int proyectoId, int tareaId, int usuarioId)
        {
            if (!await UsuarioTieneAccesoAsync(proyectoId, usuarioId))
                return null;

            var tarea = await _contexto.Tareas
                .Include(t => t.AsignadoA)
                .FirstOrDefaultAsync(t => t.Id == tareaId && t.ProyectoId == proyectoId);

            if (tarea == null) return null;
            return MapearTarea(tarea);
        }

        public async Task<TareaResponseDTO?> CrearTareaAsync(int proyectoId, CrearTareaDTO dto, int usuarioId)
        {
            if (!await UsuarioTieneAccesoAsync(proyectoId, usuarioId))
                return null;

            var tarea = new TareaItem
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                ProyectoId = proyectoId,
                AsignadoAId = dto.AsignadoAId,
                Estado = EstadoTarea.Pendiente
            };

            _contexto.Tareas.Add(tarea);
            await _contexto.SaveChangesAsync();

            await _contexto.Entry(tarea).Reference(t => t.AsignadoA).LoadAsync();
            return MapearTarea(tarea);
        }

        public async Task<TareaResponseDTO?> ActualizarTareaAsync(int proyectoId, int tareaId, ActualizarTareaDTO dto, int usuarioId)
        {
            if (!await UsuarioTieneAccesoAsync(proyectoId, usuarioId))
                return null;

            var tarea = await _contexto.Tareas
                .Include(t => t.AsignadoA)
                .FirstOrDefaultAsync(t => t.Id == tareaId && t.ProyectoId == proyectoId);

            if (tarea == null) return null;

            tarea.Titulo = dto.Titulo;
            tarea.Descripcion = dto.Descripcion;
            tarea.AsignadoAId = dto.AsignadoAId;
            await _contexto.SaveChangesAsync();

            await _contexto.Entry(tarea).Reference(t => t.AsignadoA).LoadAsync();
            return MapearTarea(tarea);
        }

        public async Task<TareaResponseDTO?> ActualizarEstadoAsync(int proyectoId, int tareaId, ActualizarEstadoDTO dto, int usuarioId)
        {
            if (!await UsuarioTieneAccesoAsync(proyectoId, usuarioId))
                return null;

            var tarea = await _contexto.Tareas
                .Include(t => t.AsignadoA)
                .FirstOrDefaultAsync(t => t.Id == tareaId && t.ProyectoId == proyectoId);

            if (tarea == null) return null;

            if (!Enum.TryParse<EstadoTarea>(dto.Estado, out var nuevoEstado))
                return null;

            tarea.Estado = nuevoEstado;
            await _contexto.SaveChangesAsync();

            return MapearTarea(tarea);
        }

        public async Task<bool> EliminarTareaAsync(int proyectoId, int tareaId, int usuarioId)
        {
            if (!await UsuarioTieneAccesoAsync(proyectoId, usuarioId))
                return false;

            var tarea = await _contexto.Tareas
                .FirstOrDefaultAsync(t => t.Id == tareaId && t.ProyectoId == proyectoId);

            if (tarea == null) return false;

            _contexto.Tareas.Remove(tarea);
            await _contexto.SaveChangesAsync();
            return true;
        }

    }
}
