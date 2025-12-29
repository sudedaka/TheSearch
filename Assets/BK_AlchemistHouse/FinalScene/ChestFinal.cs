using UnityEngine;

public class ChestFinal : MonoBehaviour
{
    public Animator chestAnimator;

    [Header("Potion")]
    public Transform potionSpawnPoint;
    public GameObject potionObject; // SAHNEDEKİ Vial_LP

    [Header("UI")]
    public GameObject pressECanvas;

    private bool opened = false;
    private bool playerInRange = false;

    void Awake()
    {
        if (!chestAnimator)
            chestAnimator = GetComponent<Animator>();

        if (pressECanvas)
            pressECanvas.SetActive(false);

        if (potionObject)
            potionObject.SetActive(false); // 🔒 başta kapalı
    }

    void Update()
    {
        if (opened) return;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            opened = true;

            if (pressECanvas)
                pressECanvas.SetActive(false);

            chestAnimator.SetTrigger("Open");

            Invoke(nameof(SpawnPotion), 3f); // kapağın açılma anına göre ayarla
        }
    }

    void SpawnPotion()
    {
        potionObject.transform.position = potionSpawnPoint.position;
        potionObject.SetActive(true);

        PotionFloatUp floatUp = potionObject.GetComponent<PotionFloatUp>();
        if (floatUp != null)
            floatUp.StartFloating();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!opened && other.CompareTag("Player"))
        {
            playerInRange = true;
            if (pressECanvas)
                pressECanvas.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (pressECanvas)
                pressECanvas.SetActive(false);
        }
    }
}
