using Microsoft.EntityFrameworkCore;
using PetHealthAPI.Data;
using PetHealthAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(
        builder.Configuration.GetConnectionString("OracleConnection"),
        oracleOptions => oracleOptions.CommandTimeout(60)
    ));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Pet Health API",
        Version = "v1",
        Description = "API Restful para a plataforma Pet Health — cuidado contínuo e preventivo para pets. " +
                      "Gerencie tutores, pets, vacinas, consultas e medicamentos de forma organizada.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Pet Health — Challenge FIAP 2026",
            Email = "rm556649@fiap.com.br"
        }
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var maxTentativas = 15;
    for (var tentativa = 1; tentativa <= maxTentativas; tentativa++)
    {
        try
        {
            logger.LogInformation("Tentativa {Tentativa}/{Max} de conectar ao Oracle...", tentativa, maxTentativas);
            db.Database.EnsureCreated();
            logger.LogInformation("Schema garantido com sucesso.");
            break;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Banco ainda não está pronto: {Mensagem}", ex.Message);
            if (tentativa == maxTentativas)
            {
                logger.LogError("Falha definitiva ao conectar no banco após {Max} tentativas.", maxTentativas);
                throw;
            }
            Thread.Sleep(10_000);
        }
    }

    // Seed inicial — só insere se o banco estiver vazio
    if (db.Tutores.Count() == 0)
    {
        logger.LogInformation("Inserindo dados iniciais...");

        var t1 = new Tutor
        {
            Nome = "Maria Silva Santos",
            Email = "maria.silva@pethealth.com.br",
            Telefone = "(11) 98765-4321",
            Endereco = "Rua das Acacias, 250 - Vila Mariana, Sao Paulo - SP",
            DataCadastro = DateTime.Now
        };
        var t2 = new Tutor
        {
            Nome = "Joao Carlos Oliveira",
            Email = "joao.oliveira@pethealth.com.br",
            Telefone = "(11) 91234-5678",
            Endereco = "Av. Paulista, 1500 - Bela Vista, Sao Paulo - SP",
            DataCadastro = DateTime.Now
        };
        db.Tutores.AddRange(t1, t2);
        db.SaveChanges();

        db.Pets.AddRange(
            new Pet
            {
                Nome = "Thor",
                Especie = "Cachorro",
                Raca = "Golden Retriever",
                Idade = 4,
                Peso = 32.50m,
                Sexo = "Macho",
                Castrado = true,
                Alergias = "Alergia leve a frango",
                DataCadastro = DateTime.Now,
                TutorId = t1.Id
            },
            new Pet
            {
                Nome = "Luna",
                Especie = "Gato",
                Raca = "Siames",
                Idade = 2,
                Peso = 4.20m,
                Sexo = "Femea",
                Castrado = true,
                DataCadastro = DateTime.Now,
                TutorId = t2.Id
            }
        );
        db.SaveChanges();

        logger.LogInformation("Dados iniciais inseridos com sucesso.");
    }
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pet Health API v1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthorization();
app.MapControllers();

app.Run();
