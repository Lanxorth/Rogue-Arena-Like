using System;
using System.Diagnostics;

class Menu
{
    static void Main(string[] args)
    {
        bool login = false;

        if (login == false)
        {
            Console.WriteLine("Select a option :");
            Console.WriteLine("1 - Login");
            Console.WriteLine("2 - Create Save");
            Console.WriteLine("3 - Close Game");

            Console.Write("Votre choix : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":

                    login = true;
                    break;

                case "2":
                    

                    break;

                case "3":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Choix invalide.");
                    return;
            }
            
        }

        if (login != false)
        {
            Console.WriteLine("Select a option :");
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
                    Save.AfficherScore();

                    break;

                case "3":
                    

                case "4":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Choix invalide.");
                    return;
            }
        }
    }
}