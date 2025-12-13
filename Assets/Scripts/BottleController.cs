using System.Collections.Generic;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    // Şişe içindeki sıvıların görselleri için (0 en alt, 3 en üst)
    public SpriteRenderer[] liquidRenderers;

    // Şişedeki renk verilerini tutan Stack (Last In First Out)
    public Stack<Color> liquidStack = new Stack<Color>();

    // Şişenin alabileceği max sıvı sayısı
    private int capacity = 4;


    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    void Awake() 
    {
      
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
    }
    // -------------------------------------------------------

    void Start()
    {
        UpdateVisuals();
            
        //Test part for making sure one bottle has luqids in it an other has.

/*

if (gameObject.name == "bottle")

{


PushLiquid(Color.blue);

PushLiquid(Color.red);

PushLiquid(Color.yellow);

}

// Başlangıçta görselleri güncelle

// liquidStack.Push(Color.blue);

UpdateVisuals(); // şişelerdeki renklerin oyun başladığında gözükmemeisini sağlıyor

*/
    }

    // Şişeye renk ekleme fonksiyonu
    public void PushLiquid(Color color)
    { 
        if (liquidStack.Count < capacity)
        {
            liquidStack.Push(color);
            UpdateVisuals();
        }
    }

    // Şişeden renk alma fonksiyonu
    public Color PopLiquid()
    {
        if (liquidStack.Count > 0)
        {
            Color color = liquidStack.Pop();
            UpdateVisuals();
            return color;
        }
        return Color.clear; // Boşsa şeffaf dön
    }

    // En üstteki renge bakma (Çıkarmaz, sadece bakar)
    public Color PeekTopColor()
    {
        if (liquidStack.Count > 0)
            return liquidStack.Peek();
        return Color.clear;
    }

    public int GetCount() { return liquidStack.Count; }
    public int GetCapacity() { return capacity; }

    // Görselleri yığın verisine göre güncelleme
    private void UpdateVisuals()
    {
        Color[] currentColors = liquidStack.ToArray();
        System.Array.Reverse(currentColors);

        for (int i = 0; i < liquidRenderers.Length; i++)
        {
            if (i < currentColors.Length)
            {
                liquidRenderers[i].color = currentColors[i];
                liquidRenderers[i].gameObject.SetActive(true); // Görünür yap
            }
            else
            {
                liquidRenderers[i].gameObject.SetActive(false); // Boş kısımları gizle
            }
        }
    }
    
    // Şişe çözüldü mü? (Ya tamamen boş olacak, ya da tamamen dolu ve tek renk olacak)
    public bool IsSolved()
    {
        // 1. Durum: Şişe boşsa çözülmüş sayılır.
        if (liquidStack.Count == 0) return true;

        // 2. Durum: Şişe dolu değilse (örn: 3 birim varsa) çözülmüş sayılamaz.
        if (liquidStack.Count < capacity) return false;

        // 3. Durum: Şişe dolu, peki renkler aynı mı?
        Color topColor = liquidStack.Peek();
        
        foreach (Color color in liquidStack)
        {
            if (color != topColor) return false; // Farklı renk varsa olmamıştır
        }

        return true; // Buraya kadar geldiyse hepsi aynı renktir.
    }
    
    // Şişenin içini tamamen boşaltır
    public void ClearBottle()
    {
        liquidStack.Clear(); // Stack verisini sil
        UpdateVisuals();     // Görüntüyü güncelle (Boş hale getir)
    }
    public Vector3 GetOriginalScale()
    {
        return originalScale;
    }
    // Restart tuşuna basıldığında GameManager bu fonksiyonu çağıracak.
    public void ResetPosition()
    {
        // Şişeyi başlangıçtaki konumuna ve açısına ışınla
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        
        // Eğer seçili olduğu için büyümüşse, boyutunu normale döndür
        transform.localScale = originalScale;
    }
}