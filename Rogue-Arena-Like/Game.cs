using System;

class Game
{
    public static void StartGame()
    {
        Item potionObj = Save.Inventory.Keys.FirstOrDefault(i => i.Name == "Potion");
        bool inGame = true;

        while (inGame)
        {
            Console.WriteLine("\n=== Menu Jeu ===");
            Console.WriteLine("1 - Go to next room");
            Console.WriteLine("2 - Check Inventory");
            Console.WriteLine("3 - Return to Menu");
            Console.Write("Votre choix : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    Save.room++;
                    Random rnd = new Random();
                    int randEvent = rnd.Next(1, 4);
                    switch (randEvent)
                    {
                        case 1:

                            Console.WriteLine("Potion trouver.");
                            if (Save.Inventory.ContainsKey(potionObj))
                                Save.Inventory[potionObj] += 1;
                            else
                                Save.Inventory[potionObj] = 1;
                            break;
                        case 2:
                            Fight(rnd);
                            return;

                        case 3:
                            Fight(rnd);
                            return;

                    }
                    Save.score += 10;
                    Save.UpdateBestScore();
                    Save.SaveGame();
                    break;

                case "2":
                    Save.ShowSaveState();
                    break;

                case "3":
                    inGame = false;
                    break;

                default:
                    Console.WriteLine("Choix invalide.");
                    break;
            }
        }
        
    }

    public static void Fight(Random random)
    {
        // Si le monstre n'existe pas encore (nouveau combat)
        if (Save.MonsterHp <= 0)
        {
            Save.MonsterHp = random.Next(50, 101); // PV monstre entre 50 et 100
            Save.MonsterAttack = random.Next(5, 16); // Dégâts entre 5 et 15
            Console.WriteLine($"Un monstre apparaît avec {Save.MonsterHp} HP !");
        }
        else
        {
            Console.WriteLine($"Combat repris ! Monstre : {Save.MonsterHp} HP, Joueur : {Save.PlayerHp} HP");
        }

        while (Save.MonsterHp > 0 && Save.PlayerHp > 0)
        {
            Console.WriteLine("\nQue voulez-vous faire ?");
            Console.WriteLine("1. Attaquer (inflige 10 dégâts)");
            Console.WriteLine("2. Utiliser une potion (+50 HP, max 100)");
            Console.WriteLine("3. Fuir le combat");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Attaque du joueur
                    Save.MonsterHp -= 10;
                    if (Save.MonsterHp < 0) Save.MonsterHp = 0;
                    Console.WriteLine($"Vous attaquez et infligez 10 dégâts ! Monstre : {Save.MonsterHp} HP restants.");
                    break;

                case "2":
                    // Vérifie si le joueur a une potion
                    Item potionItem = Item.FindByName("Potion");
                    if (Save.Inventory.ContainsKey(potionItem) && Save.Inventory[potionItem] > 0)
                    {
                        Save.PlayerHp += 50;
                        if (Save.PlayerHp > 100) Save.PlayerHp = 100; // Cap des PV
                        Save.Inventory[potionItem]--;
                        Console.WriteLine($"Vous buvez une potion et regagnez 50 HP ! PV actuels : {Save.PlayerHp} HP");
                    }
                    else
                    {
                        Console.WriteLine("Vous n’avez plus de potions !");
                        continue; // Ne passe pas le tour
                    }
                    break;

                case "3":
                    Console.WriteLine("Vous fuyez le combat !");
                    Save.SaveGame();
                    return; // Le joueur quitte le combat
            }

            // Si le monstre est encore vivant, il attaque
            if (Save.MonsterHp > 0)
            {
                Save.PlayerHp -= Save.MonsterAttack;
                if (Save.PlayerHp < 0) Save.PlayerHp = 0;
                Console.WriteLine($"Le monstre vous attaque et inflige {Save.MonsterAttack} dégâts !");
                Console.WriteLine($"Vos HP : {Save.PlayerHp}");
            }

            // Sauvegarde l’état du combat après chaque action
            Save.SaveGame();

            // Vérifie fin de combat
            if (Save.PlayerHp <= 0)
            {
                Console.WriteLine("Vous êtes mort...");
                Save.ResetSave(); // Reset la partie
                return;
            }
            else if (Save.MonsterHp <= 0)
            {
                Console.WriteLine("Vous avez vaincu le monstre !");
                Save.score += 10;
                Save.UpdateBestScore();

                // Réinitialise le combat
                Save.MonsterHp = 0;
                Save.MonsterAttack = 0;
                Save.SaveGame();
                return;
            }
        }
    }

}