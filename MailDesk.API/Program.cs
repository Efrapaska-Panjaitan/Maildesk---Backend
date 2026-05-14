using MailDesk.API.Data;
using MailDesk.API.Services;
using MailDesk.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISuratService, SuratService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDisposisiService, DisposisiService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();

// ── Swagger + Definisi X-User-Id (RBAC) ──────────────────────────────────
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title   = "MailDesk API",
        Version = "v1",
        Description = "API Manajemen Surat MailDesk — Gunakan tombol Authorize dan masukkan User ID Anda."
    });

    // Definisi header X-User-Id untuk otorisasi berbasis role
    c.AddSecurityDefinition("X-User-Id", new OpenApiSecurityScheme
    {
        Description = "Masukkan ID user: 1=Admin, 2=TU, 3=Sekretaris, 4=Pimpinan (read-only), 5=User (read-only)",
        Name        = "X-User-Id",
        In          = ParameterLocation.Header,
        Type        = SecuritySchemeType.ApiKey,
        Scheme      = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "X-User-Id"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MailDesk.API v1");
    });
}

app.UseHttpsRedirection();

// ── Serve static files (PDF uploads) ─────────────────────────────────────
app.UseStaticFiles();

// ── RBAC Middleware — validasi X-User-Id di setiap request /api ──────────
// Role mapping: 1=Admin, 2=TU, 3=Sekretaris, 4=Pimpinan, 5=User
// Pimpinan (4) dan User (5) hanya boleh akses GET (read-only)
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    if (path != null && path.StartsWith("/api"))
    {
        // 1. Header X-User-Id wajib ada
        if (!context.Request.Headers.TryGetValue("X-User-Id", out var userIdValue) ||
            string.IsNullOrWhiteSpace(userIdValue))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Unauthorized. Header X-User-Id wajib diisi. Gunakan tombol Authorize di Swagger."
            });
            return;
        }

        // 2. X-User-Id harus berupa angka
        if (!int.TryParse(userIdValue, out var userId))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Header X-User-Id harus berupa angka. Contoh: 1"
            });
            return;
        }

        // 3. User ID harus terdaftar (valid role)
        var validUserIds = new[] { 1, 2, 3, 4, 5 };
        if (!validUserIds.Contains(userId))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Forbidden. User ID tidak memiliki role yang valid (1–5)."
            });
            return;
        }

        // 4. Pimpinan (4) dan User (5) hanya boleh GET (read-only)
        var readOnlyRoles = new[] { 4, 5 };
        if (readOnlyRoles.Contains(userId) && !HttpMethods.IsGet(context.Request.Method))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message = "Forbidden. Role Anda hanya diizinkan untuk membaca data (GET)."
            });
            return;
        }
    }

    await next();
});

app.UseAuthorization();
app.MapControllers();
app.Run();