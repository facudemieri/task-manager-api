using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaskManagerAPI.Exceptions;

namespace TaskManagerAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                await WriteProblemDetails(context, StatusCodes.Status404NotFound, ex.Message);
            }
            catch(BadRequestException ex)
            {
                _logger.LogWarning(ex.Message);
                await WriteProblemDetails(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Inesperado");
                await WriteProblemDetails(context, StatusCodes.Status500InternalServerError, "Ocurrió un error interno");

            }
        
        
        }

        private async Task WriteProblemDetails(HttpContext context, int statusCode, string detail)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Detail = detail
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
