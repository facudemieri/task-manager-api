using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Exceptions;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _contexto;
        private readonly IConfiguration _configuracion;

        public AuthService(AppDbContext contexto, IConfiguration configuracion)
        {
            _contexto = contexto;
            _configuracion = configuracion;
        }

        public async Task<AuthResponseDTO> Login(LoginDTO dto)
        {
            var usuario = await _contexto.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                throw new BadRequestException("Email o contraseña incorrectos");

            return new AuthResponseDTO
            {
                Token = GenerateToken(usuario),
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };
        }

        public async Task<AuthResponseDTO> Register(RegisterDTO dto)
        {
            var UsuarioExistente = await _contexto.Usuarios
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (UsuarioExistente != null)
                throw new BadRequestException("Ya existe un usuario con ese email");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _contexto.Usuarios.Add(usuario);
            await _contexto.SaveChangesAsync();

            return new AuthResponseDTO
            {
                Token = GenerateToken(usuario),
                Nombre = usuario.Nombre,
                Email = usuario.Email
            };
        }

        private string GenerateToken(Usuario usuario)
        {
            var secret = _configuracion["JwtSettings:Secret"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email.ToString())
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
