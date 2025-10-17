using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;

class Save
{
    // ==== Données de jeu ====
    public static int room = 0;
    public static int level = 1;
    public static int PlayerHp = 100;
    public static int MonsterHp = 0;
    public static int MonsterAttack = 0;
    public static int score = 0;
    public static int bestscore = 0;
    public static Dictionary<Item, int> Inventory = new Dictionary<Item, int>(); 
    static Item StartSword = Item.FindByName("Epée en fer"); 
    static Item Potion = Item.FindByName("Potion");

    // ==== Profil courant ====
    public static string CurrentProfile = null;

    // ==== MongoDB ====
    private static IMongoCollection<BsonDocument> collection;

    public static void InitializeMongo(string connectionString)
    {
        var client = new MongoClient(connectionString);
        var db = client.GetDatabase("GameDB");
        collection = db.GetCollection<BsonDocument>("Profiles");
    }

    // ==== Création de profil ====
    public static bool CreateProfile(string profileName, string password)
    {
        if (collection.Find(Builders<BsonDocument>.Filter.Eq("_id", profileName)).Any())
        {
            Console.WriteLine("Profil déjà existant !");
            return false;
        }

        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = HashPassword(password, salt);

        var saveData = new BsonDocument
        {
            { "room", 0 },
            { "level", 1 },
            { "PlayerHp", 100 },
            { "MonsterHp", 0 },
            { "MonsterAttack", 0 },
            { "score", 0 },
            { "bestscore", 0 },
            { "Inventory", new BsonDocument { { StartSword, 1 }, { Potion, 3 } } }
        };

        var doc = new BsonDocument
        {
            { "_id", profileName },
            { "PasswordHash", Convert.ToBase64String(hash) },
            { "Salt", Convert.ToBase64String(salt) },
            { "SaveData", saveData }
        };

        collection.InsertOne(doc);
        Console.WriteLine($"Profil '{profileName}' créé avec succès !");
        return true;
    }

    // ==== Connexion ====
    public static bool Login(string profileName, string password)
    {
        var doc = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", profileName)).FirstOrDefault();
        if (doc == null)
        {
            Console.WriteLine("Profil non trouvé !");
            return false;
        }

        byte[] salt = Convert.FromBase64String(doc["Salt"].AsString);
        byte[] storedHash = Convert.FromBase64String(doc["PasswordHash"].AsString);

        byte[] hash = HashPassword(password, salt);
        if (!CryptographicOperations.FixedTimeEquals(hash, storedHash))
        {
            Console.WriteLine("Mot de passe incorrect !");
            return false;
        }

        CurrentProfile = profileName;
        LoadGame();
        Console.WriteLine($"Connecté au profil '{profileName}' !");
        return true;
    }

    private static byte[] HashPassword(string password, byte[] salt, int iterations = 100_000, int length = 32)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);
        return pbkdf2.GetBytes(length);
    }

    // ==== Sauvegarde ====
    public static void SaveGame()
    {
        if (CurrentProfile == null)
        {
            Console.WriteLine("Aucun profil connecté !");
            return;
        }

        var saveData = new BsonDocument
        {
            { "room", room },
            { "level", level },
            { "PlayerHp", PlayerHp },
            { "MonsterHp", MonsterHp },
            { "MonsterAttack", MonsterAttack },
            { "score", score },
            { "bestscore", bestscore },
            { "Inventory", new BsonDocument(Inventory) }
        };

        var update = Builders<BsonDocument>.Update.Set("SaveData", saveData);
        collection.UpdateOne(Builders<BsonDocument>.Filter.Eq("_id", CurrentProfile), update);
        Console.WriteLine("Partie sauvegardée dans MongoDB !");
    }

    // ==== Chargement ====
    public static void LoadGame()
    {
        if (CurrentProfile == null)
        {
            Console.WriteLine("Aucun profil connecté !");
            return;
        }

        var doc = collection.Find(Builders<BsonDocument>.Filter.Eq("_id", CurrentProfile)).FirstOrDefault();
        if (doc == null || !doc.Contains("SaveData"))
        {
            Console.WriteLine("Pas de sauvegarde trouvée. Réinitialisation...");
            ResetSave();
            return;
        }

        var data = doc["SaveData"].AsBsonDocument;
        room = data.GetValue("room", 0).ToInt32();
        level = data.GetValue("level", 1).ToInt32();
        PlayerHp = data.GetValue("PlayerHp", 100).ToInt32();
        MonsterHp = data.GetValue("MonsterHp", 0).ToInt32();
        MonsterAttack = data.GetValue("MonsterAttack", 0).ToInt32();
        score = data.GetValue("score", 0).ToInt32();
        bestscore = data.GetValue("bestscore", 0).ToInt32();

        Inventory = new Dictionary<string, int>();
        if (data.Contains("Inventory"))
        {
            foreach (var item in data["Inventory"].AsBsonDocument)
                Inventory[item.Name] = item.Value.AsInt32;
        }

        Console.WriteLine("Partie chargée depuis MongoDB !");
    }

    // ==== Réinitialisation ====
    public static void ResetSave()
    {
        room = 0;
        level = 1;
        PlayerHp = 100;
        MonsterHp = 0;
        MonsterAttack = 0;
        score = 0;
        bestscore = 0;
        Inventory = new Dictionary<Item, int>
        {
            { StartSword, 1 },
            { Potion, 3 }
        };

        SaveGame();
        Console.WriteLine("Sauvegarde réinitialisée !");
    }

    // ==== Affichage ====
    public static void ShowSaveState()
    {
        Console.WriteLine($"Profil: {CurrentProfile}");
        Console.WriteLine($"Score: {score}, BestScore: {bestscore}");
        Console.WriteLine($"HP Joueur: {PlayerHp}, HP Monstre: {MonsterHp}, Attaque Monstre: {MonsterAttack}");
        Console.WriteLine("Inventaire:");
        foreach (var item in Inventory)
            Console.WriteLine($"- {item.Key} x{item.Value}");
    }
}
