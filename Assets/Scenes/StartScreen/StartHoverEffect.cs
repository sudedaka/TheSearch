using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class StartHoverEffect : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public TextMeshProUGUI text;
    public Image underline;

    Color normalColor = new Color(245f / 255f, 158f / 255f, 11f / 255f);
    Color hoverColor = Color.white;

    float underlineTargetWidth = 220f;
    float animationSpeed = 10f;

    RectTransform underlineRect;
    Coroutine underlineRoutine;

    void Start()
    {
       
        text.color = normalColor;

        underlineRect = underline.GetComponent<RectTransform>();
        underline.color = normalColor;
        underline.gameObject.SetActive(false);
        underlineRect.sizeDelta = new Vector2(0, underlineRect.sizeDelta.y);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;

        underline.color = hoverColor;
        underline.gameObject.SetActive(true);

        if (underlineRoutine != null)
            StopCoroutine(underlineRoutine);

        underlineRoutine = StartCoroutine(AnimateUnderline(underlineTargetWidth));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;

        if (underlineRoutine != null)
            StopCoroutine(underlineRoutine);

        underlineRoutine = StartCoroutine(AnimateUnderline(0f));
    }

    IEnumerator AnimateUnderline(float targetWidth)
    {
        while (Mathf.Abs(underlineRect.sizeDelta.x - targetWidth) > 0.5f)
        {
            float newWidth = Mathf.Lerp(
                underlineRect.sizeDelta.x,
                targetWidth,
                Time.deltaTime * animationSpeed
            );

            underlineRect.sizeDelta = new Vector2(
                newWidth,
                underlineRect.sizeDelta.y
            );

            yield return null;
        }

        underlineRect.sizeDelta = new Vector2(
            targetWidth,
            underlineRect.sizeDelta.y
        );

        if (targetWidth == 0f)
            underline.gameObject.SetActive(false);
    }
}
