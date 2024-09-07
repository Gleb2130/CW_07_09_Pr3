var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IMathService, MathService>(); // Singleton
builder.Services.AddTransient<IUpperCaseService, UpperCaseService>(); // Transient
builder.Services.AddScoped<ILengthService, LengthService>(); // Scoped

var app = builder.Build();

app.UseMiddleware<ServiceMiddleware>();

app.Run();

public interface IMathService
{
    int Add(int a, int b);
}

public class MathService : IMathService
{
    public int Add(int a, int b) => a + b;
}

public interface IUpperCaseService
{
    string ToUpperCase(string input);
}

public class UpperCaseService : IUpperCaseService
{
    public string ToUpperCase(string input) => input.ToUpper();
}

public interface ILengthService
{
    int GetLength(string input);
}

public class LengthService : ILengthService
{
    public int GetLength(string input) => input.Length;
}


public class ServiceMiddleware
{
    private readonly RequestDelegate _next;

    public ServiceMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IMathService mathService, IUpperCaseService upperCaseService, ILengthService lengthService)
    {
        var sum = mathService.Add(5, 10);
        var upperCase = upperCaseService.ToUpperCase("hello");
        var length = lengthService.GetLength("hello");

        var resultHtml = $@"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>Service Results</title>
        </head>
        <body>
            <h1>Service Results</h1>
            <p><strong>Sum:</strong> {sum}</p>
            <p><strong>Upper Case:</strong> {upperCase}</p>
            <p><strong>Length:</strong> {length}</p>
        </body>
        </html>";


        context.Response.ContentType = "text/html;charset=utf-8";
        await context.Response.WriteAsync(resultHtml);

        await _next(context);
    }
}
