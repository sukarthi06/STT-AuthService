namespace AuthService.Api.Endpoints;

public static class AppAuthServices
{
    public static void MapAppAuthServicesEndpoints(this WebApplication app)
    {
        var serviceGroup = app.MapGroup("/api/authservice")
            .WithTags("App Auth Service Endpoints");

        serviceGroup.MapPost("/login", LoginAsync)
            .WithSummary("Authenticates a user.")
            .WithDescription("Authenticates a user with the provided credentials." +
                " Returns an access token if successful.")
            .Produces<LoginResponseDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> LoginAsync(
        [FromBody] LoginDto loginDto,
        IValidator<LoginDto> validator,
        LoginHandler handler,
        CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(loginDto, ct);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());
        var token = await handler.HandleAsync(loginDto, ct);
        return token is null
            ? Results.Unauthorized()
            : Results.Ok(token);
    }
}
