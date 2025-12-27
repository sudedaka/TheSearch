using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // UI Tıklamaları için gerekli

public class GameManager : MonoBehaviour
{
    [Header("Kamera Ayarı")]
    public Camera miniGameCamera; // Inspector'dan Mini Game kamerasını sürükle

    public BottleController[] bottles;
    public Color[] liquidColors;

    private BottleController selectedBottle;
    private bool isAnimating = false;

    void Start()
    {
          Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
        // 1. Kamera Güvenliği
        if (miniGameCamera == null)
        {
            miniGameCamera = Camera.main; 
        }

        // 2. EventSystem Çakışmasını Önle
        // Ana sahnedeki EventSystem'i bulup geçici olarak susturuyoruz.
        EventSystem[] systems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        foreach (var sys in systems)
        {
            // Eğer bu EventSystem benim sahnemde değilse (yani Ana Sahne'deyse) kapat.
            if (sys.gameObject.scene.name != gameObject.scene.name)
            {
                sys.gameObject.SetActive(false);
            }
        }

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

        // Şişelere doldur
        int idx = 0;
        for (int i = 0; i < filledBottleCount; i++) {
            for (int j = 0; j < 4; j++) {
                // Not: Burada Level1/Level3 mantığına göre true/false parametreni koru.
                // Ben varsayılan olarak true (gizle) gönderiyorum.
                bottles[i].PushLiquid(levelLiquids[idx], true); 
                idx++;
            }
        }
    }

    void Update()
    {
        if (isAnimating) return;

        if (Input.GetMouseButtonDown(0))
        {
            // Mini Game kamerasına göre tıklama hesabı
            Vector2 mousePos = miniGameCamera.ScreenToWorldPoint(Input.mousePosition);
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
                Color sourceColor = selectedBottle.PeekTopColor();
                int targetSpace = clickedBottle.GetCapacity() - clickedBottle.GetCount();

                if (targetSpace > 0 && 
                   (clickedBottle.GetCount() == 0 || clickedBottle.PeekTopColor() == sourceColor))
                {
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

    // --- KRİTİK DÜZELTME YAPILAN YER ---
    IEnumerator PourSequence(BottleController source, BottleController target)
    {
        isAnimating = true; 
        
        SortingGroup sourceSort = source.GetComponent<SortingGroup>();
        if (sourceSort != null) sourceSort.sortingOrder = 20;

        Vector3 originalPos = source.transform.position;
        Quaternion originalRot = source.transform.rotation;

        float direction = (target.transform.position.x > source.transform.position.x) ? -1 : 1; 
        Vector3 pourPos = target.transform.position + new Vector3(direction * 0.5f, 1.5f, 0); 
        
        float moveSpeed = 10f;
        
        // 1. Git (Unscaled Time)
        while (Vector3.Distance(source.transform.position, pourPos) > 0.1f)
        {
            source.transform.position = Vector3.MoveTowards(source.transform.position, pourPos, moveSpeed * Time.unscaledDeltaTime);
            yield return null; 
        }

        // 2. Dön (Unscaled Time)
        float rotateAngle = (direction == -1) ? -45f : 45f; 
        Quaternion targetRotation = Quaternion.Euler(0, 0, rotateAngle);
        float rotateSpeed = 5f;
        
        float t = 0;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * rotateSpeed;
            source.transform.rotation = Quaternion.Slerp(originalRot, targetRotation, t);
            yield return null;
        }

        // 3. Bekle (Realtime)
        yield return new WaitForSecondsRealtime(0.2f); 

        // 4. Dök (Realtime Beklemeli)
        Color colorGroup = source.PeekTopColor();
        int targetSpace = target.GetCapacity() - target.GetCount();

        while (source.GetCount() > 0 && source.PeekTopColor() == colorGroup && targetSpace > 0)
        {
            target.PushLiquid(source.PopLiquid());
            targetSpace--;
            
            yield return new WaitForSecondsRealtime(0.1f); 
          
            if (source.isMysteryMode) break; 
        }

        // 5. Geri Dön (Unscaled Time)
        t = 0;
        Quaternion currentRot = source.transform.rotation;
        while (t < 1)
        {
            t += Time.unscaledDeltaTime * rotateSpeed;
            source.transform.rotation = Quaternion.Slerp(currentRot, originalRot, t);
            yield return null;
        }

        // 6. Yerine Git (Unscaled Time)
        while (Vector3.Distance(source.transform.position, originalPos) > 0.1f)
        {
            source.transform.position = Vector3.MoveTowards(source.transform.position, originalPos, moveSpeed * Time.unscaledDeltaTime);
            yield return null;
        }

        source.transform.position = originalPos;
        source.transform.rotation = originalRot;
        if (sourceSort != null) sourceSort.sortingOrder = 0;

        Debug.Log("Animasyon Bitti. Kontrol yapılıyor...");

        CheckWinCondition();
        DeselectBottle();
        
        isAnimating = false; 
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
        StopAllCoroutines();
        isAnimating = false; 

        if (selectedBottle != null)
        {
            selectedBottle.transform.localScale = selectedBottle.GetOriginalScale();
            selectedBottle = null;
        }

        foreach (BottleController bottle in bottles)
        {
            bottle.ClearBottle();   
            bottle.ResetPosition(); 
        }

        GenerateLevel();
        Debug.Log("Oyun Yeniden Başlatıldı!");
    }

    // --- DETAYLI DEBUG VERSİYONU ---
    void CheckWinCondition()
    {
        bool allSolved = true;
        
        foreach (BottleController bottle in bottles)
        {
            if (!bottle.IsSolved())
            {
                allSolved = false;
                
                // Neden bitmediğini konsola yaz
                // Debug.LogWarning($"BİTMEDİ: {bottle.name} | Doluluk: {bottle.GetCount()}/{bottle.GetCapacity()}");
                
                break; 
            }
        }

        if (allSolved) 
        {
            Debug.Log("TEBRİKLER! TÜM ŞİŞELER TAMAMLANDI! Ana oyuna dönülüyor...");
            
            // Realtime bekleme için Invoke yerine Coroutine kullanabiliriz ama 
            // Invoke da TimeScale=0 iken çalışmayabilir!
            // En garantisi Coroutine başlatmaktır.
            StartCoroutine(WaitAndReturn());
        }
    }
    
    // Kazanma sonrası bekleme ve dönüş
    IEnumerator WaitAndReturn()
    {
        yield return new WaitForSecondsRealtime(2f); // 2 saniye bekle
        ReturnToMainGame();
    }

void ReturnToMainGame()
    {
        // 1. Zamanı tekrar akıt (Ana oyun canlansın)
        Time.timeScale = 1;

        // --- HATA BURADAYDI ---
        // Eski: SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        // Bu kod yanlışlıkla Ana Sahneyi hedef alıyordu çünkü aktif olan oydu.

        // --- DOĞRUSU ---
        // gameObject.scene -> "Bu scriptin bağlı olduğu sahne" demektir.
        // Yani direkt olarak "WaterSortMiniGame" sahnesini hedef alır.
        SceneManager.UnloadSceneAsync(gameObject.scene);
        
        Debug.Log("Mini Game Başarıyla Yok Edildi.");
    }
}