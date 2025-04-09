using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RapidPay.Application.Interfaces;
using RapidPay.Application.Services;
using RapidPay.Infrastructure.Persistence;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        AddAuth(builder);
        AddDbContext(builder);
        AddServices(builder);

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void AddAuth(WebApplicationBuilder builder)
    {
        // Read JwtSettings from appsettings.json
        IConfigurationSection jwtSettings = builder.Configuration.GetSection("JwtSettings");
        string? secretKey = jwtSettings["SecretKey"];
        string? issuer = jwtSettings["Issuer"];
        string? audience = jwtSettings["Audience"];

        // Add JWT Authentication
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
                };
            });
    }

    private static void AddServices(WebApplicationBuilder builder)
    {
        // Add application services
        builder.Services.AddScoped<ICardService, CardService>();

        // Register FeeService as a Singleton implementing IFeeService
        builder.Services.AddSingleton<IFeeService>(FeeService.Instance);
    }

    private static void AddDbContext(WebApplicationBuilder builder)
    {
        // Add database context
        builder.Services.AddDbContext<RapidPayDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("RapidPayDb")));
    }
}