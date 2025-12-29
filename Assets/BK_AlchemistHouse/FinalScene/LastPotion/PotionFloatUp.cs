using UnityEngine;

public class PotionFloatUp : MonoBehaviour
{
    public float riseSpeed = 0.5f;
    public float rotateSpeed = 30f;
    public float maxHeight = 0.5f;

    private Vector3 startPos;
    private bool rising = false;

    public void StartFloating()
    {
        startPos = transform.position; // 🔥 SADECE BURADA AL
        rising = true;
    }

    void Update()
    {
        if (!rising) return;

        transform.position += Vector3.up * riseSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        if (transform.position.y >= startPos.y + maxHeight)
            rising = false;
    }
}
