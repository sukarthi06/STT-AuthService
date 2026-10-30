namespace AuthService.Api.Endpoints;

public static class AppUserServices
{
    public static void MapAppUserServicesEndpoints(this WebApplication app)
    {
        var serviceGroup = app.MapGroup("/api/userservice")
            .WithTags("App User Service Endpoints");

        serviceGroup.MapPost("/register", RegisterAsync)
            .WithSummary("Registers a new user.")
            .WithDescription("Registers a new user with the provided registration details." +
                " Returns the UserId of the newly created user if successful.")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces<string>(StatusCodes.Status409Conflict)
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
            : Results.Created($"/api/userservice/{userId.Value}", new { UserId = userId.Value });
    }
}
