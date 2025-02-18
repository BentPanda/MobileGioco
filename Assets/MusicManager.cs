using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        // sprawdzamy czy musicManager ju¿ istnieje
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Zostawia objekt przy zmianie sceny
            GetComponent<AudioSource>().Play();
        }
        else
        {
            Destroy(gameObject); // niszczy jak ju¿ jest
        }
    }
}
