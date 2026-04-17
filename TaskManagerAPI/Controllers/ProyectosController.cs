using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    /// <summary>
    /// Gestión de proyectos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProyectosController : ControllerBase
    {
        private readonly IProyectoService _proyectoService;

        public ProyectosController(IProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }
        private int GetUsuarioId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (claim == null) throw new UnauthorizedAccessException();
            return int.Parse(claim);
        }
        /// <summary>
        /// Obtiene todos los proyectos del usuario autenticado
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResultDTO<ProyectoResponseDTO>>> GetProyectos([FromQuery] ProyectoFiltrosDTO filtros)
        {
            var usuarioId = GetUsuarioId();
            var proyectos = await _proyectoService.GetProyectosDelUsuarioAsync(usuarioId, filtros);
            return Ok(proyectos);
        }

        /// <summary>
        /// Obtiene el detalle de un proyecto por ID
        /// </summary>
        /// <response code="200">Proyecto encontrado</response>
        /// <response code="404">Proyecto no encontrado o no tenés acceso</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProyectoDetalleDTO>> GetProyecto(int id)
        {
            var usuarioId = GetUsuarioId();
            var proyecto = await _proyectoService.GetProyectoByIdAsync(id, usuarioId);
            if (proyecto == null) return NotFound();
            return Ok(proyecto);
        }

        /// <summary>
        /// Crea un nuevo proyecto
        /// </summary>
        /// <response code="201">Proyecto creado</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ProyectoResponseDTO>> CrearProyecto([FromBody] CrearProyectoDTO dto)
        {
            var usuarioId = GetUsuarioId();
            var proyecto = await _proyectoService.CrearProyectoAsync(dto, usuarioId);
            return CreatedAtAction(nameof(GetProyecto), new { id = proyecto.Id }, proyecto);
        }

        /// <summary>
        /// Actualiza un proyecto existente (solo el propietario)
        /// </summary>
        /// <response code="200">Proyecto actualizado</response>
        /// <response code="404">Proyecto no encontrado o no sos el propietario</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProyectoResponseDTO>> ActualizarProyecto(int id, [FromBody] ActualizarProyectoDTO dto)
        {
            var usuarioId = GetUsuarioId();
            var proyecto = await _proyectoService.ActualizarProyectoAsync(id, dto, usuarioId);
            if (proyecto == null) return NotFound();
            return Ok(proyecto);
        }

        /// <summary>
        /// Elimina un proyecto (solo el propietario)
        /// </summary>
        /// <response code="204">Proyecto eliminado</response>
        /// <response code="404">Proyecto no encontrado o no sos el propietario</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarProyecto(int id)
        {
            var usuarioId = GetUsuarioId();
            var eliminado = await _proyectoService.EliminarProyectoAsync(id, usuarioId);
            if (!eliminado) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Agrega un miembro al proyecto (solo el propietario)
        /// </summary>
        /// <response code="200">Miembro agregado</response>
        /// <response code="404">Proyecto o usuario no encontrado</response>
        [HttpPost("{id}/members")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AgregarMiembro(int id, [FromBody] AgregarMiembroDTO dto)
        {
            var usuarioId = GetUsuarioId();
            var agregado = await _proyectoService.AgregarMiembroAsync(id, dto.UsuarioId, usuarioId);
            if (!agregado) return NotFound();
            return Ok();
        }
    }
}
