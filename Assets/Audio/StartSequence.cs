using UnityEngine;
using System.Collections;
using StarterAssets;
using TMPro;

public class StartSequence : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip answeringMachineClip;
    public AudioClip bethVoiceClip;

    public AudioSource audioSource;

    [Header("Player")]
    public ThirdPersonController movementController;

    [Header("Subtitles")]
    public TMP_Text subtitleText;

    void Start()
    {
        if (movementController != null)
            movementController.AllowMovement = false;

        if (subtitleText != null)
            subtitleText.alpha = 0f;

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // 1️⃣ ANSWERING MACHINE
        ShowSubtitle("<i>Hello. No one is available to take your call.\nPlease leave your message after the tone.</i>");
        audioSource.PlayOneShot(answeringMachineClip);
        yield return new WaitForSeconds(answeringMachineClip.length);

        HideSubtitle();
        yield return new WaitForSeconds(0.4f);

        // 2️⃣ BETH – TEK CÜMLE
        ShowSubtitle("I can't reach my father, I hope he's home.");
        audioSource.PlayOneShot(bethVoiceClip);
        yield return new WaitForSeconds(bethVoiceClip.length);

        HideSubtitle();

        if (movementController != null)
            movementController.AllowMovement = true;
    }


    IEnumerator ShowLine(string line)
    {
        ShowSubtitle(line);
        yield return new WaitForSeconds(1.2f);
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
