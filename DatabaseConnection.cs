using MongoDB.Driver;
using MongoDB.Bson;

public class DatabaseConnection
{
    private readonly IMongoDatabase _database;

    public DatabaseConnection(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<BsonDocument> GetCollection(string collectionName)
    {
        return _database.GetCollection<BsonDocument>(collectionName);
    }
}
