using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class LetterUIManager : MonoBehaviour
{
    [Header("Post Process")]
    public Volume postProcessVolume;
    private DepthOfField dof;

    [Header("Audio")]
    public AudioSource voiceSource;
    public AudioClip letterVoiceClip;

    [Header("Subtitle")]
    public TMP_Text letterSubtitle;
    [TextArea(3, 6)]
    public string subtitleText;

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

        if (dof != null)
            dof.active = true;

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (interactionManager != null)
            interactionManager.canInteract = false;

        //  Ses + Text
        if (voiceSource != null && letterVoiceClip != null)
        {
            voiceSource.ignoreListenerPause = true;
            voiceSource.PlayOneShot(letterVoiceClip);
        }

        if (letterSubtitle != null)
        {
            letterSubtitle.text = subtitleText;
            letterSubtitle.gameObject.SetActive(true);
        }
    }

    public void CloseLetter()
    {
        gameObject.SetActive(false);

        if (dof != null)
            dof.active = false;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (interactionManager != null)
            interactionManager.canInteract = true;

        if (voiceSource != null)
            voiceSource.Stop();

        if (letterSubtitle != null)
            letterSubtitle.gameObject.SetActive(false);
    }
}
