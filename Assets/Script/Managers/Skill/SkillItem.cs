using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillData
{
    public string Name; // Name of the skill
    public string Type; // Type of the skill
    public List<string> Effects = new List<string>(); // The effects of the skill
    public bool Active; // If the skill is currently active

    // Override the ToString method to nicely format SkillData when outputted as a string
    public override string ToString()
    {
        string effectsString = string.Join(", ", Effects.ToArray());
        return $"Name: {Name}\nType: {Type}\nEffects: {effectsString}\nActive: {Active}";
    }
}
