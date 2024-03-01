using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using System.Collections.Generic;
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
            using StreamReader reader = new(context.Request.Body);
            var bodyAsString = await reader.ReadToEndAsync();

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
                Type = exception is SecurityException ? "Security" :"Exception",
                Data = new 
                    { Message = exception is SecurityException
                        ? exception.Message
                        : $"Internal server error ID = {exceptionRecord.Entity.Id}"
                    }
            });
        }
    }
}