using UnityEngine;

public class RoomApproachVoice : MonoBehaviour
{
    public AudioSource voice;
    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;

        if (other.CompareTag("Player"))
        {
            hasPlayed = true;

            if (voice != null)
                voice.Play();
        }
    }
}
