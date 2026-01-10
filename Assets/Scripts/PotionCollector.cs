using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class PotionCollector : MonoBehaviour
{
    [Header("Toplama Ayarları")]
    public KeyCode collectKey = KeyCode.P;

    [Header("UI Referansları")]
    public TextMeshProUGUI pressText;
    public TextMeshProUGUI collectedText;
    
    [Header("Gizlenecek UI Listesi")]
    public GameObject[] uiListToHide; 

    [Header("Envanter Referansı")]
    public InventoryController inventory;

    private GameObject potionInRange;

    public static int savedPotionCount = 0; 
    public int potionCount = 0;
    public int miniGameIndex = 0;

    // --- YENİ: Sırada yüklenecek sahnenin ismini tutan değişken ---
    public static string nextSceneToLoad = "";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetStatics()
    {
        savedPotionCount = 0;
        nextSceneToLoad = "";
    }

    void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    void OnSceneUnloaded(Scene current)
    {
        // --- KRİTİK GÜNCELLEME ---
        // Eğer sırada yüklenecek bir sahne varsa (nextSceneToLoad doluysa),
        // Ana Menü UI'ını açma, hemen o sahneyi yükle.
        if (!string.IsNullOrEmpty(nextSceneToLoad))
        {
            Debug.Log("Zincirleme Geçiş Yapılıyor: " + nextSceneToLoad);
            
            // Yeni sahneyi ekle
            SceneManager.LoadSceneAsync(nextSceneToLoad, LoadSceneMode.Additive);
            
            // Zamanın duruk kalmasını garantiye al
            Time.timeScale = 0;

            // Değişkeni sıfırla ki bir sonraki sefer karışmasın
            nextSceneToLoad = ""; 
            return; 
        }

        // Eğer sırada level yoksa, demek ki gerçekten ana oyuna döndük. UI'ları aç.
        if (uiListToHide != null)
        {
            foreach (GameObject ui in uiListToHide)
            {
                if (ui != null) ui.SetActive(true);
            }
            Debug.Log("Ana Sahne UI Listesi Geri Geldi.");
        }
    }

    void Start()
    {
        if (potionCount == 0 && savedPotionCount > 0)
             potionCount = savedPotionCount;

        if (pressText != null) pressText.gameObject.SetActive(false);
        if (collectedText != null) collectedText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        if (potionInRange != null && Input.GetKeyDown(collectKey))
        {
            CollectPotion(potionInRange);
        }
    }

/*void Update()
    {
        if (Time.timeScale == 0) return;

     
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log(" HİLE AKTİF: Tarot  Level 1 Açılıyor...");

            // UI'ları gizle
            if (uiListToHide != null) 
            {
                foreach (GameObject ui in uiListToHide)
                {
                    if (ui != null) ui.SetActive(false);
                }
            }

            SceneManager.LoadSceneAsync("TarotMiniGame", LoadSceneMode.Additive);
            
            Time.timeScale = 0;
            return; 
        }
        // -----------------------------

        if (potionInRange != null && Input.GetKeyDown(collectKey))
        {
            CollectPotion(potionInRange);
        }
    }*/




    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potion"))
        {
            potionInRange = other.gameObject;
            if (pressText != null) pressText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Potion"))
        {
            potionInRange = null;
            if (pressText != null) pressText.gameObject.SetActive(false);
        }
    }

    void CollectPotion(GameObject potion)
    {
        if (potion == null) return;

        PotionPickup pickup = potion.GetComponent<PotionPickup>();
        if (pickup != null && pickup.potionIcon != null && inventory != null)
        {
            inventory.AddPotion(pickup.potionIcon);
        }

        Destroy(potion);

        savedPotionCount++;       
        potionCount = savedPotionCount; 
        
        Debug.Log("Potion Count: " + potionCount);

        if (potionCount % 2 == 0 && potionCount <= 6)
        {
            miniGameIndex = potionCount / 2;
            
            string sceneToLoad = "";

            if (miniGameIndex == 1) sceneToLoad = "WaterSortMiniGame"; 
            else if (miniGameIndex == 2) sceneToLoad = "WaterSortMiniGameLevel2"; 
            else if (miniGameIndex == 3) sceneToLoad = "WaterSortMiniGameLevel3";           

            if (sceneToLoad != "")
            {
                if (uiListToHide != null) 
                {
                    foreach (GameObject ui in uiListToHide)
                    {
                        if (ui != null) ui.SetActive(false);
                    }
                }

                SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
                Time.timeScale = 0;
            }
        }

        if (pressText != null) pressText.gameObject.SetActive(false);
        if (collectedText != null) StartCoroutine(ShowCollectedMessage());
    }

    IEnumerator ShowCollectedMessage()
    {
        collectedText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        collectedText.gameObject.SetActive(false);
    }
}