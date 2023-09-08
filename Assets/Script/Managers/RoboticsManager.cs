using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticsManager : MonoBehaviour
{
    public List<RoboticsItem> robotList = new List<RoboticsItem>();

    // Add reference to ForgeItemManager
    public ForgeItemManager forgeItemManager;

    public void AddRoboticsItem(RoboticsItem newItem)
    {
        robotList.Add(newItem);
        
        // Remove the forge items used in creating the robot
        forgeItemManager.RemoveForgeItem(newItem.head);
        forgeItemManager.RemoveForgeItem(newItem.torso);
        forgeItemManager.RemoveForgeItem(newItem.arms);
        forgeItemManager.RemoveForgeItem(newItem.legs);
    }

    // This function will return all the existing robotics items.
    public List<RoboticsItem> GetAllRoboticsItems()
    {
        return robotList;
    }
}

