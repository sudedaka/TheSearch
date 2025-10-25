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
        // Open letter
        letterUIPanel.GetComponent<LetterUIManager>().OpenLetter();

        // Mission complete
        var objectiveUI = FindObjectOfType<ObjectiveUI>();
        if (objectiveUI != null)
            objectiveUI.CompleteObjective();

        // Stop the game
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
