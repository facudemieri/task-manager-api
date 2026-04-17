using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerAPI.Data;
using TaskManagerAPI.DTOs;
using TaskManagerAPI.Models;
using TaskManagerAPI.Services;

namespace TaskManagerAPI.Tests
{
    public class TareaServiceTests
    {
        private AppDbContext CrearContexto()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private async Task<(AppDbContext contexto, Usuario usuario, Proyecto proyecto)> CrearEscenarioBase()
        {
            var contexto = CrearContexto();
            var usuario = new Usuario { Id = 1, Nombre = "Facundo", Email = "facu@test.com", PasswordHash = "hash" };
            contexto.Usuarios.Add(usuario);

            var proyecto = new Proyecto { Id = 1, Nombre = "Proyecto Test", PropietarioId = 1, CreadoEn = DateTime.UtcNow };
            contexto.Proyectos.Add(proyecto);

            var miembro = new MiembroProyecto { ProyectoId = 1, UsuarioId = 1, UnidoEn = DateTime.UtcNow };
            contexto.MiembrosProyecto.Add(miembro);

            await contexto.SaveChangesAsync();
            return (contexto, usuario, proyecto);
        }

        [Fact]
        public async Task CrearTarea_DebeRetornarTarea_CuandoUsuarioTieneAcceso()
        {
            // Arrange
            var (contexto, _, _) = await CrearEscenarioBase();
            var service = new TareaService(contexto);
            var dto = new CrearTareaDTO { Titulo = "Tarea Test", Descripcion = "Descripcion" };

            // Act
            var resultado = await service.CrearTareaAsync(proyectoId: 1, dto, usuarioId: 1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Tarea Test", resultado.Titulo);
            Assert.Equal("Pendiente", resultado.Estado);
        }

        [Fact]
        public async Task CrearTarea_DebeRetornarNull_CuandoUsuarioSinAcceso()
        {
            // Arrange
            var (contexto, _, _) = await CrearEscenarioBase();
            var service = new TareaService(contexto);
            var dto = new CrearTareaDTO { Titulo = "Tarea Test" };

            // Act
            var resultado = await service.CrearTareaAsync(proyectoId: 1, dto, usuarioId: 99);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task ActualizarEstado_DebeActualizarEstado_CuandoDatosValidos()
        {
            // Arrange
            var (contexto, _, _) = await CrearEscenarioBase();
            var tarea = new TareaItem { Id = 1, Titulo = "Tarea", ProyectoId = 1, Estado = EstadoTarea.Pendiente };
            contexto.Tareas.Add(tarea);
            await contexto.SaveChangesAsync();

            var service = new TareaService(contexto);
            var dto = new ActualizarEstadoDTO { Estado = "EnProgreso" };

            // Act
            var resultado = await service.ActualizarEstadoAsync(proyectoId: 1, tareaId: 1, dto, usuarioId: 1);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("EnProgreso", resultado.Estado);
        }

        [Fact]
        public async Task ActualizarEstado_DebeRetornarNull_CuandoEstadoInvalido()
        {
            // Arrange
            var (contexto, _, _) = await CrearEscenarioBase();
            var tarea = new TareaItem { Id = 1, Titulo = "Tarea", ProyectoId = 1, Estado = EstadoTarea.Pendiente };
            contexto.Tareas.Add(tarea);
            await contexto.SaveChangesAsync();

            var service = new TareaService(contexto);
            var dto = new ActualizarEstadoDTO { Estado = "EstadoInvalido" };

            // Act
            var resultado = await service.ActualizarEstadoAsync(proyectoId: 1, tareaId: 1, dto, usuarioId: 1);

            // Assert
            Assert.Null(resultado);
        }

        [Fact]
        public async Task GetTareas_DebeFiltrarPorEstado_CuandoEstadoEsValido()
        {
            // Arrange
            var (contexto, _, _) = await CrearEscenarioBase();
            contexto.Tareas.AddRange(
                new TareaItem { Id = 1, Titulo = "Tarea 1", ProyectoId = 1, Estado = EstadoTarea.Pendiente },
                new TareaItem { Id = 2, Titulo = "Tarea 2", ProyectoId = 1, Estado = EstadoTarea.EnProgreso },
                new TareaItem { Id = 3, Titulo = "Tarea 3", ProyectoId = 1, Estado = EstadoTarea.Pendiente }
            );
            await contexto.SaveChangesAsync();

            var service = new TareaService(contexto);
            var filtros = new TareaFiltrosDTO { Estado = "Pendiente" };

            // Act
            var resultado = await service.GetTareasDelProyectoAsync(proyectoId: 1, usuarioId: 1, filtros);

            // Assert
            Assert.Equal(2, resultado.TotalItems);
            Assert.All(resultado.Items, t => Assert.Equal("Pendiente", t.Estado));
        }

    }
}
