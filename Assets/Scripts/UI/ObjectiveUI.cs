using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI objectiveText;

    private bool isCompleted = false;

    /// <summary>
    /// Yeni bir görev belirler.
    /// </summary>
    public void SetObjective(string text)
    {
        if (objectiveText == null) return;

        // Görev metnini sıfırla
        objectiveText.text = text;
        objectiveText.fontStyle = FontStyles.Normal;
        objectiveText.color = Color.white;
        isCompleted = false;
    }

    /// <summary>
    /// Görevi tamamlar (yazının üstünü çizer ve rengini değiştirir).
    /// </summary>
    public void CompleteObjective()
    {
        // Null veya zaten tamamlanmışsa dur
        if (objectiveText == null || isCompleted) return;

        // Yazıyı gri yap
        objectiveText.color = Color.gray;

        // Üstünü çizmek için RichText kullan
        objectiveText.text = $"<s>{objectiveText.text}</s>";

        // Tekrar çizilmesin
        isCompleted = true;
    }

    /// <summary>
    /// Aynı görevi yeniden aktif hale getir (örnek kullanım için).
    /// </summary>
    public void ResetObjective()
    {
        if (objectiveText == null) return;

        objectiveText.text = objectiveText.text.Replace("<s>", "").Replace("</s>", "");
        objectiveText.fontStyle = FontStyles.Normal;
        objectiveText.color = Color.white;
        isCompleted = false;
    }
}
