using UnityEngine;
using TMPro;
using System.Collections;

public class PotionCollector : MonoBehaviour
{
    [Header("Toplama Ayarları")]
    public KeyCode collectKey = KeyCode.P;   // Toplama tuşu

    [Header("UI Referansları")]
    public TextMeshProUGUI pressText;        // "Press P" yazısı
    public TextMeshProUGUI collectedText;    // "Potion Collected!" yazısı

    [Header("Envanter Referansı")]
    public InventoryController inventory;

    private GameObject potionInRange;        // Yakındaki potion referansı

    void Start()
    {
        if (pressText != null) pressText.gameObject.SetActive(false);
        if (collectedText != null) collectedText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Sadece yakınında potion varsa ve tuşa basılmışsa
        if (potionInRange != null && Input.GetKeyDown(collectKey))
        {
            CollectPotion(potionInRange);
        }
    }

    // 🔹 Karakter potion’un trigger alanına girince
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potion"))
        {
            potionInRange = other.gameObject;
            if (pressText != null)
                pressText.gameObject.SetActive(true);
        }
    }

    // 🔹 Karakter trigger alanından çıkınca
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Potion"))
        {
            potionInRange = null;
            if (pressText != null)
                pressText.gameObject.SetActive(false);
        }
    }

    void CollectPotion(GameObject potion)
    {
        if (potion == null) return;

        Debug.Log("Potion collected!");

        PotionPickup pickup = potion.GetComponent<PotionPickup>();
        if (pickup != null && pickup.potionIcon != null && inventory != null)
        {
            inventory.AddPotion(pickup.potionIcon);
        }

        Destroy(potion);

        if (pressText != null) pressText.gameObject.SetActive(false);
        if (collectedText != null)
            StartCoroutine(ShowCollectedMessage());
    }

    IEnumerator ShowCollectedMessage()
    {
        collectedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        collectedText.gameObject.SetActive(false);
    }
}
