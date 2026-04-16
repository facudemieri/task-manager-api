using TaskManagerAPI.DTOs;

namespace TaskManagerAPI.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> Register(RegisterDTO dto);
        Task<AuthResponseDTO> Login(LoginDTO dto);
    }
}
