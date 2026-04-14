namespace TaskManagerAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        
        public ICollection<MiembroProyecto> MiembrosProyecto { get; set; } = new List<MiembroProyecto>();
        public ICollection<TareaItem> TareasAsignadas { get; set; } = new List<TareaItem>();
    }
}
