//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using TMPro;
//
//public class UIManager : MonoBehaviour
//{
//	public static UIManager Instance;
//	//private GameManager gameManager;
//	
//	public List<string> SceneArray = new List<string>();
//    public GameObject[] canvasArray;
//    public GameObject forgeItemButtonPrefab; 
//    public Transform headSectionContainer;
//	public Transform torsoSectionContainer;
//	public Transform legsSectionContainer;
//	public Transform armsSectionContainer;
//	
//	public Transform robotContainer;
//	public RoboticsManager roboticsManager;
//	public GameObject RoboticsItems;
//	
//	public Button headOptionButton;
//	public Button torsoOptionButton;
//	public Button armsOptionButton;
//	public Button legsOptionButton;
//
//    private ForgeItemManager forgeItemManager;
//    public List<ForgeItem> itemsToCombine = new List<ForgeItem>();
//	private bool itemsDisplayed = false;
//	
//	private SkillManager skillManager;
//	private SkillData skillOption1;
//	private SkillData skillOption2;
//	private SkillData skillOption3;
//	private SkillData currentSkillOption1;
//	private SkillData currentSkillOption2;
//	private SkillData currentSkillOption3;
//	public Button skillButton01;
//	public Button skillButton02;
//	public Button skillButton03;
//	public TMP_Text SkillOption01;
//	public TMP_Text SkillOption02;
//	public TMP_Text SkillOption03;
//	
//    public Text XpText;
//    public Text CurrencyText;
//    public Text FloorNumberText;
//	public void SetXP(int xp) { XpText.text = $"XP: {xp}"; }
//    public void SetCurrency(int currency) { CurrencyText.text = $"Currency: {currency}"; }
//    public void SetFloorNumber(int floorNumber) { FloorNumberText.text = $"Floor: {floorNumber}"; }
//    
//    public Text ItemOption01;
//    public Text ItemOption02;
//    public Text ItemOption03;
//	
//	// Method to open/close canvases
//	private static bool isFirstTransition = true;
//	
//    public Dropdown GameModeDropdown;
//
//	void Awake()
//	{
//		// Get the active scene
//		Scene currentScene = SceneManager.GetActiveScene();
//		
//		// Check if the current scene is Scene 0 or Scene 1
//		switch (currentScene.buildIndex)
//		{
//			case 0: // For Scene 0
//				// Activate the 0 index canvas and deactivate all other canvases
//				for (int i = 0; i < canvasArray.Length; i++)
//				{
//					canvasArray[i].SetActive(i == 0);
//				}
//				break;
//	
//			case 1: // For Scene 1
//				// Deactivate all canvases initially
//				for (int i = 0; i < canvasArray.Length; i++)
//				{
//					canvasArray[i].SetActive(false);
//				}
//	
//				// Then activate the 0 and 4 index canvases
//				if (canvasArray.Length > 4)
//				{
//					canvasArray[0].SetActive(true);
//					canvasArray[4].SetActive(true);
//				}
//				break;
//	
//			default:
//				Debug.LogWarning("No canvas setup for this scene.");
//				break;
//		}
//	}
//	
//    void Start()
//	{
//		Debug.Log("UIManager: Start");
//		skillManager = FindObjectOfType<SkillManager>();
//	}
//	
//	private void DeactivateAllCanvases() 
//	{
//		for (int i = 0; i < canvasArray.Length; i++)
//		{
//			if (canvasArray[i] != null)
//			{
//				canvasArray[i].SetActive(false);
//			}
//			else
//			{
//				Debug.LogWarning("Canvas at index " + i + " is not assigned.");
//			}
//		}
//	}
//		
//	public void ToggleCanvas(int canvasIndex)
//	{
//		if (isFirstTransition)
//		{
//			DeactivateAllCanvases();
//			isFirstTransition = false;
//		}
//		else
//		{
//			if (canvasArray[0] != null)
//			{
//				canvasArray[0].SetActive(false);
//			}
//			if (canvasArray[4] != null)
//			{
//				canvasArray[4].SetActive(false);
//			}
//		}
//	
//		string currentScene = SceneManager.GetActiveScene().name;
//		Debug.Log("Current Scene: " + currentScene);
//		Debug.Log("Canvas " + canvasIndex + " is: " + (canvasArray[canvasIndex] != null ? canvasArray[canvasIndex].name : "null"));
//	
//		int activeScene = SceneManager.GetActiveScene().buildIndex;
//	
//		if (activeScene == 0 || activeScene == 1)
//		{
//			DeactivateAllCanvases();
//			
//			if (canvasArray[canvasIndex] != null)
//			{
//				canvasArray[canvasIndex].SetActive(true);
//			}
//		}
//		else if (canvasIndex == 2) // Handling canvas index 2
//		{
//			// Deactivate only the first canvas
//			if (canvasArray[0] != null)
//			{
//				canvasArray[0].SetActive(false);
//			}
//			
//			// Activate the canvas at index 2
//			if (canvasArray[canvasIndex] != null)
//			{
//				canvasArray[canvasIndex].SetActive(true);
//				Debug.Log("Canvas 2 Activated, populating skill options"); // Debug message before calling the method
//				PopulateSkillOptions(); // Populate the skill options when canvas 2 is activated
//				Debug.Log("Skill options populated"); // Debug message after calling the method
//			}
//		}
//		
//		else if (canvasIndex == 9)
//		{
//			if (forgeItemManager == null)
//			{
//				forgeItemManager = FindObjectOfType<ForgeItemManager>();
//				if (forgeItemManager == null)
//				{
//					Debug.LogError("ForgeItemManager not found.");
//					return;
//				}
//			}
//		
//			if (headSectionContainer == null)
//			{
//				headSectionContainer = GameObject.Find("ForgeHeadSection").transform;
//				if (headSectionContainer == null)
//				{
//					Debug.LogError("HeadSectionContainer not found.");
//					return;
//				}
//			}
//			if (torsoSectionContainer == null)
//			{
//				torsoSectionContainer = GameObject.Find("ForgeTorsoSection").transform;
//				if (torsoSectionContainer == null)
//				{
//					Debug.LogError("TorsoSectionContainer not found.");
//					return;
//				}
//			}
//			if (legsSectionContainer == null)
//			{
//				legsSectionContainer = GameObject.Find("ForgeLegsSection").transform;
//				if (legsSectionContainer == null)
//				{
//					Debug.LogError("LegsSectionContainer not found.");
//					return;
//				}
//			}
//			if (armsSectionContainer == null)
//			{
//				armsSectionContainer = GameObject.Find("ForgeArmsSection").transform;
//				if (armsSectionContainer == null)
//				{
//					Debug.LogError("ArmsSectionContainer not found.");
//					return;
//				}
//			}
//	
//			if (forgeItemButtonPrefab == null)
//			{
//				forgeItemButtonPrefab = Resources.Load<GameObject>("Assets/Mesh/ForgeItems.prefab");
//				if (forgeItemButtonPrefab == null)
//				{
//					Debug.LogError("ForgeItemButtonPrefab not found.");
//					return;
//				}
//			}
//		
//			Debug.Log("ForgeItemManager: " + forgeItemManager);
//			Debug.Log("Number of ForgeItems: " + forgeItemManager.allForgeItems.Count);
//	
//			RefreshItems();
//		}
//	}
//	
//	public void ReturnButtonClicked()
//	{
//		// Deactivate all canvases
//		DeactivateAllCanvases();
//	
//		// Activate the first canvas (index 0)
//		canvasArray[0].SetActive(true);
//	
//		// Activate the second canvas (index 4)
//		canvasArray[4].SetActive(true);
//	}
//	
//    public void RefreshItems()
//    {
//        foreach (Transform child in headSectionContainer)
//        {
//            Destroy(child.gameObject);
//        }
//		foreach (Transform child in torsoSectionContainer)
//		{
//			Destroy(child.gameObject);
//		}
//		foreach (Transform child in legsSectionContainer)
//		{
//			Destroy(child.gameObject);
//		}
//		foreach (Transform child in armsSectionContainer)
//		{
//			Destroy(child.gameObject);
//		}
//		
//        DisplayItems();
//    }
//	
//	void DisplayItems()
//	{
//		// Check if the items have already been displayed
//		//if (itemsDisplayed)
//		//{
//		//  return;
//		//}
//		//
//		if (forgeItemManager == null)
//		{
//			Debug.LogError("forgeItemManager is null.");
//			return;
//		}
//	
//		if (forgeItemManager.allForgeItems == null)
//		{
//			Debug.LogError("allForgeItems is null.");
//			return;
//		}
//	
//		DisplayItemInContainer(forgeItemManager.GetForgeItemsByType("Head"), headSectionContainer);
//		DisplayItemInContainer(forgeItemManager.GetForgeItemsByType("Torso"), torsoSectionContainer);
//		DisplayItemInContainer(forgeItemManager.GetForgeItemsByType("Legs"), legsSectionContainer);
//		DisplayItemInContainer(forgeItemManager.GetForgeItemsByType("Arms"), armsSectionContainer);
//		
//		// Set the flag so the items won't be displayed again
//		//itemsDisplayed = true;
//	}
//	
//	void DisplayItemInContainer(List<ForgeItem> itemList, Transform container)
//	{
//		// Clear the container
//		foreach (Transform child in container)
//		{
//			Destroy(child.gameObject);
//		}
//	
//		// Loop through all items
//		foreach (ForgeItem item in itemList)
//		{
//			GameObject buttonObj = Instantiate(forgeItemButtonPrefab, container);
//			buttonObj.name = item.name; // Set the buttonObj's name to the item's name
//			
//			// Set the button's onClick listener
//			Button button = buttonObj.GetComponent<Button>();
//			if (button != null)
//			{
//				button.onClick.AddListener(() => AddForgeItemToCombine(item, button)); // Pass the button to the function
//			}
//			
//			// Set the associatedForgeItem for ForgeItemButton
//			ForgeItemButton forgeItemButton = buttonObj.GetComponent<ForgeItemButton>();
//			if (forgeItemButton != null)
//			{
//				forgeItemButton.associatedForgeItem = item;
//			}
//			
//			// Set the button's text
//			TMPro.TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
//			if (buttonText != null)
//			{
//				buttonText.text = item.name;
//			}
//		}
//
//	}
//
//    public void SetItemOptions(string option1, string option2, string option3)
//    {
//        ItemOption01.text = option1;
//        ItemOption02.text = option2;
//        ItemOption03.text = option3;
//    }
//
//    public void SetSkillOptions(SkillData option1, SkillData option2, SkillData option3)
//	{
//		// Set text to skill names, or empty if null
//		SkillOption01.text = option1 != null ? option1.Name : "";
//		SkillOption02.text = option2 != null ? option2.Name : "";
//		SkillOption03.text = option3 != null ? option3.Name : "";
//	
//		// Clear existing listeners on the buttons
//		skillButton01.onClick.RemoveAllListeners();
//		skillButton02.onClick.RemoveAllListeners();
//		skillButton03.onClick.RemoveAllListeners();
//	
//		// Add new listeners for each button to activate the corresponding skill
//		// Only add listener if the skill is not null
//		if (option1 != null)
//		{
//			skillButton01.onClick.AddListener(() => skillManager.ActivateSkillByName(option1.Name));
//		}
//	
//		if (option2 != null)
//		{
//			skillButton02.onClick.AddListener(() => skillManager.ActivateSkillByName(option2.Name));
//		}
//		
//		if (option3 != null)
//		{
//			skillButton03.onClick.AddListener(() => skillManager.ActivateSkillByName(option3.Name));
//		}
//	}
//	
//	public void PopulateSkillOptions()
//	{
//		if (skillManager != null)
//		{
//			currentSkillOption1 = skillManager.GetRandomSkill();
//			currentSkillOption2 = skillManager.GetRandomSkill();
//			currentSkillOption3 = skillManager.GetRandomSkill();
//		
//			if (currentSkillOption1 != null && currentSkillOption2 != null && currentSkillOption3 != null)
//			{
//				SetSkillOptions(currentSkillOption1, currentSkillOption2, currentSkillOption3);
//			}
//		}
//		else
//		{
//			Debug.LogError("SkillManager not found in scene.");
//		}
//	}
//	
//	public void OnSkillOption1Clicked()
//	{
//		string skillName = SkillOption01.text.Split('-')[0].Trim(); // Get the skill name from the button text
//		SkillData skill = skillManager.FindSkillByName(skillName); // Find the corresponding skill
//		skillManager.ActivateSkill(skill); // Activate the skill
//		
//	}
//	
//	public void OnSkillOption2Clicked()
//	{
//		string skillName = SkillOption02.text.Split('-')[0].Trim(); // Get the skill name from the button text
//		SkillData skill = skillManager.FindSkillByName(skillName); // Find the corresponding skill
//		skillManager.ActivateSkill(skill); // Activate the skill
//		
//	}
//	
//	public void OnSkillOption3Clicked()
//	{
//		string skillName = SkillOption03.text.Split('-')[0].Trim(); // Get the skill name from the button text
//		SkillData skill = skillManager.FindSkillByName(skillName); // Find the corresponding skill
//		skillManager.ActivateSkill(skill); // Activate the skill
//		
//	}
//	
//    public void SetGameMode()
//    {
//        switch (GameModeDropdown.value)
//        {
//            case 0:
//                break;
//            case 1:
//                break;
//            default:
//                break;
//        }
//    }
//
//    public void ChangeScene(int sceneIndex)
//	{
//		if (sceneIndex < 0 || sceneIndex >= SceneArray.Count)
//		{
//			Debug.LogError("Invalid scene index");
//			return;
//		}
//		
//		string sceneName = SceneArray[sceneIndex];
//		Debug.Log("Moving to scene: " + sceneName);
//	
//		// Call Initiate.Fade and pass in the sceneName and a multiplier
//		Initiate.Fade(sceneName, 1.0f);
//	}
//	
//
//    public void AddForgeItemToCombine(ForgeItem item, Button itemButton)
//	{
//		if (item == null || itemButton == null)
//		{
//			Debug.LogError("Item or item button is null");
//			return;
//		}
//	
//		itemsToCombine.Add(item);
//		
//		 // Add debug log here
//		Debug.Log("Item added to combine. Current count: " + itemsToCombine.Count);
//    
//		Button newButton = null;
//		switch (item.type)
//		{
//			case "Head":
//				newButton = ReplaceButton(headOptionButton, itemButton);
//				headOptionButton = newButton;
//				break;
//			case "Torso":
//				newButton = ReplaceButton(torsoOptionButton, itemButton);
//				torsoOptionButton = newButton;
//				break;
//			case "Arms":
//				newButton = ReplaceButton(armsOptionButton, itemButton);
//				armsOptionButton = newButton;
//				break;
//			case "Legs":
//				newButton = ReplaceButton(legsOptionButton, itemButton);
//				legsOptionButton = newButton;
//				break;
//			default:
//				Debug.LogError("Invalid item type: " + item.type);
//				break;
//		}
//		Destroy(itemButton.gameObject);
//	}
//	
//	void DisplaySingleItemInContainer(ForgeItem item, Transform container)
//	{
//		if (item == null || container == null)
//		{
//			Debug.LogError("Item or container is null");
//			return;
//		}
//	
//		foreach (Transform child in container)
//		{
//			Destroy(child.gameObject);
//		}
//	
//		// Create a new button for the item
//		GameObject buttonObj = Instantiate(forgeItemButtonPrefab, container);
//		buttonObj.name = item.name; // Set the buttonObj's name to the item's name
//		
//		// Get the ForgeItemButton component and assign the associatedForgeItem
//		ForgeItemButton forgeItemButton = buttonObj.GetComponent<ForgeItemButton>();
//		forgeItemButton.associatedForgeItem = item;
//		
//		// Set the button's onClick listener
//		Button button = buttonObj.GetComponent<Button>();
//		if (button != null)
//		{
//			button.onClick.AddListener(() => AddForgeItemToCombine(item, button)); // Pass the button to the function
//		}
//		TMPro.TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
//		if (buttonText != null)
//		{
//			buttonText.text = item.name;
//		}
//	}
//	
//	Button ReplaceButton(Button originalButton, Button itemButton)
//	{
//		Destroy(originalButton.gameObject); // remove the existing button
//		Button newButton = Instantiate(itemButton, originalButton.transform.parent); // create a new button
//		newButton.transform.localPosition = originalButton.transform.localPosition;
//		newButton.transform.localScale = originalButton.transform.localScale;
//		newButton.transform.localRotation = originalButton.transform.localRotation;
//	
//		ForgeItemButton fib = newButton.GetComponent<ForgeItemButton>(); // get ForgeItemButton script attached to the button
//		if (fib != null) // if the script is attached
//		{
//			newButton.onClick.AddListener(() => RemoveForgeItemFromCombine(fib.associatedForgeItem, newButton)); // set its OnClick action
//		}
//	
//		return newButton;
//	}
//	
//	public void RemoveForgeItemFromCombine(ForgeItem item, Button itemButton)
//	{
//		if (item == null || itemButton == null)
//		{
//			Debug.LogError("Item or item button is null");
//			return;
//		}
//	
//		itemsToCombine.Remove(item);
//		
//		// Add debug log here
//		Debug.Log("Item removed from combine. Current count: " + itemsToCombine.Count);
//
//		Button newButton = null;
//		switch (item.type)
//		{
//			case "Head":
//				newButton = ReplaceButton(headOptionButton, itemButton);
//				headOptionButton = newButton;
//				break;
//			case "Torso":
//				newButton = ReplaceButton(torsoOptionButton, itemButton);
//				torsoOptionButton = newButton;
//				break;
//			case "Arms":
//				newButton = ReplaceButton(armsOptionButton, itemButton);
//				armsOptionButton = newButton;
//				break;
//			case "Legs":
//				newButton = ReplaceButton(legsOptionButton, itemButton);
//				legsOptionButton = newButton;
//				break;
//			default:
//				Debug.LogError("Invalid item type: " + item.type);
//				break;
//		}
//		Transform container = GetTypeContainer(item.type);
//		if (container != null)
//		{
//			DisplaySingleItemInContainer(item, container);
//		}
//	}
//	
//	Transform GetTypeContainer(string type)
//	{
//		switch(type)
//		{
//			case "Head":
//				return headSectionContainer;
//			case "Torso":
//				return torsoSectionContainer;
//			case "Arms":
//				return armsSectionContainer;
//			case "Legs":
//				return legsSectionContainer;
//			default:
//				Debug.LogError("Invalid item type: " + type);
//				return null;
//		}
//	}
//	
//	public void Combine()
//	{
//		if (itemsToCombine.Count != 4)
//		{
//			Debug.LogError("Not enough items to combine");
//			return;
//		}
//	
//		ForgeItem head = null;
//		ForgeItem torso = null;
//		ForgeItem arms = null;
//		ForgeItem legs = null;
//	
//		foreach (ForgeItem item in itemsToCombine)
//		{
//			switch (item.type)
//			{
//				case "Head":
//					head = item;
//					break;
//				case "Torso":
//					torso = item;
//					break;
//				case "Arms":
//					arms = item;
//					break;
//				case "Legs":
//					legs = item;
//					break;
//				default:
//					Debug.LogError("Unknown item type: " + item.type);
//					return;
//			}
//		}
//	
//		if (head == null || torso == null || arms == null || legs == null)
//		{
//			Debug.LogError("Not all parts are present.");
//			return;
//		}
//	
//		RoboticsItem newItem = new RoboticsItem("Robot", head, torso, arms, legs);
//		roboticsManager.AddRoboticsItem(newItem); // The ForgeItems are removed from ForgeItemManager here
//	
//		itemsToCombine.Clear();
//		RefreshDisplay();
//	
//		InstantiateRoboticsItem(newItem); // Instantiate a new button for the combined item
//	
//		RefreshRoboticsCanvas();
//		// Add debug log here
//		Debug.Log("Combining items. Current count: " + itemsToCombine.Count);
//	}
//	
//	void InstantiateRoboticsItem(RoboticsItem item)
//	{
//		GameObject newRobotButton = Instantiate(RoboticsItems, robotContainer); // Instantiate a new button
//		// TODO: Set the properties of the newRobotButton to reflect the properties of the item
//		// For example, you might want to set the text on the button to be the name of the item
//	}
//	
//	public void RefreshRoboticsCanvas()
//	{
//		foreach (Transform child in robotContainer.transform)
//		{
//			Destroy(child.gameObject);
//		}
//	
//		foreach (RoboticsItem item in roboticsManager.GetAllRoboticsItems())
//		{
//			InstantiateRoboticsItem(item); // Instantiate a new button for each item
//		}
//	}
//	
//	void RefreshDisplay()
//	{
//		// TODO: Update UI elements here based on current game state
//	}
//		
//	public void OnStartForge() 
//	{
//		// TODO: Add the code to start the forge process if needed
//	}
//	
//}
//