using UnityEngine;
using static Bullet;

public class HardEnemyController : MonoBehaviour, IDamageable
{
    public float speed = 0.8f;
    private Transform player;
    public int damage = 1;
    public int pointsValue = 100;
    public int health = 2;
    public GameObject explosionEffectPrefab;
    public AudioClip deathSound;
    public float deathSoundVolume = 2f;

    private void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;
    }

    private void Update()
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
            Die();
    }

    private void Die()
    {
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager?.ResetCombo();

            PlayerShoot playerComponent = other.GetComponent<PlayerShoot>();
            if (playerComponent != null)
            {
                playerComponent.TakeDamage(damage);
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
                cameraShake?.StartShake(0.3f);
            }
            Explode();
            Destroy(gameObject);
        }
    }

    private void Explode()
    {
        if (explosionEffectPrefab != null)
            Instantiate(explosionEffectPrefab, transform.position, transform.rotation);

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position, deathSoundVolume);
    }
}
