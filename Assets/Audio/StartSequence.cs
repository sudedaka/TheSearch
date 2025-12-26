using UnityEngine;
using System.Collections;
using StarterAssets;
using TMPro;

public class StartSequence : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip bethVoiceClip;
    public AudioSource audioSource;

    [Header("Subtitles")]
    public TMP_Text subtitleText;

    void Start()
    {
        if (subtitleText != null)
            subtitleText.alpha = 0f;

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
     
        yield return new WaitForSeconds(2.0f);

   
        ShowSubtitle("Everything is as he left it...");
        audioSource.PlayOneShot(bethVoiceClip);

        yield return new WaitForSeconds(bethVoiceClip.length);

        HideSubtitle();
    }

    void ShowSubtitle(string text)
    {
        if (subtitleText == null) return;

        subtitleText.text = text;
        subtitleText.alpha = 1f;
    }

    void HideSubtitle()
    {
        if (subtitleText == null) return;

        subtitleText.alpha = 0f;
    }
}
