using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
   
        public class ProyectoService : IProyectoService
        {
            private readonly AppDbContext _contexto;

            public ProyectoService(AppDbContext contexto)
            {
                _contexto = contexto;
            }

            public async Task<PagedResultDTO<ProyectoResponseDTO>> GetProyectosDelUsuarioAsync(int usuarioId, ProyectoFiltrosDTO filtros)
            {
                var query = _contexto.Proyectos
                    .Where(p => p.PropietarioId == usuarioId ||
                    p.Miembros.Any(m => m.UsuarioId == usuarioId));

                if (!string.IsNullOrEmpty(filtros.Nombre))
                    query = query.Where(p => p.Nombre.Contains(filtros.Nombre));

                var totalItems = await query.CountAsync();

                var items = await query
                    .Skip((filtros.Page - 1) * filtros.PageSize)
                    .Take(filtros.PageSize)
                    .Select(p => new ProyectoResponseDTO
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        CreadoEn = p.CreadoEn,
                        PropietarioId = p.PropietarioId,
                        PropietarioNombre = p.Propietario.Nombre
                    })
                    .ToListAsync();

                return new PagedResultDTO<ProyectoResponseDTO>
                {
                    Items = items,
                    TotalItems = totalItems,
                    Page = filtros.Page,
                    PageSize = filtros.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalItems / filtros.PageSize)
                };
            }

            public async Task<ProyectoDetalleDTO?> GetProyectoByIdAsync(int proyectoId, int usuarioId)
            {
                var proyecto = await _contexto.Proyectos
                    .Include(p => p.Propietario)
                    .Include(p => p.Miembros)
                        .ThenInclude(m => m.Usuario)
                    .FirstOrDefaultAsync(p => p.Id == proyectoId &&
                        (p.PropietarioId == usuarioId ||
                         p.Miembros.Any(m => m.UsuarioId == usuarioId)));

                if (proyecto == null) return null;

                return new ProyectoDetalleDTO
                {
                    Id = proyecto.Id,
                    Nombre = proyecto.Nombre,
                    Descripcion = proyecto.Descripcion,
                    CreadoEn = proyecto.CreadoEn,
                    PropietarioId = proyecto.PropietarioId,
                    PropietarioNombre = proyecto.Propietario.Nombre,
                    Miembros = proyecto.Miembros.Select(m => new MiembroDTO
                    {
                        UsuarioId = m.UsuarioId,
                        Nombre = m.Usuario.Nombre,
                        Email = m.Usuario.Email,
                        UnidoEn = m.UnidoEn
                    }).ToList()
                };
            }

            public async Task<ProyectoResponseDTO> CrearProyectoAsync(CrearProyectoDTO dto, int usuarioId)
            {
                var proyecto = new Proyecto
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    PropietarioId = usuarioId,
                    CreadoEn = DateTime.UtcNow
                };

                _contexto.Proyectos.Add(proyecto);
                await _contexto.SaveChangesAsync();

                
                _contexto.MiembrosProyecto.Add(new MiembroProyecto
                {
                    ProyectoId = proyecto.Id,
                    UsuarioId = usuarioId,
                    UnidoEn = DateTime.UtcNow
                });
                await _contexto.SaveChangesAsync();

                var propietario = await _contexto.Usuarios.FindAsync(usuarioId);

                return new ProyectoResponseDTO
                {
                    Id = proyecto.Id,
                    Nombre = proyecto.Nombre,
                    Descripcion = proyecto.Descripcion,
                    CreadoEn = proyecto.CreadoEn,
                    PropietarioId = proyecto.PropietarioId,
                    PropietarioNombre = propietario!.Nombre
                };
            }

            public async Task<ProyectoResponseDTO?> ActualizarProyectoAsync(int proyectoId, ActualizarProyectoDTO dto, int usuarioId)
            {
                var proyecto = await _contexto.Proyectos
                    .Include(p => p.Propietario)
                    .FirstOrDefaultAsync(p => p.Id == proyectoId && p.PropietarioId == usuarioId);

                if (proyecto == null) return null;

                proyecto.Nombre = dto.Nombre;
                proyecto.Descripcion = dto.Descripcion;
                await _contexto.SaveChangesAsync();

                return new ProyectoResponseDTO
                {
                    Id = proyecto.Id,
                    Nombre = proyecto.Nombre,
                    Descripcion = proyecto.Descripcion,
                    CreadoEn = proyecto.CreadoEn,
                    PropietarioId = proyecto.PropietarioId,
                    PropietarioNombre = proyecto.Propietario.Nombre
                };
            }

            public async Task<bool> EliminarProyectoAsync(int proyectoId, int usuarioId)
            {
                var proyecto = await _contexto.Proyectos
                    .FirstOrDefaultAsync(p => p.Id == proyectoId && p.PropietarioId == usuarioId);

                if (proyecto == null) return false;

                _contexto.Proyectos.Remove(proyecto);
                await _contexto.SaveChangesAsync();
                return true;
            }

            public async Task<bool> AgregarMiembroAsync(int proyectoId, int usuarioIdTarget, int usuarioIdPropietario)
            {
                var proyecto = await _contexto.Proyectos
                    .FirstOrDefaultAsync(p => p.Id == proyectoId && p.PropietarioId == usuarioIdPropietario);

                if (proyecto == null) return false;

                var usuarioExiste = await _contexto.Usuarios.AnyAsync(u => u.Id == usuarioIdTarget);
                if (!usuarioExiste) return false;

                var yaesMiembro = await _contexto.MiembrosProyecto
                    .AnyAsync(m => m.ProyectoId == proyectoId && m.UsuarioId == usuarioIdTarget);
                if (yaesMiembro) return false;

                _contexto.MiembrosProyecto.Add(new MiembroProyecto
                {
                    ProyectoId = proyectoId,
                    UsuarioId = usuarioIdTarget,
                    UnidoEn = DateTime.UtcNow
                });
                await _contexto.SaveChangesAsync();
                return true;
            }
        }
    }

