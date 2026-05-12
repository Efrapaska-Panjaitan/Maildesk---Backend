using Microsoft.EntityFrameworkCore;
using MailDesk.Api.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapControllers();

// SEEDING DATA MANUAL DARI FILE dummy.sql
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // 1. Ngepel lantai: Hapus semua data surat & kembalikan ID-nya mulai dari 1 lagi
    db.Database.ExecuteSqlRaw("TRUNCATE TABLE disposisi_relation, disposisi, inbox, surat RESTART IDENTITY CASCADE;");

    // 2. Cek apakah file dummy.sql ada di folder project?
    if (File.Exists("dummy.sql"))
    {
        // 3. Kalau ada, baca isinya dan eksekusi ke database
        var sqlText = File.ReadAllText("dummy.sql");
        if (!string.IsNullOrWhiteSpace(sqlText))
        {
            db.Database.ExecuteSqlRaw(sqlText);
        }
    }
}

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
