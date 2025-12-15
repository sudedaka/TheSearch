using UnityEngine;
using TMPro;
using System.Collections;

public class MemoryDialogueSubtitle : MonoBehaviour
{
    public TextMeshProUGUI subtitleText;

    [TextArea(2, 4)]
    public string bethLine;

    [TextArea(2, 4)]
    public string fatherLine;

    void Awake()
    {
        Clear();
    }

    public IEnumerator ShowBeth(float duration)
    {
        subtitleText.text = bethLine;
        yield return new WaitForSeconds(duration);
        subtitleText.text = "";
    }

    public IEnumerator ShowFather(float duration)
    {
        subtitleText.text = fatherLine;
        yield return new WaitForSeconds(duration);
        subtitleText.text = "";
    }

    public void Clear()
    {
        if (subtitleText)
            subtitleText.text = "";
    }
}
