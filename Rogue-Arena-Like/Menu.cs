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
            Console.WriteLine("2 - Check Score");
            Console.WriteLine("3 - Close Game");

            Console.Write("Votre choix : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    // Lancer le Bloc-notes

                    break;

                case "2":
                    // Lancer la Calculatrice

                    break;

                case "3":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Choix invalide.");
                    return;
            }

        }
        

    }
}