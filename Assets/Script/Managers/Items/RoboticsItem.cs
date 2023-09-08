using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboticsItem : MonoBehaviour
{
    public ForgeItem head, torso, arms, legs;
    public string robotName;

    public RoboticsItem(string name, ForgeItem headItem, ForgeItem torsoItem, ForgeItem armsItem, ForgeItem legsItem)
    {
        this.robotName = name;
        this.head = headItem;
        this.torso = torsoItem;
        this.arms = armsItem;
        this.legs = legsItem;
    }

    // You can also define more functions here as per the game requirements.
}
