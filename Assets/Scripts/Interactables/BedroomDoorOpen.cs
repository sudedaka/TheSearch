using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;

    [Header("Memory")]
    public MemoryManager memoryManager;

    private bool isOpen = false;
    private bool memoryPlayed = false;
    private bool isBusy = false;

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        if (!doorPivot)
            doorPivot = transform;

        closedRot = doorPivot.localRotation;
        openRot = Quaternion.Euler(0, openAngle, 0) * closedRot;
    }

    // 🔤 UI YAZISI BURADAN GELİR
    public string GetInteractText()
    {
        return isOpen ? "Close Door (E)" : "Open Door (E)";
    }

    // 🔑 PLAYER E'YE BASINCA ÇAĞRILAN
    public void OnInteract()
    {
        if (isBusy) return;

        // 🧠 MEMORY SADECE İLK AÇILIŞTA
        if (!isOpen && !memoryPlayed && memoryManager != null)
        {
            memoryPlayed = true;
            memoryManager.PlayMemory();
        }

        StopAllCoroutines();
        StartCoroutine(RotateDoor());
    }

    IEnumerator RotateDoor()
    {
        isBusy = true;
        isOpen = !isOpen;

        Quaternion start = doorPivot.localRotation;
        Quaternion target = isOpen ? openRot : closedRot;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorPivot.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        isBusy = false;
    }
}
