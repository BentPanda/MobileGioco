using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void StartShake(float magnitude = 0.2f)
    {
        StartCoroutine(Shake(0.2f, magnitude));  // Shake trwa 0.2 sekundy
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-1f, 1f) * magnitude;
            float yOffset = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + xOffset, originalPos.y + yOffset, originalPos.z);

            elapsedTime += Time.deltaTime;

            // oczekiwanie na nastêpny frame
            yield return null;
        }

        // Przywrócenie kamery do oryginalnej pozycji
        transform.localPosition = originalPos;
    }
}
