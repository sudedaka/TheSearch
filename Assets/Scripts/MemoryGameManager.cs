using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Ayarlar")]
    public MemoryCard cardPrefab; // Kart kalıbı (Prefab)
    public Sprite[] cardImages;   // Eşleşecek resimlerin listesi
    public Camera gameCamera;     // Tıklama için kamera (Inspector'dan ata!)
    
    [Header("Grid Ayarları")]
    public int rows = 2;    // Satır Sayısı
    public int cols = 3;    // Sütun Sayısı
    public float spaceX = 2.5f; // Kartlar arası yatay boşluk
    public float spaceY = 3.0f; // Kartlar arası dikey boşluk

    [Header("Sonraki Level Ayarı")]
   
    public string nextLevelName = ""; 

   
    [HideInInspector] public bool canClick = true;
    private MemoryCard firstCard;
    private MemoryCard secondCard;
    private int matchesFound = 0;
    private int totalMatches;

    void Start()
    {
        // 1. EventSystem Çakışmasını önlemek
        EventSystem[] systems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        foreach (var sys in systems)
        {
           
            if (sys.gameObject.scene.name != gameObject.scene.name)
                sys.gameObject.SetActive(false);
        }

        if (gameCamera == null) gameCamera = Camera.main;

        GenerateGrid();
    }

    void GenerateGrid()
    {
        int totalCards = rows * cols;
        totalMatches = totalCards / 2;

        if (totalCards % 2 != 0)
        {
            Debug.LogError("HATA: Kart sayısı çift olmalı! (Rows x Cols çarpımı çift sayı yap)");
            return;
        }

        // Resim çiftlerini listeye ekle
        List<Sprite> deck = new List<Sprite>();
        List<int> ids = new List<int>();

        for (int i = 0; i < totalMatches; i++)
        {
            Sprite img = cardImages[i % cardImages.Length];
            deck.Add(img); ids.Add(i);
            deck.Add(img); ids.Add(i);
        }

        // Listeyi Karıştır 
        for (int i = 0; i < deck.Count; i++)
        {
            Sprite tempS = deck[i]; int tempI = ids[i];
            int r = Random.Range(i, deck.Count);
            
            deck[i] = deck[r]; ids[i] = ids[r];
            deck[r] = tempS; ids[r] = tempI;
        }

        // Kartları Sahneye Diz
        Vector2 startPos = new Vector2(5000 - (cols-1) * spaceX / 2, 5000 + (rows-1) * spaceY / 2);

        int index = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 pos = new Vector3(startPos.x + c * spaceX, startPos.y - r * spaceY, 0);
                
                MemoryCard card = Instantiate(cardPrefab, pos, Quaternion.identity);
                
               
                card.transform.SetParent(this.transform); 
                // --------------------------------------------------------

                card.Setup(ids[index], deck[index], this);
                index++;
            }
        }
    }

    public void CardClicked(MemoryCard card)
    {
        if (firstCard == null)
        {
            firstCard = card;
        }
        else
        {
            secondCard = card;
            canClick = false; 
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        if (firstCard.cardID == secondCard.cardID)
        {
            firstCard.Match();
            secondCard.Match();
            matchesFound++;

            if (matchesFound >= totalMatches)
            {
                Debug.Log("OYUN BİTTİ! KAZANDIN!");
                
                
                StartCoroutine(WaitAndFinish());
            }
        }
        else
        {
            firstCard.FlipClose();
            secondCard.FlipClose();
        }

        firstCard = null;
        secondCard = null;
        canClick = true;
    }

   
    IEnumerator WaitAndFinish()
    {
        yield return new WaitForSecondsRealtime(1.5f); 
        ReturnToMainGame();
    }

    void ReturnToMainGame()
    {
        
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            Debug.Log("Zincirleme Geçiş: " + nextLevelName);
            
            // PotionCollectora haber ver
            PotionCollector.nextSceneToLoad = nextLevelName;
            
            // Zaman hala duruk kalsın(za warudo)
            Time.timeScale = 0; 
        }
        else
        {
            // Sırada level yoksa ana oyuna dön
            Debug.Log("Tüm Mini Gameler Bitti -> Ana Sahne");
            Time.timeScale = 1;
        }

        // Bu sahneyi  yok et
        SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}