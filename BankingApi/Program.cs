using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ── PREPARAR: registrar el servicio de autenticación JWT ──
string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
    ?? throw new Exception("JWT_SECRET not set");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ── ARRANCAR: activar autenticación y autorización en el pipeline ──
app.UseAuthentication();   // ¿quién eres? (lee el token)
app.UseAuthorization();    // ¿puedes pasar? (revisa permisos)

app.MapControllers();

app.Run();