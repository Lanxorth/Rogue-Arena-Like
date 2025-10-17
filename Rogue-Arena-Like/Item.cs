public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int State { get; set; }

    public static List<Item> Items = new List<Item>
    {
        new Item { Name = "Epée en fer", Description = "Epée en fer, inflige 10 point de dégat", State = 10},
        new Item { Name = "Epée en acier", Description = "Epée en acier, inflige 15 point de dégat", State = 15},
        new Item { Name = "Epée en argent", Description = "Epée en argent, inflige 20 point de dégat", State = 20},
        new Item { Name = "Epée en or", Description = "Epée en or, inflige 25 point de dégat", State = 25},
        new Item { Name = "Epée en mithril", Description = "Epée en mithril, inflige 30 point de dégat", State = 30},
        new Item { Name = "Epée en adamantium", Description = "Epée en adamantium, inflige 35 point de dégat", State = 35},
        new Item { Name = "Epée ultime", Description = "L'épée ultime, inflige 40 point de dégat", State = 40},
        new Item { Name = "Potion", Description = "Une simple potion , soigne le joueur de 50 PV", State = 50},
        new Item { Name = "Super-Potion", Description = "Une améliorer potion , soigne le joueur de 100 PV", State = 100},
    };

    public static Item FindByName(string name)
    {
        return Items.FirstOrDefault(i => i.Name == name);
    }

    public override string ToString()
    {
        return Name; // Affiche le nom de l'objet
    }
}