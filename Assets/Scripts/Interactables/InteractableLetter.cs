using UnityEngine;

public class InteractableLetter : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject letterUIPanel;

    public string GetInteractText()
    {
        return "Press E to Read";
    }

    public void OnInteract()
    {
        letterUIPanel.GetComponent<LetterUIManager>().OpenLetter();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
