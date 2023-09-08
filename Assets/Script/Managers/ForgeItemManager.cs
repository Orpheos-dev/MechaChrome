using System.Collections.Generic;
using UnityEngine;

public class ForgeItemManager : MonoBehaviour
{
    public List<ForgeItem> allForgeItems = new List<ForgeItem>(); // List of all forge items in the game

    // Method to add a forge item to the list (you would call this when an item is found during gameplay)
    public void AddForgeItem(ForgeItem newForgeItem)
    {
        allForgeItems.Add(newForgeItem);
    }

    // Method to remove a forge item from the list (you might call this when an item is used or destroyed)
    public void RemoveForgeItem(ForgeItem itemToRemove)
    {
        if (allForgeItems.Contains(itemToRemove))
        {
            allForgeItems.Remove(itemToRemove);
        }
        else
        {
            Debug.LogWarning("Trying to remove an item that doesn't exist in the list.");
        }
    }

    // Method to get a forge item based on its type (you might call this to populate the forge UI)
    public List<ForgeItem> GetForgeItemsByType(string type)
    {
        List<ForgeItem> itemsOfType = new List<ForgeItem>();
        foreach (ForgeItem item in allForgeItems)
        {
            if (item.type == type)
            {
                itemsOfType.Add(item);
            }
        }
        return itemsOfType;
    }

    // You can add more methods as needed for your game's logic.
}
