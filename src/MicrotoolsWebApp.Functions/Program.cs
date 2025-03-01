using System;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MicrotoolsWebApp.Core;
using MicrotoolsWebApp.Core.Models;

// Create and initialize the registry
var microtoolsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "microtools");
var registry = new MicrotoolRegistry(microtoolsDirectory);

try
{
    registry.DiscoverMicrotools();
}
catch (Exception ex)
{
    Console.WriteLine($"Error discovering microtools: {ex.Message}");
}

// Set the port to 7071
Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://localhost:7071");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton(registry);
builder.Services.AddSingleton<MicrotoolLoader>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

// Configure JSON serialization options
var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
};

// Define API endpoints
app.MapGet("/api/microtools", async (HttpContext context) =>
{
    var microtools = registry.GetAllMicrotools().Select(m => new
    {
        id = m.Metadata.Id,
        name = m.Metadata.Name,
        description = m.Metadata.Description,
        version = m.Metadata.Version,
        author = m.Metadata.Author,
        tags = m.Metadata.Tags
    });

    await context.Response.WriteAsJsonAsync(microtools, jsonOptions);
});

app.MapGet("/api/microtools/{id}", async (HttpContext context, string id) =>
{
    var microtool = registry.GetMicrotoolById(id);
    if (microtool == null)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync($"Microtool with ID '{id}' not found");
        return;
    }

    var result = new
    {
        id = microtool.Metadata.Id,
        name = microtool.Metadata.Name,
        description = microtool.Metadata.Description,
        version = microtool.Metadata.Version,
        author = microtool.Metadata.Author,
        tags = microtool.Metadata.Tags,
        uiSchema = microtool.UiSchema
    };

    await context.Response.WriteAsJsonAsync(result, jsonOptions);
});

app.MapPost("/api/microtools/{id}/execute", async (HttpContext context, string id) =>
{
    var microtool = registry.GetMicrotoolById(id);
    if (microtool == null)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsync($"Microtool with ID '{id}' not found");
        return;
    }

    try
    {
        // Load the microtool instance if not already loaded
        if (microtool.Instance == null)
        {
            var loader = app.Services.GetRequiredService<MicrotoolLoader>();
            microtool.Instance = loader.LoadMicrotoolFromDirectory(microtool.DirectoryPath, id);
            
            if (microtool.Instance == null)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync($"Failed to load microtool with ID '{id}'");
                return;
            }
        }

        // Parse the request body to get the parameters
        var parameters = await JsonSerializer.DeserializeAsync<Dictionary<string, object>>(
            context.Request.Body, jsonOptions);

        // Execute the microtool
        var result = await microtool.Instance.ExecuteAsync(parameters ?? new Dictionary<string, object>());

        // Return the result
        await context.Response.WriteAsJsonAsync(result, jsonOptions);
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync($"Error executing microtool: {ex.Message}");
    }
});

Console.WriteLine("Starting API server on http://localhost:7071");
app.Run();
