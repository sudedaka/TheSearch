using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door Settings")]
    public Transform door;           // Kapının kendisi
    public float openAngle = 90f;    // Ne kadar dönecek
    public float openSpeed = 2f;     // Açılma hızı

    private bool isOpen = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        if (door == null)
            door = transform;

        closedRot = door.rotation;
        openRot = Quaternion.Euler(door.eulerAngles + new Vector3(0, openAngle, 0));
    }

    public void OnInteract()
    {
        isOpen = !isOpen;  // Kapı durumunu değiştir
    }

    public string GetInteractText()
    {
        return isOpen ? "Close Door(E)" : "Open Door(E)";
    }

    void Update()
    {
        // Kapının dönüş animasyonu
        if (isOpen)
            door.rotation = Quaternion.Slerp(door.rotation, openRot, Time.deltaTime * openSpeed);
        else
            door.rotation = Quaternion.Slerp(door.rotation, closedRot, Time.deltaTime * openSpeed);
    }
}
