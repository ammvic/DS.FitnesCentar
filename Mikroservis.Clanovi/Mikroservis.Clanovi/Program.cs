using FitnessCentar.Members.Interface;
using FitnessCentar.Members.Persistence;
using FitnessCentar.Members.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MembersDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!builder.Environment.IsDevelopment())
    {
        var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
        connectionString = string.Format(connectionString, password);
    }
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    });
});

builder.Services.AddSingleton<IMessageBroker, RabbitMqMessageBroker>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Dodavanje inicijalizacije baze sa retry logikom
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var maxRetries = 10;
    var retry = 0;
    
    while (retry < maxRetries)
    {
        try
        {
            Console.WriteLine($"Pokušaj {retry + 1} od {maxRetries} za inicijalizaciju baze...");
            var context = services.GetRequiredService<MembersDbContext>();
            context.Database.Migrate();
            Console.WriteLine("Uspešna inicijalizacija baze i primena migracija.");
            break;
        }
        catch (Exception ex)
        {
            retry++;
            if (retry == maxRetries)
            {
                Console.WriteLine($"Greška prilikom inicijalizacije baze nakon {maxRetries} pokušaja: {ex.Message}");
                throw;
            }
            
            Console.WriteLine($"Pokušaj {retry} nije uspeo. Čekanje 5 sekundi pre sledećeg pokušaja...");
            Thread.Sleep(5000);
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();