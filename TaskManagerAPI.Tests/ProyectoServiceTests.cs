using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace TaskManagerAPI.Tests
{
    public class ProyectoServiceTests
    {
        private AppDbContext CrearContexto()
        {
            var options = new DbContextOptionsBuilder <AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CrearProyecto_DebeRetornarProyecto_CuandoDatosValidos()
        {
            //arrange
            var contexto = CrearContexto();
            var usuario = new Usuario { Id = 1, Nombre = "Facundo", Email = "facu@test.com", PasswordHash = "hash" };
            contexto.Usuarios.Add(usuario);
            await contexto.SaveChangesAsync();

            var service = new ProyectoService(contexto, NullLogger<ProyectoService>.Instance);
            var dto = new CrearProyectoDTO { Nombre = "Proyecto Test", Descripcion = "Descripcion" };

            //act
            var resultado = await service.CrearProyectoAsync(dto, usuarioId: 1);

            //assert
            Assert.NotNull(resultado);
            Assert.Equal("Proyecto Test", resultado.Nombre);
            Assert.Equal(1, resultado.PropietarioId);
        }

        [Fact]
        public async Task CrearProyecto_DebeAgregarPropietarioComoMiembro()
        {
            // Arrange
            var contexto = CrearContexto();
            var usuario = new Usuario { Id = 1, Nombre = "Facundo", Email = "facu@test.com", PasswordHash = "hash" };
            contexto.Usuarios.Add(usuario);
            await contexto.SaveChangesAsync();

            var service = new ProyectoService(contexto, NullLogger<ProyectoService>.Instance);
            var dto = new CrearProyectoDTO { Nombre = "Proyecto Test" };

            // Act
            await service.CrearProyectoAsync(dto, usuarioId: 1);

            // Assert
            var miembro = await contexto.MiembrosProyecto.FirstOrDefaultAsync();
            Assert.NotNull(miembro);
            Assert.Equal(1, miembro.UsuarioId);
        }

        [Fact]
        public async Task GetProyectosDelUsuario_DebeRetornarSoloProyectosDelUsuario()
        {
            // Arrange
            var contexto = CrearContexto();
            var usuario1 = new Usuario { Id = 1, Nombre = "Facundo", Email = "facu@test.com", PasswordHash = "hash" };
            var usuario2 = new Usuario { Id = 2, Nombre = "Otro", Email = "otro@test.com", PasswordHash = "hash" };
            contexto.Usuarios.AddRange(usuario1, usuario2);

            var proyecto1 = new Proyecto { Id = 1, Nombre = "Proyecto Facu", PropietarioId = 1, CreadoEn = DateTime.UtcNow };
            var proyecto2 = new Proyecto { Id = 2, Nombre = "Proyecto Otro", PropietarioId = 2, CreadoEn = DateTime.UtcNow };
            contexto.Proyectos.AddRange(proyecto1, proyecto2);
            await contexto.SaveChangesAsync();

            var service = new ProyectoService(contexto, NullLogger<ProyectoService>.Instance);
            var filtros = new ProyectoFiltrosDTO();

            // Act
            var resultado = await service.GetProyectosDelUsuarioAsync(usuarioId: 1, filtros);

            // Assert
            Assert.Single(resultado.Items);
            Assert.Equal("Proyecto Facu", resultado.Items[0].Nombre);
        }

        [Fact]
        public async Task EliminarProyecto_DebeRetornarFalse_CuandoUsuarioNoEsPropietario()
        {
            // Arrange
            var contexto = CrearContexto();
            var usuario1 = new Usuario { Id = 1, Nombre = "Facundo", Email = "facu@test.com", PasswordHash = "hash" };
            var usuario2 = new Usuario { Id = 2, Nombre = "Otro", Email = "otro@test.com", PasswordHash = "hash" };
            contexto.Usuarios.AddRange(usuario1, usuario2);

            var proyecto = new Proyecto { Id = 1, Nombre = "Proyecto Facu", PropietarioId = 1, CreadoEn = DateTime.UtcNow };
            contexto.Proyectos.Add(proyecto);
            await contexto.SaveChangesAsync();

            var service = new ProyectoService(contexto, NullLogger<ProyectoService>.Instance);

            // Act
            var resultado = await service.EliminarProyectoAsync(proyectoId: 1, usuarioId: 2);

            // Assert
            Assert.False(resultado);
        }

    }
}
