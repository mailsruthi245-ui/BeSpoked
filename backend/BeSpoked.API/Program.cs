using BeSpoked.API;
using BeSpoked.API.Settings;
using BeSpoked.Core.Interfaces;
using BeSpoked.Infrastructure.Data;
using BeSpoked.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<QuarterlyBonusSettings>(
    builder.Configuration.GetSection(QuarterlyBonusSettings.Section));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddCors(options =>
    options.AddPolicy("ReactApp", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Retry loop: SQL Server takes ~20s to start in Docker
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    for (var attempt = 1; attempt <= 10; attempt++)
    {
        try { db.Database.Migrate(); break; }
        catch when (attempt < 10) { Thread.Sleep(3000); }
    }
    await DataSeeder.SeedAsync(scope.ServiceProvider);
}

app.UseCors("ReactApp");
app.UseAuthorization();
app.MapControllers();
app.Run();
