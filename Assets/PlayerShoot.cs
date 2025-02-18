using UnityEngine;
using UnityEngine.EventSystems;


//nie chce mi siê zmieniaæ nazwy bo w innych skryptach ju¿ jest dane to, ale to jest PlayerController
public class PlayerShoot : MonoBehaviour
{
    public Camera mainCamera;
    public float rotationSpeed = 5f;
    public int maxHP = 2;
    private int currentHP;

    private SpriteRenderer playerRenderer;
    private Color playerColor;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject shootEffectPrefab;
    public GameObject deathEffectPrefab;
    public float fireRate = 0.5f;
    private float fireCooldown = 0f;

    public AudioSource firingAudioSource;

    public HealthDisplay healthDisplay;

    void Start()
    {
        maxHP = PlayerPrefs.GetInt("MaxHP", 2);
        currentHP = maxHP;

        playerRenderer = GetComponent<SpriteRenderer>();
        playerColor = LoadPlayerColor();
        if (playerRenderer != null)
        {
            playerRenderer.color = playerColor;
        }

        // wyœwietlenie hp
        healthDisplay = FindObjectOfType<HealthDisplay>();
        if (healthDisplay != null)
        {
            healthDisplay.SetupHearts(maxHP);
            healthDisplay.UpdateHearts(currentHP);
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {

    }

    public void Shoot()
    {
        if (fireCooldown <= 0f)
        {
            Instantiate(bulletPrefab, firePoint.position, transform.rotation);
            if (shootEffectPrefab != null)
            {
                Instantiate(shootEffectPrefab, firePoint.position, transform.rotation);
            }

            if (firingAudioSource != null)
            {
                firingAudioSource.Play();
            }

            fireCooldown = fireRate;
        }
    }

    void FixedUpdate()
    {
        // cooldwon strzelania
        fireCooldown -= Time.fixedDeltaTime;
        if (fireCooldown < 0f)
            fireCooldown = 0f;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        // update wyswietlanego zycia
        if (healthDisplay != null)
        {
            healthDisplay.UpdateHearts(currentHP);
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // efekt œmirci
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, transform.rotation);
        }

        // sprawenie ze gracz "zniknie"
        playerRenderer.enabled = false;
        Collider2D playerCollider = GetComponent<Collider2D>();
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        // update game managera
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.PlayerDied();
        }
    }

    private Color LoadPlayerColor()
    {
        string selectedColorName = PlayerPrefs.GetString("SelectedColor", "");

        // defaultowy color jak nie ma wybranego
        if (string.IsNullOrEmpty(selectedColorName))
        {
            return Color.white;
        }

        switch (selectedColorName)
        {
            case "Red":
                return Color.red;
            case "Green":
                return Color.green;
            case "Blue":
                return Color.blue;
            case "Purple":
                return Color.magenta;

            default:
                return Color.white;
        }
    }

    private bool IsPointerOverUIObject()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    private bool IsPointerOverUIObject(Touch touch)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touch.position;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
    