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
    static public Dictionary<string, object> Inventory = new Dictionary<string, object>();

    static string pathToSave = @"C:\Users\bndiaye\source\repos\Rogue-Arena-Like\Rogue-Arena-Like\Save\Save.JSON";
    static public void AfficherScore()
    {
        Console.WriteLine($"Votre score actuel est : {score} point(s).");
    }

    static public void ReinitialiserScore()
    {
        score = 0;
        SauvegarderScore();
        Console.WriteLine("Score réinitialisé !");
    }

    static public void SauvegarderScore()
    {
        try
        {
            File.WriteAllText(pathToSave, score.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la sauvegarde : {ex.Message}");
        }
    }

    static public void ChargerScore()
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
            Console.WriteLine($"Creation d'un nouveau fichier de sauvegard");
            FileStream fs = File.Create(pathToSave);
        }
    }
}
