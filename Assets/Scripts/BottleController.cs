using System.Collections.Generic;
using UnityEngine;

public class BottleController : MonoBehaviour
{
    // Şişe içindeki sıvıların görselleri için (0 en alt, 3 en üst)
    public SpriteRenderer[] liquidRenderers;
    public GameObject[] questionMarks;

    [Header("Gizemli Mod Ayarları")]
    public bool isMysteryMode = false;
    public Color mysteryColor = Color.gray;

    // Şişedeki renk verilerini tutan Stack
    public Stack<Color> liquidStack = new Stack<Color>();
    
    // YENİ: Hangi katmanın görünür olduğu bilgisini tutan liste
    // (True: Görünür, False: Soru İşareti)
    private List<bool> revealedLayers = new List<bool>();

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

    void Start()
    {
        UpdateVisuals();
    }

    // Şişeye renk ekleme fonksiyonu
    // isLevelStart = true ise bu işlem oyun başında yapılıyor demektir (Zorla gizle)
    public void PushLiquid(Color color, bool isLevelStart = false)
    {
        if (liquidStack.Count < capacity)
        {
            if (isLevelStart)
            {
                // LEVEL OLUŞTURMA ANI:
                // Yeni gelen sıvıyı listeye ekle ama "Gizli" (false) olarak işaretle.
                // En tepedeki kuralı UpdateVisuals'da işleyeceği için sorun yok.
                revealedLayers.Add(false);
            }
            else
            {
                // OYUN ANI (Normal Dökme):
                // 1. Yeni gelen sıvı her zaman görünürdür (En üstte olduğu için).
                revealedLayers.Add(true);

                // 2. Eğer altındaki sıvı (Count - 1) aynı renkse, onun da kilidini aç!
                if (liquidStack.Count > 0 && liquidStack.Peek() == color)
                {
                    revealedLayers[liquidStack.Count - 1] = true;
                }
            }

            // Stack'e rengi ekle
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
            
            // Sıvı gidince onun görünürlük bilgisini de listeden siliyoruz
            if (revealedLayers.Count > 0)
            {
                revealedLayers.RemoveAt(revealedLayers.Count - 1);
            }

            UpdateVisuals();
            return color;
        }
        return Color.clear;
    }

    public Color PeekTopColor()
    {
        if (liquidStack.Count > 0)
            return liquidStack.Peek();
        return Color.clear;
    }

    public int GetCount() { return liquidStack.Count; }
    public int GetCapacity() { return capacity; }

    // Görselleri güncelleme
    private void UpdateVisuals()
    {
        Color[] currentColors = liquidStack.ToArray();
        System.Array.Reverse(currentColors); // [Dip, ..., Tepe]

        // Görünürlük listesini de ters çevirip dizi yapıyoruz ki indexler tutsun
        bool[] currentRevealed = revealedLayers.ToArray();
        // Stack yapısı olduğu için revealedLayers [Dip, ..., Tepe] sırasındadır,
        // Ancak Stack.ToArray() LIFO (Tepe...Dip) verir, sonra Reverse ile (Dip...Tepe) yaptık.
        // List zaten (Dip...Tepe) tuttuğu için Reverse etmemize GEREK YOKTUR, ama garanti olsun.
        // DÜZELTME: List.Add ile sona ekliyoruz. Yani List[0] en alt, List[Son] en üst.
        // currentColors[0] da en alt. Yani sıralama aynı.

        for (int i = 0; i < liquidRenderers.Length; i++)
        {
            bool hasQM = (questionMarks != null && i < questionMarks.Length && questionMarks[i] != null);

            // Önce temizlik
            if (hasQM) questionMarks[i].SetActive(false);
            liquidRenderers[i].gameObject.SetActive(false);

            if (i < currentColors.Length)
            {
                liquidRenderers[i].gameObject.SetActive(true);

                // GÖRÜNME KURALI:
                // 1. Mod kapalıysa -> GÖSTER
                // 2. Bu parça en tepedeyse -> GÖSTER (Her zaman ucunu görürüz)
                // 3. 'revealedLayers' listesinde true ise -> GÖSTER
                
                bool isTop = (i == currentColors.Length - 1);
                bool isRevealed = currentRevealed[i];

                if (!isMysteryMode || isTop || isRevealed)
                {
                    // GÖRÜNÜR
                    liquidRenderers[i].color = currentColors[i];
                }
                else
                {
                    // GİZLİ
                    liquidRenderers[i].color = mysteryColor;
                    if (hasQM) questionMarks[i].SetActive(true);
                }
            }
        }
    }

    public bool IsSolved()
    {
        if (liquidStack.Count == 0) return true;
        if (liquidStack.Count < capacity) return false;
        Color topColor = liquidStack.Peek();
        foreach (Color color in liquidStack)
        {
            if (color != topColor) return false;
        }
        return true;
    }

    public void ClearBottle()
    {
        liquidStack.Clear();
        revealedLayers.Clear(); // Listeyi de temizle
        UpdateVisuals();
    }

    public Vector3 GetOriginalScale() { return originalScale; }

    public void ResetPosition()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        transform.localScale = originalScale;
    }
    
}