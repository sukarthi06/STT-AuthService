namespace AuthService.Api.Endpoints;

public static class AppUserServices
{
    public static void MapAppUserServicesEndpoints(this WebApplication app)
    {
        var serviceGroup = app.MapGroup("/api/user")
            .WithTags("App User Service Endpoints");

        serviceGroup.MapPost("/register", RegisterAsync)
            .WithSummary("Registers a new user.")
            .WithDescription("Registers a new user with the provided registration details." +
                " Returns the UserId of the newly created user if successful.")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces<string>(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        serviceGroup.MapGet("/me", GetCurrentUserAsync)
            .WithSummary("Gets the current user.")
            .WithDescription("Returns the profile of the authenticated user.")
            .Produces<UserResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> RegisterAsync(
    [FromBody] RegistrationDto registrationDto,
    IValidator<RegistrationDto> validator,
    RegistrationHandler handler,
    CancellationToken ct)
    {
        var validation = await validator.ValidateAsync(registrationDto, ct);
        if (!validation.IsValid)
            return Results.ValidationProblem(validation.ToDictionary());

        var userId = await handler.HandleAsync(registrationDto, ct);
        return userId is null
            ? Results.Conflict("Email already registered.")
            : Results.Created($"/api/user/{userId.Value}", new { UserId = userId.Value });
    }

    private static async Task<IResult> GetCurrentUserAsync(
    [FromHeader(Name = "X-User-Id")] string? userIdHeader,
    GetUserHandler handler,
    CancellationToken ct)
    {
        if (!Guid.TryParse(userIdHeader, out var id) || UserId.Empty(id))
            return Results.Unauthorized();

        var user = await handler.HandleAsync(UserId.Of(id), ct);
        return user is null
            ? Results.NotFound()
            : Results.Ok(user);
    }
}
