public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int State { get; set; }

    public static List<Item> Items = new List<Item>
    {
        new Item { Name = "Ep�e en fer", Description = "Ep�e en fer, inflige 10 point de d�gat", State = 10},
        new Item { Name = "Ep�e en acier", Description = "Ep�e en acier, inflige 15 point de d�gat", State = 15},
        new Item { Name = "Ep�e en argent", Description = "Ep�e en argent, inflige 20 point de d�gat", State = 20},
        new Item { Name = "Ep�e en or", Description = "Ep�e en or, inflige 25 point de d�gat", State = 25},
        new Item { Name = "Ep�e en mithril", Description = "Ep�e en mithril, inflige 30 point de d�gat", State = 30},
        new Item { Name = "Ep�e en adamantium", Description = "Ep�e en adamantium, inflige 35 point de d�gat", State = 35},
        new Item { Name = "Ep�e ultime", Description = "L'�p�e ultime, inflige 40 point de d�gat", State = 40},
        new Item { Name = "Potion", Description = "Une simple potion , soigne le joueur de 50 PV", State = 50},
        new Item { Name = "Super-Potion", Description = "Une am�liorer potion , soigne le joueur de 100 PV", State = 100},
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