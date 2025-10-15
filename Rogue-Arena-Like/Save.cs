//using MongoDB.Driver;
using System;
using System.IO;
using System.Text;

class Save
{
    static public double room = 0.0;
    static public int level = 0;
    static public int PlayerHp = 0;
    static public int MonsterHp = 0;
    static public int MonsterAttack = 0;
    static public int score = 0;
    static public int bestscore = 0;
    static public Dictionary<Item, int> Inventory = new Dictionary<Item, int>();
    static Item StartSword = Item.FindByName("Epée en fer");
    static Item Potion = Item.FindByName("Potion");
    static public string profile;

    static string pathToSave;
    static public void ShowSaveState()
    {
        Console.WriteLine($"Votre score actuel est : {score} point(s).");
    }

    static public void ResetSave()
    {
        score = 0;
        room = 0.0;
        level = 1;
        PlayerHp = 0;
        MonsterHp = 0;
        MonsterAttack = 0;
        Inventory = new Dictionary<Item, int>()
        {
            {StartSword, 1 },
            {Potion, 3},
        };

        SaveGame();
        Console.WriteLine("Save réinitialisé !");
    }

    static public void SaveGame()
    {
        try
        {
            pathToSave = @"C:\Users\bndiaye\source\repos\Rogue-Arena-Like\Rogue-Arena-Like\Save\Save" + profile + ".JSON";
            File.WriteAllText(pathToSave, score.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la sauvegarde : {ex.Message}");
        }
    }

    static public void LoadSave()
    {
        try
        {
            if (File.Exists(pathToSave))
            {
                string contenu = File.ReadAllText(pathToSave);
                if (int.TryParse(contenu, out int sauvegarde))
                {
                    score = sauvegarde;
                }
            }
            else
            {
                FileStream fs = File.Create(pathToSave);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du chargement du score : {ex.Message}");
        }
    }
}
