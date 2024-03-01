using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using System.Collections.Generic;
using System.Text;
using WebApplication2.Exceptions;

namespace WebApplication2.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            string bodyAsString = await GetBody(context);

            var exceptionRecord = dbContext.ExceptionRecords.Add(new ExceptionRecord
            {
                StackTrace = exception.StackTrace,
                BodyParameters = bodyAsString,
                QueryParameters = string.Join(Environment.NewLine, context.Request.Query),
                Timestamp = DateTime.Now.ToUniversalTime()
            });

            await dbContext.SaveChangesAsync();

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                Id = exceptionRecord.Entity.Id,
                Type = exception is SecurityException ? "Security" : "Exception",
                Data = new
                {
                    Message = exception is SecurityException
                        ? exception.Message
                        : $"Internal server error ID = {exceptionRecord.Entity.Id}"
                }
            });
        }
    }

    private static async Task<string> GetBody(HttpContext context)
    {
        context.Request.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();

        context.Request.Body.Seek(0, SeekOrigin.Begin);

        return body;
    }
}