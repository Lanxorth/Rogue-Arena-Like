using System;

class Menu
{
    static void Main(string[] args)
    {
        // Initialisation MongoDB
        Save.InitializeMongo("mongodb://localhost:27017"); // Remplace par ton URI

        bool loggedIn = false;

        while (true)
        {
            if (!loggedIn)
            {
                Console.WriteLine("\n=== Menu Profil ===");
                Console.WriteLine("1 - Login");
                Console.WriteLine("2 - Create Profile");
                Console.WriteLine("3 - Close Game");
                Console.Write("Votre choix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        Console.Write("Nom du profil : ");
                        string loginName = Console.ReadLine();
                        Console.Write("Mot de passe : ");
                        string loginPass = ReadPassword();
                        loggedIn = Save.Login(loginName, loginPass);
                        break;

                    case "2":
                        Console.Write("Nom du nouveau profil : ");
                        string newProfile = Console.ReadLine();
                        Console.Write("Mot de passe : ");
                        string newPass = ReadPassword();
                        Save.CreateProfile(newProfile, newPass);
                        break;

                    case "3":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Choix invalide.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("\n=== Menu Jeu ===");
                Console.WriteLine("1 - Start Game");
                Console.WriteLine("2 - Check Save");
                Console.WriteLine("3 - Reset Save");
                Console.WriteLine("4 - Close Game");
                Console.Write("Votre choix : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        Game.StartGame();
                        break;

                    case "2":
                        Save.ShowSaveState();
                        break;

                    case "3":
                        Save.ResetSave();
                        break;

                    case "4":
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Choix invalide.");
                        break;
                }
            }
        }
    }

    // Méthode pour cacher le mot de passe à la saisie
    private static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo info;
        do
        {
            info = Console.ReadKey(true);
            if (info.Key != ConsoleKey.Backspace && info.Key != ConsoleKey.Enter)
            {
                password += info.KeyChar;
                Console.Write("*");
            }
            else if (info.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
        } while (info.Key != ConsoleKey.Enter);
        Console.WriteLine();
        return password;
    }
}
