using UnityEngine;
using System.Collections;

public class MemoryTrigger : MonoBehaviour
{
    public GameObject memoryScene;
    public Camera memoryCamera;
    public Camera playerCamera;
    public float memoryDuration = 10f;

    bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(PlayMemory());
        }
    }

    IEnumerator PlayMemory()
    {
        memoryScene.SetActive(true);

        playerCamera.gameObject.SetActive(false);
        memoryCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(memoryDuration);

        memoryCamera.gameObject.SetActive(false);
        playerCamera.gameObject.SetActive(true);

        memoryScene.SetActive(false);
    }
}
