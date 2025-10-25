using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LetterUIManager : MonoBehaviour
{
    public Volume postProcessVolume;
    private DepthOfField dof;

    private InteractionManager interactionManager;

    void Start()
    {
        if (postProcessVolume != null)
            postProcessVolume.profile.TryGet(out dof);

        
        interactionManager = FindObjectOfType<InteractionManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CloseLetter();
    }

    public void OpenLetter()
    {
        gameObject.SetActive(true);
        if (dof != null) dof.active = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //  Close interaction
        if (interactionManager != null)
            interactionManager.canInteract = false;
    }

    public void CloseLetter()
    {
        gameObject.SetActive(false);
        if (dof != null) dof.active = false;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //  Open interaction again
        if (interactionManager != null)
            interactionManager.canInteract = true;
    }
}
