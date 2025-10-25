using UnityEngine;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private TextMeshProUGUI interactText;

    private Camera cam;
    private IInteractable currentTarget;

    // To close the interaction while the letter is open
    public bool canInteract = true;

    void Start()
    {
        cam = Camera.main;
        interactText.gameObject.SetActive(false);
    }

    void Update()
    {
        // If interactivity is disabled, do nothing
        if (!canInteract)
        {
            interactText.gameObject.SetActive(false);
            return;
        }

        Ray ray = new Ray(cam.transform.position + cam.transform.forward * 0.5f, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        if (Physics.SphereCast(ray, 0.2f, out RaycastHit hit, interactDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentTarget = interactable;
                interactText.text = interactable.GetInteractText();
                interactText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactText.gameObject.SetActive(false);
                    interactable.OnInteract();

                    // If the letter is opened, close the interaction
                    canInteract = false;
                }
            }
        }
        else
        {
            currentTarget = null;
            interactText.gameObject.SetActive(false);
        }
    }
}
