using UnityEngine;
using TMPro;
using System.Collections;

public class FinalEndSequence : MonoBehaviour
{
    [Header("Fade")]
    public CanvasGroup fadeGroup;
    public float fadeSpeed = 1f;

    [Header("Texts")]
    public TextMeshProUGUI endText;
    public TextMeshProUGUI thanksText;

    [Header("Player")]
    public MonoBehaviour playerController;
    public Camera mainCamera;

    void Awake()
    {
        fadeGroup.alpha = 0;
        endText.gameObject.SetActive(false);
        thanksText.gameObject.SetActive(false);
    }

    // BU FONKSİYON ŞART
    public void StartFinal()
    {
        StartCoroutine(FinalRoutine());
    }

    IEnumerator FinalRoutine()
    {
        // Player kontrol kapat
        if (playerController)
            playerController.enabled = false;

        // Fade to black
        while (fadeGroup.alpha < 1)
        {
            fadeGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        endText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        thanksText.gameObject.SetActive(true);
    }
}
