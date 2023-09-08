using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;



public class SkillManager : MonoBehaviour
{
	public static SkillManager Instance;
	private FireRateEffect fireRateEffect;
    private SpawnEffect spawnEffect;
	private DamageEffect damageEffect;
	
    public List<SkillData> skills = new List<SkillData>(); // List of all the skills available
    private GameObject activeSkill; // The currently active skill
	
	// "Level" scene - skill reward UI variables
	private SkillData skillOption1;
	private SkillData skillOption2;
	private SkillData skillOption3;
	private SkillData currentSkillOption1;
	private SkillData currentSkillOption2;
	private SkillData currentSkillOption3;
	[Header("Skill UI random population")]
	public Button skillButton01;
	public Button skillButton02;
	public Button skillButton03;
	public TMP_Text SkillOption01;
	public TMP_Text SkillOption02;
	public TMP_Text SkillOption03;
	
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			Debug.Log("SkillManager: Created and set as Singleton instance");
		}
		else
		{
			Debug.Log("Existing SkillManager detected. Destroying this instance.");
			Destroy(gameObject);
		}
		// Getting the references
        fireRateEffect = GetComponent<FireRateEffect>();
        spawnEffect = GetComponent<SpawnEffect>();
        damageEffect = GetComponent<DamageEffect>();
	}
	
    // Method to select a random skill from the skills list
    public SkillData GetRandomSkill()
    {
        if (skills.Count > 0)
        {
            int randomIndex = Random.Range(0, skills.Count);
            return skills[randomIndex];
        }
        else
        {
            Debug.LogError("No skills available in SkillManager");
            return null;
        }
    }

    // Method to find a specific skill by its name
    public SkillData FindSkillByName(string name)
    {
        return skills.Find(skill => skill.Name == name);
    }

    // Apply the effects of a given skill
    public void ApplySkillEffects(SkillData skill)
    {
        if (skill != null)
        {
            foreach (var effect in skill.Effects)
            {
                switch (effect)
                {
                    case "Damage":
                        if (damageEffect != null)
                        {
                            damageEffect.ApplyDamageEffect();
                        }
                        break;

                    case "Fire Rate":
                        if (fireRateEffect != null)
                        {
                            fireRateEffect.isActive = true; // Start shooting projectiles
                        }
                        break;

                    case "Spawn":
                        if (spawnEffect != null)
                        {
                            spawnEffect.Spawn(); // Always spawn when the skill is activated
                            spawnEffect.isActive = true;
                        }
                        break;

                    // Add more effect checks as needed
                }
            }
        }
        else
        {
            Debug.LogError("Attempted to apply effects for a null skill.");
        }
    }

	// Remove the effects of a given skill
    public void RemoveSkillEffects(SkillData skill) 
    {
        if (skill != null)
        {
            foreach (var effect in skill.Effects)
            {
                switch (effect)
                {
                    case "Damage":
                        if (damageEffect != null)
                        {
                            damageEffect.RemoveDamageEffect();
                        }
                        break;

                    case "Fire Rate":
                        if (fireRateEffect != null)
                        {
                            fireRateEffect.isActive = false; // Stop shooting projectiles
                        }
                        break;

                    case "Spawn":
                        if (spawnEffect != null)
                        {
                            spawnEffect.isActive = false; // Stop the missile base from following the player
                        }
                        break;

                    // Add more effect checks as needed
                }
            }
        }
        else
        {
            Debug.LogError("Attempted to remove effects for a null skill.");
        }
    }
	
    // Activate a given skill
    public void ActivateSkill(SkillData skill = null)
    {
        if (skill != null)
        {
            skill.Active = true;
            ApplySkillEffects(skill);
            Debug.Log($"Skill {skill.Name} has been activated.");
        }
        else
        {
            Debug.LogError("Attempted to activate a null skill.");
        }
    }

    // Deactivate a given skill
    public void DeactivateSkill(SkillData skill = null)
    {
        if (skill != null)
        {
            skill.Active = false;
            RemoveSkillEffects(skill);
            Debug.Log($"Skill {skill.Name} has been deactivated.");
        }
        else
        {
            Debug.LogError("Attempted to deactivate a null skill.");
        }
    }

    // Activate a skill by its name
    public void ActivateSkillByName(string skillName)
    {
        SkillData skill = skills.Find(s => s.Name == skillName);
    }
	
	// Set the skill options displayed in the UI
	public void SetSkillOptions(SkillData option1, SkillData option2, SkillData option3)
    {
        // Set text to skill names, or empty if null
        SkillOption01.text = option1 != null ? option1.Name : "";
        SkillOption02.text = option2 != null ? option2.Name : "";
        SkillOption03.text = option3 != null ? option3.Name : "";
    
        // Clear existing listeners on the buttons
        skillButton01.onClick.RemoveAllListeners();
        skillButton02.onClick.RemoveAllListeners();
        skillButton03.onClick.RemoveAllListeners();
    
        // Add new listeners for each button to activate the corresponding skill
        // Only add listener if the skill is not null
        if (option1 != null)
        {
            skillButton01.onClick.AddListener(() => ActivateSkillByName(option1.Name));
        }
    
        if (option2 != null)
        {
            skillButton02.onClick.AddListener(() => ActivateSkillByName(option2.Name));
        }
        
        if (option3 != null)
        {
            skillButton03.onClick.AddListener(() => ActivateSkillByName(option3.Name));
        }
    }
    
	// Populate the skill options in the UI with random skills
    public void PopulateSkillOptions()
    {
        SkillData currentSkillOption1 = GetRandomSkill();
        SkillData currentSkillOption2 = GetRandomSkill();
        SkillData currentSkillOption3 = GetRandomSkill();
    
        if (currentSkillOption1 != null && currentSkillOption2 != null && currentSkillOption3 != null)
        {
            SetSkillOptions(currentSkillOption1, currentSkillOption2, currentSkillOption3);
        }
    }
    
	// Activate the skill corresponding to Skill Option 1 when clicked
    public void OnSkillOption1Clicked()
    {
        string skillName = SkillOption01.text.Split('-')[0].Trim(); // Get the skill name from the button text
        SkillData skill = FindSkillByName(skillName); // Find the corresponding skill
        ActivateSkill(skill); // Activate the skill
		UIManager.Instance.ToggleCanvas(3);
    }
    // Activate the skill corresponding to Skill Option 2 when clicked
    public void OnSkillOption2Clicked()
    {
        string skillName = SkillOption02.text.Split('-')[0].Trim(); // Get the skill name from the button text
        SkillData skill = FindSkillByName(skillName); // Find the corresponding skill
        ActivateSkill(skill); // Activate the skill
		UIManager.Instance.ToggleCanvas(3);
    }
    // Activate the skill corresponding to Skill Option 3 when clicked
    public void OnSkillOption3Clicked()
    {
        string skillName = SkillOption03.text.Split('-')[0].Trim(); // Get the skill name from the button text
        SkillData skill = FindSkillByName(skillName); // Find the corresponding skill
        ActivateSkill(skill); // Activate the skill
		UIManager.Instance.ToggleCanvas(3);
    }	
}
