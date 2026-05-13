using MailDesk.API.Data;
using MailDesk.API.Services;
using MailDesk.API.Services.Interfaces;
using MailDesk.API.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Database ────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Services ────────────────────────────────────────────────────────────────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISuratService, SuratService>();
builder.Services.AddScoped<IDisposisiService, DisposisiService>();

// ── Controllers & Swagger ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() 
    { 
        Title = "MailDesk API", 
        Version = "v1",
        Description = "Sistem Persuratan Digital — Sprint 1"
    });
    // Include XML comments untuk dokumentasi endpoint
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);

    //IFormFile
    c.OperationFilter<SwaggerFileOperationFilter>();
});

var app = builder.Build();

// ── Middleware ───────────────────────────────────────────────────────────────
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();