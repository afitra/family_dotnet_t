using System.Security.Claims;
using familyMart.Helpers;
using familyMart.Master;

namespace familyMart.Middleware;

public class JwtAuthenticationMiddleware
{
    protected readonly BasicLogger _basicLogger;
    private readonly RequestDelegate _next;
    protected string _source;

    public JwtAuthenticationMiddleware()
    {
    }

    public JwtAuthenticationMiddleware(BasicLogger basicLogger, RequestDelegate next)
    {
        _source = GetType().Name;
        _basicLogger = basicLogger;
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var headerValue = context.Request.Headers["Authorization"].ToString();

            if (headerValue.StartsWith("Bearer "))
            {
                var token = headerValue.Substring("Bearer ".Length);

                if (Helper.VerifyToken(token, out var payload))
                {
                    var claims = new List<Claim>();

                    // Assuming 'payload' is an ExpandoObject
                    foreach (var property in ((IDictionary<string, object>)payload))
                    {
                        claims.Add(new Claim(property.Key, property.Value.ToString()));
                    }

                    var identity = new ClaimsIdentity(claims, "custom", ClaimTypes.Email, ClaimTypes.Name);
                    var principal = new ClaimsPrincipal(identity);

                    context.User = principal;

                    _basicLogger.Log("Information", "Information", _source, "JwtPayload", payload);
                    await _next(context);
                    return;
                }
            }
        }

        context.Response.StatusCode = 401;
        context.Response.ContentType = "application/json";

        var response = new GlobalResponse
        {
            Code = BasicCode.UnauthorizedErrorCode,
            Status = false,
            Title = "Unauthorized",
            Message = BasicMessage.AuthErrorMessage,
            Data = null
        };

        await context.Response.WriteAsJsonAsync(response);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<JwtAuthenticationMiddleware>();
    }
}

public static class JwtAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtAuthenticationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtAuthenticationMiddleware>();
    }
}