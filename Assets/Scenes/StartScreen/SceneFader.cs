using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 1.5f;

    [Header("UI")]
    public GameObject loadingText;
    public GameObject startText;
    public GameObject startUnderline;

    [Header("Audio")]
    public AudioSource startMusic;   // MusicPlayer AudioSource
    public AudioSource sfxSource;    // SceneFader üzerindeki AudioSource
    public AudioClip windClip;
    public AudioClip doorClip;

    void Awake()
    {
        SetAlpha(0f);

        if (loadingText != null)
            loadingText.SetActive(false);
    }

    public void FadeToScene(string sceneName)
    {
        // Start müziğini kes
        if (startMusic != null)
            startMusic.Stop();

        // UI kapat
        if (startText != null)
            startText.SetActive(false);

        if (startUnderline != null)
            startUnderline.SetActive(false);

        if (loadingText != null)
            loadingText.SetActive(true);

        // Rüzgar sesi
        if (sfxSource != null && windClip != null)
            sfxSource.PlayOneShot(windClip);

        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }

       
        if (sfxSource != null && doorClip != null)
        {
            sfxSource.PlayOneShot(doorClip);
            yield return new WaitForSeconds(doorClip.length);
        }

        if (sfxSource != null)
            sfxSource.Stop();

        SceneManager.LoadScene(sceneName);
    }




    void SetAlpha(float a)
    {
        if (fadeImage == null) return;

        Color c = fadeImage.color;
        c.a = a;
        fadeImage.color = c;
    }
}
