using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public GameObject heartPrefab;
    public Transform heartContainer;

    private readonly List<GameObject> hearts = new List<GameObject>();

    public void SetupHearts(int maxHP)
    {
        if (heartContainer == null || heartPrefab == null)
        {
            Debug.LogError("HeartContainer or HeartPrefab not assigned in HealthDisplay.");
            return;
        }

        foreach (GameObject heart in hearts)
            Destroy(heart);
        hearts.Clear();

        for (int i = 0; i < maxHP; i++)
            hearts.Add(Instantiate(heartPrefab, heartContainer));
    }

    public void UpdateHearts(int currentHP)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            Image heartImage = hearts[i].GetComponent<Image>();
            heartImage.sprite = i < currentHP ? fullHeart : emptyHeart;
        }
    }
}
