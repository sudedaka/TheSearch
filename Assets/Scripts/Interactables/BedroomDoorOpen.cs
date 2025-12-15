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
    public bool playMemoryOnFirstOpen = true;

    private bool isOpen = false;
    private bool hasPlayedMemory = false;
    private bool isInteractable = true;
    private bool interactTextVisible = true;

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        if (doorPivot == null)
            doorPivot = transform;

        closedRot = doorPivot.localRotation;
        openRot = Quaternion.Euler(0, openAngle, 0) * closedRot;
    }

    public void SetInteractable(bool value)
    {
        isInteractable = value;
    }

    public void SetInteractTextVisible(bool value)
    {
        interactTextVisible = value;
    }

    public string GetInteractText()
    {
        if (!isInteractable || !interactTextVisible)
            return "";

        return isOpen ? "Close Door (E)" : "Open Door (E)";
    }

    public void OnInteract()
    {
        if (!isInteractable) return;

        // 🔥 MEMORY TETİĞİ
        if (!isOpen && playMemoryOnFirstOpen && !hasPlayedMemory && memoryManager != null)
        {
            hasPlayedMemory = true;
            memoryManager.PlayMemory();
        }

        StopAllCoroutines();
        StartCoroutine(RotateDoor());
    }

    IEnumerator RotateDoor()
    {
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
    }
}
