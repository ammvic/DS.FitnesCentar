using FitnessCentar.Email.Persistence;
using FitnessCentar.Email.Services;
using Neo4j.Driver;

var builder = WebApplication.CreateBuilder(args);

// Registracija Neo4j drajvera
builder.Services.AddSingleton<IDriver>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return GraphDatabase.Driver(
        configuration["Neo4j:Uri"],
        AuthTokens.Basic(configuration["Neo4j:Username"], configuration["Neo4j:Password"])
    );
});

// Registracija EmailDbContext
builder.Services.AddSingleton<EmailDbContext>();

// Registracija servisa
builder.Services.AddSingleton<EmailService>();
builder.Services.AddSingleton<RabbitMqListener>();

// Registracija kontrolera
builder.Services.AddControllers();

// Registracija Swagger servisa
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registracija autorizacije
builder.Services.AddAuthorization();

var app = builder.Build();
var rabbitMqListener = app.Services.GetRequiredService<RabbitMqListener>();
rabbitMqListener.StartListening();

// Konfiguriši HTTP zahtevni pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uklanjanje HTTPS redirekcije
// app.UseHttpsRedirection(); // Ovu liniju možete ukloniti

app.UseAuthorization();

app.MapControllers();

app.Run();
