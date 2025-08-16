
namespace Real_Time_Chat_Backend.MiddleWares
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        public AuthorizationMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var exp = context.User.FindFirst("exp")?.Value;

                if (long.TryParse(exp, out var expiryUnix))
                {
                    var expiryTime = DateTimeOffset.FromUnixTimeSeconds(expiryUnix).UtcDateTime;
                    if (expiryTime < DateTime.UtcNow)
                    {
                        Console.WriteLine("⚠️ Token expired!");
                    }
                    else
                    {
                        Console.WriteLine($"Token valid until: {expiryTime}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No Token Injected");
            }

            await _next(context);
        }
    }
}
