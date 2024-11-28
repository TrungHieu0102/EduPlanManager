using EduPlanManager.Services.Interface;

public class TokenRenewalMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TokenRenewalMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            var accessToken = context.Request.Cookies["AccessToken"];
            var refreshToken = context.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                await _next(context);  
                return;
            }

            if (tokenService.IsAccessTokenExpired(accessToken))
            {
                var newAccessToken = await tokenService.RefreshAccessTokenAsync(refreshToken);

                if (newAccessToken != null)
                {
                    context.Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        Expires = DateTime.Now.AddMinutes(30)
                    });
                }
            }

            await _next(context);
        }
    }
}
