using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Security.Cryptography;

public static class MongoService
{
    private static IMongoCollection<BsonDocument> collection;
    public static string CurrentProfile { get; private set; }
    public static int BestScore { get; private set; }

    public static void InitializeMongo(string connectionString = "mongodb://localhost:27017")
    {
        try
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase("game");
            collection = db.GetCollection<BsonDocument>("Profiles");
            Console.WriteLine("Connexion à MongoDB réussie.");
        }
        catch (MongoConnectionException ex)
        {
            Console.WriteLine("Erreur de connexion à MongoDB : " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de l'initialisation de MongoDB : " + ex.Message);
        }
    }

    public static bool CreateProfile(string profileName, string password)
    {
        try
        {
            if (collection == null)
            {
                Console.WriteLine("MongoDB non initialisé.");
                return false;
            }

            if (collection.Find(Builders<BsonDocument>.Filter.Eq("_id", profileName)).Any())
            {
                Console.WriteLine("Profil déjà existant.");
                return false;
            }

            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] hash = HashPassword(password, salt);

            var doc = new BsonDocument
            {
                { "_id", profileName },
                { "PasswordHash", Convert.ToBase64String(hash) },
                { "Salt", Convert.ToBase64String(salt) },
                { "BestScore", 0 }
            };

            collection.InsertOne(doc);
            Console.WriteLine("Profil créé avec succès.");
            return true;
        }
        catch (MongoException ex)
        {
            Console.WriteLine("Erreur MongoDB lors de la création du profil : " + ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de la création du profil : " + ex.Message);
            return false;
        }
    }

    public static bool Login(string profileName, string password)
    {
        try
        {
            if (collection == null)
            {
                Console.WriteLine("MongoDB non initialisé.");
                return false;
            }

            var doc = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", profileName)).FirstOrDefault();
            if (doc == null)
            {
                Console.WriteLine("Profil non trouvé.");
                return false;
            }

            byte[] storedHash = Convert.FromBase64String(doc["PasswordHash"].AsString);
            byte[] salt = Convert.FromBase64String(doc["Salt"].AsString);
            byte[] hash = HashPassword(password, salt);

            if (!CryptographicOperations.FixedTimeEquals(hash, storedHash))
            {
                Console.WriteLine("Mot de passe incorrect.");
                return false;
            }

            CurrentProfile = profileName;
            BestScore = doc["BestScore"].ToInt32();

            Console.WriteLine("Connexion réussie.");
            return true;
        }
        catch (MongoException ex)
        {
            Console.WriteLine("Erreur MongoDB lors de la connexion : " + ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de la connexion : " + ex.Message);
            return false;
        }
    }

    public static void UpdateBestScore(int currentScore)
    {
        if (CurrentProfile == null || collection == null) return;

        try
        {
            if (currentScore > BestScore)
            {
                BestScore = currentScore;
                var filter = Builders<BsonDocument>.Filter.Eq("_id", CurrentProfile);
                var update = Builders<BsonDocument>.Update.Set("BestScore", BestScore);
                collection.UpdateOne(filter, update);
            }
        }
        catch (MongoException ex)
        {
            Console.WriteLine("Erreur MongoDB lors de la mise à jour du score : " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de la mise à jour du score : " + ex.Message);
        }
    }

    public static void ShowLeaderboard(int top = 10)
    {
        if (collection == null)
        {
            Console.WriteLine("MongoDB non initialisé.");
            return;
        }

        try
        {
            var sort = Builders<BsonDocument>.Sort.Descending("BestScore");
            var topPlayers = collection.Find(new BsonDocument())
                                       .Sort(sort)
                                       .Limit(top)
                                       .ToList();

            Console.WriteLine("===== LEADERBOARD =====");
            int rank = 1;
            foreach (var player in topPlayers)
            {
                string name = player["_id"].AsString;
                int score = player["BestScore"].ToInt32();
                Console.WriteLine($"{rank}. {name} - {score} pts");
                rank++;
            }
            Console.WriteLine("========================");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de l'affichage du leaderboard : " + ex.Message);
        }
    }

    private static byte[] HashPassword(string password, byte[] salt, int iterations = 100000, int length = 32)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);
        return pbkdf2.GetBytes(length);
    }
}
