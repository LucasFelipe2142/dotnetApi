using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using blazorback.Data;
using blazorback.Mock.Data;

public class MongoDBContext
{
    private readonly IMongoDatabase _database;

    public MongoDBContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDB"));
        _database = client.GetDatabase("dotnet");

        var usuariosCollection = _database.GetCollection<Usuario>("Usuarios");
        var emailIndexDefinition = Builders<Usuario>.IndexKeys.Ascending(u => u.Email);
        var emailIndexOptions = new CreateIndexOptions { Unique = true };
        var emailIndexModel = new CreateIndexModel<Usuario>(emailIndexDefinition, emailIndexOptions);
        usuariosCollection.Indexes.CreateOne(emailIndexModel);

        var atividadesCollection = _database.GetCollection<AtividadeBD>("Atividades");
    }

    public IMongoCollection<Usuario> Usuarios => _database.GetCollection<Usuario>("Usuarios");
    public IMongoCollection<AtividadeBD> Atividades => _database.GetCollection<AtividadeBD>("Atividades");
}
