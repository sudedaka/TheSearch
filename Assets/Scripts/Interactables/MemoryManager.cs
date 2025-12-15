using UnityEngine;
using System.Collections;

public class MemoryManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera playerCamera;
    public Camera memoryCamera;

    [Header("Player")]
    public MonoBehaviour playerController;

    [Header("Fade")]
    public CanvasGroup fadeCanvas;

    [Header("Dialogue Audio")]
    public AudioSource bethVoice;
    public AudioSource fatherVoice;

    [Header("Dialogue Subtitle")]
    public MemoryDialogueSubtitle memorySubtitle; // 🔥 SADECE BU EKLENDİ

    private bool isPlaying = false;

    void Start()
    {
        if (bethVoice) bethVoice.Stop();
        if (fatherVoice) fatherVoice.Stop();
    }

    public void PlayMemory()
    {
        if (isPlaying) return;
        StartCoroutine(MemoryRoutine());
    }

    IEnumerator MemoryRoutine()
    {
        isPlaying = true;

        if (playerController)
            playerController.enabled = false;

        yield return StartCoroutine(Fade(1f, 0.4f));

        playerCamera.gameObject.SetActive(false);
        memoryCamera.gameObject.SetActive(true);

        yield return StartCoroutine(Fade(0f, 0.4f));

        // 🔹 BETH
        if (bethVoice && bethVoice.clip)
        {
            bethVoice.Play();

            if (memorySubtitle != null)
                yield return StartCoroutine(
                    memorySubtitle.ShowBeth(bethVoice.clip.length)
                );
            else
                yield return new WaitForSeconds(bethVoice.clip.length);
        }

        yield return new WaitForSeconds(0.3f);

        // 🔹 FATHER
        if (fatherVoice && fatherVoice.clip)
        {
            fatherVoice.Play();

            if (memorySubtitle != null)
                yield return StartCoroutine(
                    memorySubtitle.ShowFather(fatherVoice.clip.length)
                );
            else
                yield return new WaitForSeconds(fatherVoice.clip.length);
        }

        yield return StartCoroutine(Fade(1f, 0.4f));

        memoryCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        yield return StartCoroutine(Fade(0f, 0.4f));

        if (playerController)
            playerController.enabled = true;

        isPlaying = false;
    }

    IEnumerator Fade(float target, float duration)
    {
        if (!fadeCanvas) yield break;

        float start = fadeCanvas.alpha;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            fadeCanvas.alpha = Mathf.Lerp(start, target, t);
            yield return null;
        }
    }
}
