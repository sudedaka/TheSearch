using UnityEngine;
using TMPro;
using System.Collections;

public class LoadingDots : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float speed = 0.5f;

    Coroutine routine;

    void OnEnable()
    {
        routine = StartCoroutine(AnimateDots());
    }

    void OnDisable()
    {
        if (routine != null)
            StopCoroutine(routine);
    }

    IEnumerator AnimateDots()
    {
        string baseText = "Loading";
        int dots = 0;

        while (true)
        {
            dots = (dots + 1) % 4; // 0..3
            text.text = baseText + new string('.', dots);
            yield return new WaitForSeconds(speed);
        }
    }
}
