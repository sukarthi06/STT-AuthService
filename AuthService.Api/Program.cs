using AuthService.Api;
using AuthService.Api.Endpoints;
using AuthService.Application;
using AuthService.Infra;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.AddHostInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

if (!builder.Environment.IsEnvironment("Local"))
{
    builder.Services.AddObservability(builder.Configuration);
}

builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();//Always keep this first in the pipeline to catch all exceptions

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowedClientOrigins");

#region Endpoints

app.MapGet("/health", () => Results.Ok());

app.MapAppUserServicesEndpoints();
app.MapWellKnownEndpoints();
app.MapAppAuthServicesEndpoints();

#endregion

app.Run();