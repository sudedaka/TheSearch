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

    // --- YENİ: Anahtar Objesi ---
    [Header("Final Anahtarı")]
    public GameObject finalKeyObject; 

    private GameObject objectInRange; 

    public static int savedPotionCount = 0; 
    public int potionCount = 0;
    public int miniGameIndex = 0;

    public static string nextSceneToLoad = "";
    
    // --- YENİ: Oyun bitti mi kontrolü ---
    public static bool allMiniGamesFinished = false; 

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetStatics()
    {
        savedPotionCount = 0;
        nextSceneToLoad = "";
        allMiniGamesFinished = false;
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
        // 1. Eğer sırada başka level varsa (Zincirleme Geçiş)
        if (!string.IsNullOrEmpty(nextSceneToLoad))
        {
            Debug.Log("Zincirleme Geçiş Yapılıyor: " + nextSceneToLoad);
            SceneManager.LoadSceneAsync(nextSceneToLoad, LoadSceneMode.Additive);
            Time.timeScale = 0;
            nextSceneToLoad = ""; 
            return; 
        }

        // 2. Eğer tüm oyunlar bittiyse ANAHTARI AÇ
        if (allMiniGamesFinished && finalKeyObject != null)
        {
            finalKeyObject.SetActive(true);
            Debug.Log("🎉 BÜTÜN OYUNLAR BİTTİ! ANAHTAR ORTAYA ÇIKTI!");
        }

        // 3. UI'ları geri getir
        if (uiListToHide != null)
        {
            foreach (GameObject ui in uiListToHide)
            {
                if (ui != null) ui.SetActive(true);
            }
        }
    }

    void Start()
    {
        if (potionCount == 0 && savedPotionCount > 0)
             potionCount = savedPotionCount;

        if (pressText != null) pressText.gameObject.SetActive(false);
        if (collectedText != null) collectedText.gameObject.SetActive(false);

        // --- YENİ: Başlangıçta anahtarı gizle ---
        if (finalKeyObject != null)
        {
            finalKeyObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        // HİLE (F3 ile Level 3'ü aç)
        if (Input.GetKeyDown(KeyCode.O))
        {
            miniGameIndex = 3; 
            if (uiListToHide != null) 
            {
                foreach (GameObject ui in uiListToHide)
                    if (ui != null) ui.SetActive(false);
            }
            SceneManager.LoadSceneAsync("TarotMiniGameLevel2", LoadSceneMode.Additive);
            Time.timeScale = 0;
            return; 
        }

        // --- YENİ: Etkileşim Tuşu ---
        if (objectInRange != null && Input.GetKeyDown(collectKey))
        {
            if (objectInRange.CompareTag("Potion"))
            {
                CollectPotion(objectInRange);
            }
            else if (objectInRange.CompareTag("Key")) // Eğer anahtarsa
            {
                CollectKey(objectInRange);
            }
        }
    }

    // Çarpışma Kontrolleri
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potion") || other.CompareTag("Key"))
        {
            objectInRange = other.gameObject;
            if (pressText != null) 
            {
                pressText.text = "Press 'P' to Collect"; // Yazıyı güncelle
                pressText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Potion") || other.CompareTag("Key"))
        {
            objectInRange = null;
            if (pressText != null) pressText.gameObject.SetActive(false);
        }
    }

    // --- YENİ: Anahtar Toplama Fonksiyonu ---
    void CollectKey(GameObject keyObj)
    {
        KeyPickup pickup = keyObj.GetComponent<KeyPickup>();
        if (pickup != null && pickup.keyIcon != null && inventory != null)
        {
          
            inventory.AddPotion(pickup.keyIcon); 
        }

        Destroy(keyObj); // Sahneden sil
        Debug.Log("ANAHTAR ALINDI!");
        
       
        if (collectedText != null)
        {
            collectedText.text = "Mystery Key Collected!";
            StartCoroutine(ShowCollectedMessage());
        }
    }

    void CollectPotion(GameObject potion)
    {
        
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
                        if (ui != null) ui.SetActive(false);
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