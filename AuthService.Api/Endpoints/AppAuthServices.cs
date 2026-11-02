namespace AuthService.Api.Endpoints;

public static class AppAuthServices
{
    private const string RefreshCookieName = "stt_refresh";
    private const string RefreshCookiePath = "/api/auth";

    public static void MapAppAuthServicesEndpoints(this WebApplication app)
    {
        var serviceGroup = app.MapGroup("/api/auth")
            .WithTags("App Auth Service Endpoints");

        serviceGroup.MapPost("/login", LoginAsync)
            .WithSummary("Authenticates a user.")
            .WithDescription("Authenticates a user with the provided credentials." +
                " Returns an access token and sets the refresh token cookie if successful.")
            .Produces<LoginResponseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        serviceGroup.MapPost("/refresh", RefreshAsync)
            .WithSummary("Refreshes the session.")
            .WithDescription("Uses the refresh token cookie to issue a new access token and rotate the refresh token.")
            .Produces<LoginResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        serviceGroup.MapPost("/logout", LogoutAsync)
            .WithSummary("Logs out the user.")
            .WithDescription("Revokes the refresh token session and clears the cookie.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginDto loginDto,
        IValidator<LoginDto> validator,
        LoginHandler handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(loginDto, ct);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var session = await handler.HandleAsync(loginDto, ct);
        if (session is null)
            return Results.Unauthorized();

        SetRefreshCookie(httpContext.Response, session);
        return Results.Ok(new LoginResponseDto(session.AccessToken));
    }

    private static async Task<IResult> RefreshAsync(
        RefreshHandler handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var rawToken = httpContext.Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(rawToken))
            return Results.Unauthorized();

        var session = await handler.HandleAsync(rawToken, ct);
        if (session is null)
        {
            ClearRefreshCookie(httpContext.Response);
            return Results.Unauthorized();
        }

        SetRefreshCookie(httpContext.Response, session);
        return Results.Ok(new LoginResponseDto(session.AccessToken));
    }

    private static async Task<IResult> LogoutAsync(
        LogoutHandler handler,
        HttpContext httpContext,
        CancellationToken ct)
    {
        var rawToken = httpContext.Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrEmpty(rawToken))
            await handler.HandleAsync(rawToken, ct);

        ClearRefreshCookie(httpContext.Response);
        return Results.NoContent();
    }

    private static void SetRefreshCookie(HttpResponse response, AuthSession session) =>
        response.Cookies.Append(RefreshCookieName, session.RefreshToken, CreateCookieOptions(session.RefreshTokenExpiresAt));

    private static void ClearRefreshCookie(HttpResponse response) =>
        response.Cookies.Delete(RefreshCookieName, CreateCookieOptions(null));

    private static CookieOptions CreateCookieOptions(DateTime? expiresAt) => new()
    {
        HttpOnly = true,                // not readable by JavaScript
        Secure = true,                  // HTTPS only (browsers allow it on localhost)
        SameSite = SameSiteMode.Strict, // not sent on cross-site requests (CSRF)
        Path = RefreshCookiePath,       // only sent to /api/auth/*
        Expires = expiresAt,
        IsEssential = true
    };
}