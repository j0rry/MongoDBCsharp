using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        var connectionString = "mongodb://localhost:27017";
        var databaseName = "appDB";
        var dbConnection = new DatabaseConnection(connectionString, databaseName);
        var usersCollection = dbConnection.GetCollection("users");

        Console.WriteLine("1. Register");
        Console.WriteLine("2. Login");
        Console.WriteLine("3. Display All Users");
        Console.Write("Choose an option: ");
        var choice = Console.ReadLine();

        if (choice == "1")
        {
            await Register(usersCollection);
            await Login(usersCollection);
        }
        else if (choice == "2")
        {
            await Login(usersCollection);
        }
        else if (choice == "3")
        {
            await DisplayAllUsers(usersCollection);
        }
        else
        {
            Console.WriteLine("Invalid choice. Try Again...");
        }
        Console.ReadLine();
    }

    static async Task Register(IMongoCollection<BsonDocument> usersCollection)
    {
        Console.Write("Enter username: ");
        var username = Console.ReadLine();
        Console.Write("Enter password: ");
        var password = ReadPassword();

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var userDocument = new BsonDocument
        {
            { "username", username },
            { "password", hashedPassword }
        };

        await usersCollection.InsertOneAsync(userDocument);
        Console.WriteLine("User registered successfully.");
    }

    static async Task Login(IMongoCollection<BsonDocument> usersCollection)
    {
        Console.Write("Enter username: ");
        var username = Console.ReadLine();
        Console.Write("Enter password: ");
        var password = ReadPassword();

        var filter = Builders<BsonDocument>.Filter.Eq("username", username);
        var user = await usersCollection.Find(filter).FirstOrDefaultAsync();

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user["password"].AsString))
        {
            Console.WriteLine("Invalid credentials. Exiting...");
            return;
        }

        Console.WriteLine("Login successful.");
    }

    static async Task DisplayAllUsers(IMongoCollection<BsonDocument> usersCollection)
    {
        var users = await usersCollection.Find(new BsonDocument()).ToListAsync();
        foreach (var user in users)
        {
            Console.WriteLine(user.ToString());
        }
    }

    static string ReadPassword()
    {
        var password = new StringBuilder();
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                break;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
            }
            else
            {
                password.Append(key.KeyChar);
                Console.Write("*");
            }
        }
        return password.ToString();
    }
}
