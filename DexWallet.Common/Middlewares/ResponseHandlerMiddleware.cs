using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace DexWallet.Common.Middlewares;

public class ResponseHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        string jsonResponse;

        await using var buffer = new MemoryStream();
        using var bufferReader = new StreamReader(buffer);
        var stream = response.Body;
        response.Body = buffer;

        try
        {
            await _next(context);

            buffer.Seek(0, SeekOrigin.Begin);
            var body = await bufferReader.ReadToEndAsync();


            var respObj = string.IsNullOrEmpty(body) ? body : JsonSerializer.Deserialize<object>(body);
            jsonResponse = JsonSerializer.Serialize(new AppResponse(respObj), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
        catch (Exception exception)
        {
            response.StatusCode = exception switch
            {
                AppException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            jsonResponse = JsonSerializer.Serialize(new AppResponse(null, false, exception.Message), new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        buffer.Seek(0, SeekOrigin.Begin);
        await response.WriteAsync(jsonResponse);

        response.Body.Seek(0, SeekOrigin.Begin);
        await response.Body.CopyToAsync(stream);
    }
}