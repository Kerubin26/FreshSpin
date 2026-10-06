using System.Text;
using FreshSpinApi.Data;
using FreshSpinApi.Models;
using FreshSpinApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<FreshSpinDbContext>(options =>
    options.UseInMemoryDatabase("FreshSpinDb"));
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddSingleton<PasswordHasher<AppUser>>();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FreshSpinDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<PasswordHasher<AppUser>>();

    void AddUser(string userId, string fullName, string username, string password, string role)
    {
        if (db.Users.Any(u => u.Username == username)) return;
        var user = new AppUser
        {
            UserId = userId,
            FullName = fullName,
            Username = username,
            Role = role
        };
        user.PasswordHash = hasher.HashPassword(user, password);
        db.Users.Add(user);
    }

    AddUser("ADMIN-001", "FreshSpin Administrator", "admin", "Admin123!", "Admin");
    AddUser("STAFF-001", "FreshSpin Staff", "staff", "Staff123!", "Staff");
    AddUser("CUST-001", "Sample Customer", "customer", "Customer123!", "Customer");
    db.SaveChanges();

    if (!db.Bookings.Any())
    {
        db.Bookings.Add(new Booking
        {
            CustomerId = "CUST-001",
            ServiceType = "Wash & Fold",
            WeightKg = 5,
            TotalCost = 250,
            PickupAddress = "Sample Address, Manila",
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        db.SaveChanges();
    }
}

app.Run();
