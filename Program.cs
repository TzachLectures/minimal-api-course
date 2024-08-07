using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory list to store profiles
var profiles = new List<ColorCalibrationProfile>();

// Welcome message
app.MapGet("/", () => "Welcome to the Color Calibration API!");

// Retrieve all profiles
app.MapGet("/profiles", () => Results.Ok(profiles));

// Retrieve a specific profile by ID
app.MapGet("/profiles/{id}", (int id) =>
{
    var profile = profiles.Find(p => p.Id == id);
    return profile is not null ? Results.Ok(profile) : Results.NotFound();
});

// Create a new profile
app.MapPost("/profiles", (ColorCalibrationProfile profile) =>
{
    var newProfile = profile with { Id = profiles.Count > 0 ? profiles[^1].Id + 1 : 1 };
    profiles.Add(newProfile);
    return Results.Created($"/profiles/{newProfile.Id}", newProfile);
});


// Update an existing profile
app.MapPut("/profiles/{id}", (int id, ColorCalibrationProfile updatedProfile) =>
{
    var index = profiles.FindIndex(p => p.Id == id);
    if (index == -1) return Results.NotFound();

    profiles[index] = updatedProfile with { Id = id };
    return Results.Ok(updatedProfile);
});

// Delete a profile by ID
app.MapDelete("/profiles/{id}", (int id) =>
{
    var index = profiles.FindIndex(p => p.Id == id);
    if (index == -1) return Results.NotFound();

    profiles.RemoveAt(index);
    return Results.NoContent();
});

// Apply a profile
app.MapPost("/profiles/{id}/apply", (int id) =>
{
    var profile = profiles.Find(p => p.Id == id);
    if (profile is null) return Results.NotFound();

    // Simulate applying the profile to a machine
    profile.LastApplied = DateTime.UtcNow;
    return Results.Ok($"Profile {id} applied successfully.");
});

app.Run();

// Data model for Color Calibration Profile
public record ColorCalibrationProfile
{
    public int Id { get; init; }

    [Required]
    public string ProfileName { get; init; }

    [Required]
    public string MachineId { get; init; }

    public Dictionary<string, double> ColorSettings { get; init; } = new();

    public DateTime DateOfCalibration { get; init; }

    public DateTime? LastApplied { get; set; }
}
