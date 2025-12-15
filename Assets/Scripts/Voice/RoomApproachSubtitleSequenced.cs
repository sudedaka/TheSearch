using UnityEngine;
using TMPro;
using System.Collections;

public class RoomApproachSubtitleSequenced : MonoBehaviour
{
    public AudioSource voice;
    public TextMeshProUGUI subtitleText;
    public Door door;

    [TextArea(2, 4)]
    public string[] lines;

    public float lineDelay = 2.5f;
    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;
        if (!other.CompareTag("Player")) return;

        hasPlayed = true;
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // 🔒 Kapıyı kilitle + yazıyı gizle
        if (door != null)
        {
            door.SetInteractable(false);
            door.SetInteractTextVisible(false);
        }

        if (voice) voice.Play();

        subtitleText.text = "";

        for (int i = 0; i < lines.Length; i++)
        {
            subtitleText.text = lines[i];
            yield return new WaitForSeconds(lineDelay);
        }

        subtitleText.text = "";

        // 🔓 Konuşma bitti → kapı aktif
        yield return new WaitForSeconds(0.2f);

        if (door != null)
        {
            door.SetInteractable(true);
            door.SetInteractTextVisible(true);
        }
    }
}
