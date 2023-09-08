//using UnityEngine;
//using UnityEngine.UI;
//
//public class ForgeItemButton : MonoBehaviour
//{
//    public ForgeItem associatedForgeItem;  // The ForgeItem that this button represents
//
//    private UIManager uiManager;  // A reference to the UIManager so this button can interact with it
//
//    void Start()
//    {
//        uiManager = FindObjectOfType<UIManager>();  // Find the UIManager in the scene and set our reference to it
//    }
//
//	public void OnClick()
//	{
//		if (associatedForgeItem == null)
//		{
//			Debug.LogError("No associated item found");
//			return;
//		}
//		Button button = this.GetComponent<Button>();
//		if (button == null)
//		{
//			Debug.LogError("Button component not found");
//			return;
//		}
//		if (uiManager.itemsToCombine.Contains(associatedForgeItem))
//		{
//			uiManager.RemoveForgeItemFromCombine(associatedForgeItem, button);
//		}
//		else
//		{
//			uiManager.AddForgeItemToCombine(associatedForgeItem, button);
//		}
//	}
//}	