using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    /// <summary>
    /// Gestión de tareas dentro de un proyecto
    /// </summary>
    [ApiController]
    [Route("api/proyectos/{proyectoId}/tareas")]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private readonly ITareaService _tareaService;

        public TareasController(ITareaService tareaService)
        {
            _tareaService = tareaService;
        }

        private int GetUsuarioId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (claim == null) throw new UnauthorizedAccessException();
            return int.Parse(claim);
        }

        /// <summary>
        /// Obtiene todas las tareas de un proyecto
        /// </summary>
        /// <response code="200">Lista de tareas</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResultDTO<TareaResponseDTO>>> GetTareas(int proyectoId, [FromQuery] TareaFiltrosDTO filtros)
        {
            var usuarioId = GetUsuarioId();
            var tareas = await _tareaService.GetTareasDelProyectoAsync(proyectoId, usuarioId, filtros);
            return Ok(tareas);
        }

        /// <summary>
        /// Obtiene una tarea por ID
        /// </summary>
        /// <response code="200">Tarea encontrada</response>
        /// <response code="404">Tarea no encontrada o sin acceso</response>
        [HttpGet("{tareaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TareaResponseDTO>> GetTarea(int proyectoId, int tareaId)
        {
            var usuarioId = GetUsuarioId();
            var tarea = await _tareaService.GetTareaByIdAsync(proyectoId, tareaId, usuarioId);
            if (tarea == null) return NotFound();
            return Ok(tarea);
        }

        /// <summary>
        /// Crea una nueva tarea en el proyecto
        /// </summary>
        /// <response code="201">Tarea creada</response>
        /// <response code="404">Proyecto no encontrado o sin acceso</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TareaResponseDTO>> CrearTarea(int proyectoId, [FromBody] CrearTareaDTO dto)
        {
            var usuarioId = GetUsuarioId();
            var tarea = await _tareaService.CrearTareaAsync(proyectoId, dto, usuarioId);
            if (tarea == null) return NotFound();
            return CreatedAtAction(nameof(GetTarea), new { proyectoId, tareaId = tarea.Id }, tarea);
        }

        /// <summary>
        /// Actualiza una tarea existente
        /// </summary>
        /// <response code="200">Tarea actualizada</response>
        /// <response code="404">Tarea no encontrada o sin acceso</response>
        [HttpPut("{tareaId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TareaResponseDTO>> ActualizarTarea(int proyectoId, int tareaId, [FromBody] ActualizarTareaDTO dto)
        {
            var usuarioId = GetUsuarioId();
            var tarea = await _tareaService.ActualizarTareaAsync(proyectoId, tareaId, dto, usuarioId);
            if (tarea == null) return NotFound();
            return Ok(tarea);
        }

        /// <summary>
        /// Actualiza el estado de una tarea
        /// </summary>
        /// <response code="200">Estado actualizado</response>
        /// <response code="400">Estado inválido</response>
        /// <response code="404">Tarea no encontrada o sin acceso</response>
        [HttpPatch("{tareaId}/estado")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TareaResponseDTO>> ActualizarEstado(int proyectoId, int tareaId, [FromBody] ActualizarEstadoDTO dto)
        {
            var usuarioId = GetUsuarioId();
            var tarea = await _tareaService.ActualizarEstadoAsync(proyectoId, tareaId, dto, usuarioId);
            if (tarea == null) return NotFound();
            return Ok(tarea);
        }

        /// <summary>
        /// Elimina una tarea
        /// </summary>
        /// <response code="204">Tarea eliminada</response>
        /// <response code="404">Tarea no encontrada o sin acceso</response>
        [HttpDelete("{tareaId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarTarea(int proyectoId, int tareaId)
        {
            var usuarioId = GetUsuarioId();
            var eliminado = await _tareaService.EliminarTareaAsync(proyectoId, tareaId, usuarioId);
            if (!eliminado) return NotFound();
            return NoContent();
        }

    }
}
