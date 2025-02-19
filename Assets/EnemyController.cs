using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 1.2f;
    public GameObject explosionEffectPrefab;
    private Transform player;
    public int damage = 1;
    public int pointsValue = 25;
    public int health = 1;

    public AudioClip deathSound;
    public float deathSoundVolume = 2f;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // dodanie Scora
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.AddScore(pointsValue);
            gameManager.IncreaseCombo();
            gameManager.killCount++;
        }

        Explode();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Resetyowanie combo jak wróg trafi gracza
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.ResetCombo();
            }

            PlayerShoot playerComponent = other.GetComponent<PlayerShoot>();
            if (playerComponent != null)
            {
                playerComponent.TakeDamage(damage);

                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
                cameraShake?.StartShake(0.3f);
            }

            // Wróg te¿ umiera jak gracza trafia, ale nie dodaje mu za to punktów
            Explode();
            Destroy(gameObject);
        }
    }

    void Explode()
    {
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, deathSoundVolume);
        }
    }
}
