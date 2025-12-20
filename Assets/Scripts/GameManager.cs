using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public BottleController[] bottles;
    public Color[] liquidColors;

    private BottleController selectedBottle;
    private bool isAnimating = false; // Şu an animasyon oynuyor mu?

    void Start()
    {
        GenerateLevel();
    }

void GenerateLevel()
    {
        List<Color> levelLiquids = new List<Color>();
        int filledBottleCount = bottles.Length - 2;
        
        // Renkleri hazırla
        for (int i = 0; i < filledBottleCount; i++) {
            Color colorToUse = liquidColors[i % liquidColors.Length];
            for (int j = 0; j < 4; j++) levelLiquids.Add(colorToUse);
        }
        
        // Karıştır
        for (int i = 0; i < levelLiquids.Count; i++) { 
            Color temp = levelLiquids[i];
            int r = Random.Range(i, levelLiquids.Count);
            levelLiquids[i] = levelLiquids[r];
            levelLiquids[r] = temp;
        }

        int idx = 0;
        for (int i = 0; i < filledBottleCount; i++) {
            for (int j = 0; j < 4; j++) {
                
                // İŞTE KRİTİK NOKTA BURASI:
                // İkinci parametreye 'true' gönderiyoruz.
                // Bu sayede BottleController anlıyor ki: "Bu oyun başı dizilimidir, aynı renk gelse bile GİZLE."
                bottles[i].PushLiquid(levelLiquids[idx], true); 
                
                idx++;
            }
        }
    }

    void Update()
    {
        // Eğer animasyon oynuyorsa tıklamaya izin verme!
        if (isAnimating) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                BottleController clickedBottle = hit.collider.GetComponent<BottleController>();
                if (clickedBottle != null)
                {
                    HandleBottleClick(clickedBottle);
                }
            }
        }
    }

    void HandleBottleClick(BottleController clickedBottle)
    {
        if (selectedBottle == null)
        {
            if (clickedBottle.GetCount() > 0)
            {
                selectedBottle = clickedBottle;
                // Hafif büyüme efekti
                selectedBottle.transform.localScale = selectedBottle.GetOriginalScale() * 1.1f;
            }
        }
        else
        {
            if (selectedBottle == clickedBottle)
            {
                DeselectBottle();
            }
            else
            {
                // Dökme kurallarını kontrol et
                Color sourceColor = selectedBottle.PeekTopColor();
                int targetSpace = clickedBottle.GetCapacity() - clickedBottle.GetCount();

                if (targetSpace > 0 && 
                   (clickedBottle.GetCount() == 0 || clickedBottle.PeekTopColor() == sourceColor))
                {
                    // ŞARTLAR UYUYOR -> ANIMASYONU BAŞLAT
                    StartCoroutine(PourSequence(selectedBottle, clickedBottle));
                }
                else
                {
                    Debug.Log("Dökülemez!");
                    DeselectBottle();
                }
            }
        }
    }

    // dökme animsyonu
    IEnumerator PourSequence(BottleController source, BottleController target)
    {
        isAnimating = true; // kullanıcaın animasyon ilerlerken başka yere tıklamasını engellemek için
        
        SortingGroup sourceSort = source.GetComponent<SortingGroup>();
       
        sourceSort.sortingOrder = 20;

        // 1. Pozisyonları Kaydet
        Vector3 originalPos = source.transform.position;
        Quaternion originalRot = source.transform.rotation;

        // 2. Hedef Pozisyonu Hesapla (Hedef şişenin biraz üstü ve yanı)
        // Hangi taraftan dökecek? Solundaysa soldan, sağındaysa sağdan.
        float direction = (target.transform.position.x > source.transform.position.x) ? -1 : 1; 
        Vector3 pourPos = target.transform.position + new Vector3(direction * 0.5f, 1.5f, 0); // 1.5 birim yukarı, 0.5 yana
        
        // 3. Şişeyi Hedefe Götür (Hareket Animasyonu)
        float moveSpeed = 10f;
        while (Vector3.Distance(source.transform.position, pourPos) > 0.1f)
        {
            source.transform.position = Vector3.MoveTowards(source.transform.position, pourPos, moveSpeed * Time.deltaTime);
            yield return null; // Bir sonraki kareyi bekle
        }

        // 4. Şişeyi Eğ (Rotate Animasyonu)
        float rotateAngle = (direction == -1) ? -45f : 45f; // Sağa veya sola eğ
        Quaternion targetRotation = Quaternion.Euler(0, 0, rotateAngle);
        float rotateSpeed = 5f;
        
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * rotateSpeed;
            source.transform.rotation = Quaternion.Slerp(originalRot, targetRotation, t);
            yield return null;
        }

        // 5. SIVIYI AKTAR (Logic Kısmı)
        // Animasyon bitince sıvıyı gerçekten döküyoruz
        yield return new WaitForSeconds(0.2f); // Dökülüyormuş gibi azıcık bekle

        Color colorGroup = source.PeekTopColor();
        int targetSpace = target.GetCapacity() - target.GetCount();

        while (source.GetCount() > 0 && 
               source.PeekTopColor() == colorGroup && 
               targetSpace > 0)
        {
            target.PushLiquid(source.PopLiquid());
            targetSpace--;
            yield return new WaitForSeconds(0.1f); // Her birim sıvı için pıt-pıt bekleme süresi
          
            if (source.isMysteryMode) 
            {
                break; 
            }
        }
        

        // 6. Şişeyi Düzelt (Geri Dönüş Başlıyor)
        t = 0;
        Quaternion currentRot = source.transform.rotation;
        while (t < 1)
        {
            t += Time.deltaTime * rotateSpeed;
            source.transform.rotation = Quaternion.Slerp(currentRot, originalRot, t);
            yield return null;
        }

        // 7. Şişeyi Yerine Götür
        while (Vector3.Distance(source.transform.position, originalPos) > 0.1f)
        {
            source.transform.position = Vector3.MoveTowards(source.transform.position, originalPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Her şeyin tam oturduğundan emin ol
        source.transform.position = originalPos;
        source.transform.rotation = originalRot;
        
        sourceSort.sortingOrder = 0;

        CheckWinCondition();
        DeselectBottle();
        
        isAnimating = false; // Tıklamayı tekrar aç
    }

    void DeselectBottle()
    {
        if (selectedBottle != null)
        {
            selectedBottle.transform.localScale = selectedBottle.GetOriginalScale();
            selectedBottle = null;
        }
    }

   public void RestartGame()
    {
        // 1. Devam eden tüm animasyonları (dökme işlemini) anında durdur
        StopAllCoroutines();
        isAnimating = false; // Tıklama kilidini aç

        // 2. Eğer o an seçili (büyümüş) bir şişe varsa seçimini kaldır
        if (selectedBottle != null)
        {
            selectedBottle.transform.localScale = selectedBottle.GetOriginalScale();
            selectedBottle = null;
        }

        // 3. Bütün şişeleri tek tek gez
        foreach (BottleController bottle in bottles)
        {
            bottle.ClearBottle();   // İçindeki sıvıyı boşalt
            bottle.ResetPosition(); // KONUMUNU VE DÖNMESİNİ SIFIRLA (Yeni Eklediğimiz)
        }

        // 4. Leveli baştan oluştur
        GenerateLevel();
        
        Debug.Log("Oyun ve Pozisyonlar Sıfırlandı!");
    }

    void CheckWinCondition()
    {
        bool allSolved = true;
        foreach (BottleController bottle in bottles)
        {
            if (!bottle.IsSolved())
            {
                allSolved = false;
                break;
            }
        }
        if (allSolved) Debug.Log("TEBRİKLER! OYUN BİTTİ!");
    }
}