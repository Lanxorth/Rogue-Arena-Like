using System;

class Game
{
    public static void StartGame()
    {
        Item potionObj = Save.Inventory.Keys.FirstOrDefault(i => i.Name == "Potion");
        bool inGame = true;

        while (inGame)
        {
            Random rnd = new Random();

            if (Save.inFight == true)
            {
                Fight(rnd);
            }

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
                    int randEvent = rnd.Next(1, 4);
                    Console.WriteLine("\nVous entrez dans la salle " + Save.room+".");
                    switch (randEvent)
                    {
                        case 1:

                            int randomPotion = rnd.Next(1, 4);
                            Console.WriteLine("\n"+randomPotion+" Potion trouver!");
                            if (Save.Inventory.ContainsKey(potionObj))
                                Save.Inventory[potionObj] += randomPotion;
                            else
                                Save.Inventory[potionObj] = randomPotion;
                            break;
                        case 2:
                            Save.inFight = true;
                            Save.SaveGame();
                            Fight(rnd);
                            break;

                        case 3:
                            Save.inFight = true;
                            Save.SaveGame();
                            Fight(rnd);
                            break;

                    }
                    Save.score += 10;
                    MongoService.UpdateBestScore(Save.score);
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
        // Si le monstre n'existe pas encore
        if (Save.MonsterHp <= 0)
        {
            Save.MonsterHp = random.Next(50, 101); // PV monstre entre 50 et 100
            Save.MonsterAttack = random.Next(0, 11) + Save.room; // Dégâts entre 0 et 10
            Console.WriteLine($"Un monstre apparaît avec {Save.MonsterHp} HP !");
        }
        else
        {
            Console.WriteLine($"Combat repris ! Monstre : {Save.MonsterHp} HP, Joueur : {Save.PlayerHp} HP");
        }

        while (Save.MonsterHp > 0 && Save.PlayerHp > 0)
        {
            Console.WriteLine("\nQue voulez-vous faire ?");
            Console.WriteLine("1. Attaquer (inflige "+Save.PlayerAttack+" dégâts)");
            Console.WriteLine("2. Utiliser une potion (+50 HP, max 100)");

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

                default:
                    Console.WriteLine("Choix invalide.");
                    break;
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
                MongoService.UpdateBestScore(Save.score);
                Save.ResetSave(); // Reset la partie
                return;
            }
            else if (Save.MonsterHp <= 0)
            {
                Console.WriteLine("Vous avez vaincu le monstre !");
                Save.score += 50;
                Save.level ++;
                Save.PlayerAttack += 5;
                MongoService.UpdateBestScore(Save.score);
                Save.inFight = false;

                // Réinitialise le combat
                Save.MonsterHp = 0;
                Save.MonsterAttack = 0;
                Save.SaveGame();
                return;
            }
        }
    }

}