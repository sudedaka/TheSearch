using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MemoryGameManager : MonoBehaviour
{
    [Header("Ayarlar")]
    public MemoryCard cardPrefab; // Kart kalıbı
    public Sprite[] cardImages;   // Eşleşecek resimlerin listesi
    public Camera gameCamera;     // Tıklama için kamera
    
    [Header("Grid Ayarları")]
    public int rows = 2;    // Satır
    public int cols = 3;    // Sütun (Toplam kart = rows * cols)
    public float spaceX = 2.5f; // Kartlar arası boşluk X
    public float spaceY = 3.0f; // Kartlar arası boşluk Y

    // Oyun Mantığı Değişkenleri
    [HideInInspector] public bool canClick = true;
    private MemoryCard firstCard;
    private MemoryCard secondCard;
    private int matchesFound = 0;
    private int totalMatches;

    void Start()
    {
        // 1. EventSystem Çakışmasını Önle (WaterSort'taki gibi)
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
        // Toplam kaç çift olacak?
        int totalCards = rows * cols;
        totalMatches = totalCards / 2;

        if (totalCards % 2 != 0)
        {
            Debug.LogError("Kart sayısı çift olmalı! (Rows x Cols çift sayı yap)");
            return;
        }

        // Resim çiftlerini listeye ekle
        List<Sprite> deck = new List<Sprite>();
        List<int> ids = new List<int>();

        for (int i = 0; i < totalMatches; i++)
        {
            // Eğer yeterli resim yoksa başa dön
            Sprite img = cardImages[i % cardImages.Length];
            
            // Çift ekle (2 tane aynı resim)
            deck.Add(img); ids.Add(i);
            deck.Add(img); ids.Add(i);
        }

        // Listeyi Karıştır (Shuffle)
        for (int i = 0; i < deck.Count; i++)
        {
            Sprite tempS = deck[i]; int tempI = ids[i];
            int r = Random.Range(i, deck.Count);
            
            deck[i] = deck[r]; ids[i] = ids[r];
            deck[r] = tempS; ids[r] = tempI;
        }

        // Kartları Sahneye Diz (5000, 5000 merkezli)
        Vector2 startPos = new Vector2(5000 - (cols-1) * spaceX / 2, 5000 + (rows-1) * spaceY / 2);

        int index = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 pos = new Vector3(startPos.x + c * spaceX, startPos.y - r * spaceY, 0);
                MemoryCard card = Instantiate(cardPrefab, pos, Quaternion.identity);
                
                // Kartı ayarla
                card.Setup(ids[index], deck[index], this);
                index++;
            }
        }
    }

    public void CardClicked(MemoryCard card)
    {
        if (firstCard == null)
        {
            // İlk kart seçildi
            firstCard = card;
        }
        else
        {
            // İkinci kart seçildi
            secondCard = card;
            canClick = false; // Başka tıklamayı engelle
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        // Oyuncu 2. kartı görsün diye azıcık bekle
        yield return new WaitForSecondsRealtime(1.0f);

        if (firstCard.cardID == secondCard.cardID)
        {
            // EŞLEŞTİ!
            firstCard.Match();
            secondCard.Match();
            matchesFound++;

            if (matchesFound >= totalMatches)
            {
                Debug.Log("OYUN BİTTİ! KAZANDIN!");
                Invoke("ReturnToMainGame", 1.5f);
            }
        }
        else
        {
            // EŞLEŞMEDİ :(
            firstCard.FlipClose();
            secondCard.FlipClose();
        }

        // Seçimleri sıfırla
        firstCard = null;
        secondCard = null;
        canClick = true;
    }

    void ReturnToMainGame()
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync(gameObject.scene);
    }
}