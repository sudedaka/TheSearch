using UnityEngine;
using TMPro;
using System.Collections;

public class PotionCollector : MonoBehaviour
{
    [Header("Toplama Ayarları")]
    public float collectRange = 2f;          // Karakter ne kadar yakından toplasın
    public KeyCode collectKey = KeyCode.P;   // Toplama tuşu

    [Header("UI Referansları")]
    public TextMeshProUGUI pressText;        // "Press P" yazısı
    public TextMeshProUGUI collectedText;    // "Potion Collected!" yazısı

    [Header("Envanter Referansı")]
    public InventoryController inventory; 

    private GameObject closestPotion;        // En yakın potion referansı

    void Start()
    {
        // Oyun başında UI yazılarını gizle
        if (pressText != null) pressText.gameObject.SetActive(false);
        if (collectedText != null) collectedText.gameObject.SetActive(false);
    }

    void Update()
    {
        FindClosestPotion(); // Her frame en yakın potion’u bul

        if (closestPotion != null)
        {
            // Yakında potion varsa "Press P" yazısını göster
            if (pressText != null && !pressText.gameObject.activeSelf)
                pressText.gameObject.SetActive(true);

            // Eğer oyuncu P’ye bastıysa potion’u topla
            if (Input.GetKeyDown(collectKey))
            {
                CollectPotion();
            }
        }
        else
        {
            // Etrafta potion yoksa yazıyı gizle
            if (pressText != null && pressText.gameObject.activeSelf)
                pressText.gameObject.SetActive(false);
        }
    }

    void FindClosestPotion()
    {
        GameObject[] potions = GameObject.FindGameObjectsWithTag("Potion");
        closestPotion = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject potion in potions)
        {
            float dist = Vector3.Distance(transform.position, potion.transform.position);

            // Belirtilen mesafeden yakınsa seç
            if (dist < collectRange && dist < minDistance)
            {
                closestPotion = potion;
                minDistance = dist;
            }
        }
    }

    void CollectPotion()
{
    if (closestPotion == null) return;

    Debug.Log("Potion collected!");

    PotionPickup pickup = closestPotion.GetComponent<PotionPickup>();
    if (pickup != null && pickup.potionIcon != null && inventory != null)
    {
        inventory.AddPotion(pickup.potionIcon);
    }

    Destroy(closestPotion);

    if (pressText != null) pressText.gameObject.SetActive(false);
    if (collectedText != null)
        StartCoroutine(ShowCollectedMessage());
}


    IEnumerator ShowCollectedMessage()
    {
        collectedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);  // 2 saniye kalsın
        collectedText.gameObject.SetActive(false);
    }
}


