using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public List<Sprite> potionIcons = new List<Sprite>();
    public InventoryUI inventoryUI;
    public GameObject inventoryPanel; // "I" tuşuyla aç/kapat

    public void AddPotion(Sprite potionIcon)
    {
        potionIcons.Add(potionIcon);
        if (inventoryUI != null)
            inventoryUI.RefreshInventory(potionIcons);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}
