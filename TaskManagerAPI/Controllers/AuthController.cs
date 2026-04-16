using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Controllers
{
    /// <summary>
    /// Registro y login de usuarios
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registra un nuevo usuario
        /// </summary>
        /// <response code="200">Usuario registrado, devuelve el token</response>
        /// <response code="400">El email ya existe</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDTO>> Register([FromBody] RegisterDTO dto)
        {
            var result = await _authService.Register(dto);
            return Ok(result);
        }

        /// <summary>
        /// Login de usuario existente
        /// </summary>
        /// <response code="200">Login exitoso, devuelve el token</response>
        /// <response code="400">Credenciales incorrectas</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDTO>> Login([FromBody] LoginDTO dto)
        {
            var result = await _authService.Login(dto);
            return Ok(result);
        }
    }
}
