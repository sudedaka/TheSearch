using UnityEngine;

public class MemoryCard : MonoBehaviour
{
    [Header("Görsel Referanslar")]
    public GameObject backObject;  // Kartın arkası (Kapak)
    public SpriteRenderer frontRenderer; // Kartın önü (Resim)

    [HideInInspector] public int cardID; // Eşleşme kontrolü için kimlik
    private MemoryGameManager gameManager;
    private bool isFlipped = false; // Şu an açık mı?

    public void Setup(int id, Sprite image, MemoryGameManager manager)
    {
        cardID = id;
        frontRenderer.sprite = image;
        gameManager = manager;
    }

    private void OnMouseDown()
    {
        // Eğer zaten açıksa veya oyun kilitliyse (2 kart açılmışsa) tıklama
        if (isFlipped || gameManager.canClick == false) return;

        FlipOpen();
        gameManager.CardClicked(this);
    }

    public void FlipOpen()
    {
        backObject.SetActive(false); // Kapağı gizle -> Resim gözüksün
        isFlipped = true;
    }

    public void FlipClose()
    {
        backObject.SetActive(true); // Kapağı aç -> Resim gizlensin
        isFlipped = false;
    }

    // Kartlar eşleşince çağrılacak (Örn: hafif silikleşsin veya yok olsun)
    public void Match()
    {
        // Tıklanamaz hale getir
        GetComponent<BoxCollider2D>().enabled = false; 
        
        // İstersen kartı yok edebilirsin:
        // Destroy(gameObject, 0.5f);
        
        // Veya rengini gri yapıp bırakabilirsin:
        frontRenderer.color = Color.gray;
    }
}