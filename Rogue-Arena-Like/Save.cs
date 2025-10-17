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
    public static int PlayerAttack = 10;
    public static int MonsterHp = 0;
    public static int MonsterAttack = 0;
    public static int score = 0;
    public static Dictionary<Item, int> Inventory = new Dictionary<Item, int>();
    static Item StartSword = Item.FindByName("Epée en fer");
    static Item Potion = Item.FindByName("Potion");
    public static bool inFight = false;

    // ==== Sauvegarde JSON ====
    public static void SaveGame()
    {
        if (MongoService.CurrentProfile == null) return;

        try
        {
            string saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save");
            if (!Directory.Exists(saveDir))
                Directory.CreateDirectory(saveDir);

            string path = Path.Combine(saveDir, $"Save_{MongoService.CurrentProfile}.json");

            var inventoryForJson = new Dictionary<string, int>();
            foreach (var kvp in Inventory)
                inventoryForJson[kvp.Key.Name] = kvp.Value;

            var saveData = new SaveData
            {
                room = room,
                level = level,
                PlayerHp = PlayerHp,
                PlayerAttack = PlayerAttack,
                MonsterHp = MonsterHp,
                MonsterAttack = MonsterAttack,
                score = score,
                Inventory = inventoryForJson,
                inFight = inFight
            };

            string json = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
            byte[] encrypted = EncryptString(json);
            File.WriteAllBytes(path, encrypted);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de la sauvegarde du jeu : " + ex.Message);
        }
    }

    // ==== Chargement JSON ====
    public static void LoadGame()
    {
        if (MongoService.CurrentProfile == null) return;

        try
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save", $"Save_{MongoService.CurrentProfile}.json");
            if (!File.Exists(path))
            {
                Console.WriteLine("Aucune sauvegarde trouvée. Réinitialisation.");
                ResetSave();
                return;
            }

            byte[] encrypted = File.ReadAllBytes(path);
            string json = DecryptBytes(encrypted);
            var data = JsonSerializer.Deserialize<SaveData>(json);

            room = data.room;
            level = data.level;
            PlayerHp = data.PlayerHp;
            PlayerAttack = data.PlayerAttack;
            MonsterHp = data.MonsterHp;
            MonsterAttack = data.MonsterAttack;
            score = data.score;
            inFight = data.inFight;

            Inventory = new Dictionary<Item, int>();
            foreach (var kvp in data.Inventory)
            {
                Item itemObj = Item.FindByName(kvp.Key);
                if (itemObj != null)
                    Inventory[itemObj] = kvp.Value;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors du chargement de la sauvegarde : " + ex.Message);
        }
    }

    // ==== Réinitialisation ====
    public static void ResetSave()
    {
        try
        {
            room = 0;
            level = 1;
            PlayerHp = 100;
            PlayerAttack = 10;
            MonsterHp = 0;
            MonsterAttack = 0;
            score = 0;
            Inventory = new Dictionary<Item, int>
            {
                { StartSword, 1 },
                { Potion, 3 }
            };
            inFight = false;

            SaveGame();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de la réinitialisation : " + ex.Message);
        }
    }

    // ==== Affichage ====
    public static void ShowSaveState()
    {
        try
        {
            Console.WriteLine("Profil: " + MongoService.CurrentProfile);
            Console.WriteLine("Score: " + score + ", Meilleur score: " + MongoService.BestScore);
            Console.WriteLine("Salle: " + room + ", Niveau: " + level);
            Console.WriteLine("PV Joueur: " + PlayerHp + ", Attaque Joueur: " + PlayerAttack);
            Console.WriteLine("Inventaire:");
            foreach (var item in Inventory)
                Console.WriteLine("- " + item.Key.Name + " x" + item.Value);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erreur lors de l'affichage de la sauvegarde : " + ex.Message);
        }
    }

    private static byte[] encryptionKey = new byte[32]  // 256 bits AES
{
    21, 32, 12, 44, 55, 66, 77, 88, 99, 100, 101, 102, 103, 104, 105, 106,
    107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122
};

    private static byte[] EncryptString(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = encryptionKey;
        aes.GenerateIV(); // génère un vecteur d'initialisation (IV) aléatoire

        using MemoryStream ms = new();
        ms.Write(aes.IV, 0, aes.IV.Length); // stocke l'IV au début du fichier

        using CryptoStream cryptoStream = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
        using StreamWriter sw = new(cryptoStream);
        sw.Write(plainText);

        return ms.ToArray();
    }

    private static string DecryptBytes(byte[] cipherData)
    {
        using Aes aes = Aes.Create();
        aes.Key = encryptionKey;

        byte[] iv = new byte[16]; // taille d'un IV pour AES
        Array.Copy(cipherData, 0, iv, 0, iv.Length);
        aes.IV = iv;

        using MemoryStream ms = new(cipherData, iv.Length, cipherData.Length - iv.Length);
        using CryptoStream cryptoStream = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
        using StreamReader sr = new(cryptoStream);
        return sr.ReadToEnd();
    }

    private class SaveData
    {
        public int room { get; set; }
        public int level { get; set; }
        public int PlayerHp { get; set; }
        public int PlayerAttack { get; set; }
        public int MonsterHp { get; set; }
        public int MonsterAttack { get; set; }
        public int score { get; set; }
        public Dictionary<string, int> Inventory { get; set; }
        public bool inFight { get; set; }
    }
}
