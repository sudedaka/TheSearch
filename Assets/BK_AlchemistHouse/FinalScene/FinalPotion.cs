using UnityEngine;

public class FinalPotion : MonoBehaviour
{
    public GameObject pressPCanvas;     // "Press P" yazısı
    public FinalEndSequence finalEnd;   // FinalCanvas scripti

    private bool playerInRange = false;
    private bool used = false;

    void Start()
    {
        if (pressPCanvas)
            pressPCanvas.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange || used) return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            used = true;

            if (pressPCanvas)
                pressPCanvas.SetActive(false);

            if (finalEnd)
                finalEnd.StartFinal(); //  final başlar
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (pressPCanvas)
                pressPCanvas.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (pressPCanvas)
                pressPCanvas.SetActive(false);
        }
    }
}
