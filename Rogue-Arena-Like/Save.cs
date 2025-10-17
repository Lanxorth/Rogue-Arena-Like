using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

class Save
{
    // ==== Données de jeu ====
    public static int room = 0;
    public static int level = 1;
    public static int PlayerHp = 100;
    public static int MonsterHp = 0;
    public static int MonsterAttack = 0;
    public static int score = 0;
    public static Dictionary<Item, int> Inventory = new Dictionary<Item, int>();
    static Item StartSword = Item.FindByName("Epée en fer");
    static Item Potion = Item.FindByName("Potion");

    // ==== Profil ====
    public static string CurrentProfile = null;
    public static int BestScore = 0;

    // ==== MongoDB ====
    private static IMongoCollection<BsonDocument> collection;

    // ==== Initialise MongoDB ====
    public static void InitializeMongo(string connectionString = "mongodb://localhost:27017")
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

        var doc = new BsonDocument
        {
            { "_id", profileName },
            { "PasswordHash", Convert.ToBase64String(hash) },
            { "Salt", Convert.ToBase64String(salt) },
            { "BestScore", 0 }
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

        byte[] storedHash = Convert.FromBase64String(doc["PasswordHash"].AsString);
        byte[] salt = Convert.FromBase64String(doc["Salt"].AsString);
        byte[] hash = HashPassword(password, salt);

        if (!CryptographicOperations.FixedTimeEquals(hash, storedHash))
        {
            Console.WriteLine("Mot de passe incorrect !");
            return false;
        }

        CurrentProfile = profileName;
        BestScore = doc["BestScore"].ToInt32();

        LoadGame();
        Console.WriteLine($"Connecté au profil '{profileName}' !");
        return true;
    }

    // ==== Hash du mot de passe ====
    private static byte[] HashPassword(string password, byte[] salt, int iterations = 100_000, int length = 32)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512);
        return pbkdf2.GetBytes(length);
    }

    // ==== Sauvegarde JSON locale ====
    public static void SaveGame()
    {
        if (CurrentProfile == null) return;

        try
        {
            string saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save");
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            string path = Path.Combine(saveDir, $"Save_{CurrentProfile}.json");

            var inventoryForJson = new Dictionary<string, int>();
            foreach (var kvp in Inventory)
                inventoryForJson[kvp.Key.Name] = kvp.Value;

            var saveData = new SaveData
            {
                room = room,
                level = level,
                PlayerHp = PlayerHp,
                MonsterHp = MonsterHp,
                MonsterAttack = MonsterAttack,
                score = score,
                Inventory = inventoryForJson
            };

            string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la sauvegarde JSON : {ex.Message}");
        }
    }

    // ==== Chargement JSON locale ====
    public static void LoadGame()
    {
        if (CurrentProfile == null) return;

        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save", $"Save_{CurrentProfile}.json");
        if (!File.Exists(path))
        {
            Console.WriteLine("Aucune sauvegarde locale trouvée. Réinitialisation...");
            ResetSave();
            return;
        }

        string json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<SaveData>(json);

        room = data.room;
        level = data.level;
        PlayerHp = data.PlayerHp;
        MonsterHp = data.MonsterHp;
        MonsterAttack = data.MonsterAttack;
        score = data.score;

        Inventory = new Dictionary<Item, int>();
        foreach (var kvp in data.Inventory)
        {
            Item itemObj = Item.FindByName(kvp.Key);
            if (itemObj != null)
                Inventory[itemObj] = kvp.Value;
        }
    }

    // ==== Update BestScore MongoDB ====
    public static void UpdateBestScore()
    {
        if (CurrentProfile == null) return;
        if (score > BestScore)
        {
            BestScore = score;
            var filter = Builders<BsonDocument>.Filter.Eq("_id", CurrentProfile);
            var update = Builders<BsonDocument>.Update.Set("BestScore", BestScore);
            collection.UpdateOne(filter, update);
        }
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
        Inventory = new Dictionary<Item, int>
        {
            { StartSword, 1 },
            { Potion, 3 }
        };

        SaveGame();
    }

    // ==== Affichage ====
    public static void ShowSaveState()
    {
        Console.WriteLine($"Profil: {CurrentProfile}");
        Console.WriteLine($"Score: {score}, BestScore: {BestScore}");
        Console.WriteLine($"Salle: {room}, Level: {level}");
        Console.WriteLine($"HP Joueur: {PlayerHp}");
        Console.WriteLine("Inventaire:");
        foreach (var item in Inventory)
            Console.WriteLine($"- {item.Key.Name} x{item.Value}");
    }

    private class SaveData
    {
        public int room { get; set; }
        public int level { get; set; }
        public int PlayerHp { get; set; }
        public int MonsterHp { get; set; }
        public int MonsterAttack { get; set; }
        public int score { get; set; }
        public Dictionary<string, int> Inventory { get; set; }
    }
}
