using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.IO;

namespace WebAPI.Lib
{
    public class ExceptionMiddleware
    {

        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (!Directory.Exists("log"))
                {
                    Directory.CreateDirectory("log");
                }
                using StreamWriter file = new(@$"log\{DateTime.Now.ToString("yyyyMMdd")}.txt", append: true);
                await file.WriteLineAsync(ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync($"{GetType().Name} catch exception. Message: {ex.Message}");
            }
        }
    }
}