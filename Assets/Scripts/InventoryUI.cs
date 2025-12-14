using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject itemSlotPrefab; // ItemSlot prefab
    public Transform itemsParent;     // ItemsParent referansı

    public void RefreshInventory(List<Sprite> potionIcons)
    {
        foreach (Transform child in itemsParent)
            Destroy(child.gameObject);

        foreach (Sprite icon in potionIcons)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemsParent);
            slot.GetComponent<Image>().sprite = icon;
        }
    }
}
