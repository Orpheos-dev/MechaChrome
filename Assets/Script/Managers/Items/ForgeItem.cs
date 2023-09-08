[System.Serializable]
public class ForgeItem
{
    public string name; // The name of the item
    public string type; // The type of the item (e.g., "Head", "Torso", "Legs", "Arms")
    
    // Add any other properties that a ForgeItem should have

    public ForgeItem(string name, string type)
    {
        this.name = name;
        this.type = type;
    }
}
