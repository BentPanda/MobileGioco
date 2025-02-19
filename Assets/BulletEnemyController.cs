using UnityEngine;
using static Bullet;

public class BulletEnemyController : MonoBehaviour, IDamageable
{
    public float speed = 2f;
    private Transform player;
    public int damage = 1;
    public int pointsValue = 10;
    public int health = 1;

    public GameObject explosionEffectPrefab;
    public AudioClip deathSound;
    public float deathSoundVolume = 2f;

    public GameObject bulletPrefab;
    public int numberOfBullets = 16;
    public float bulletSpeed = 5f;

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

        SpawnBullets();

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position, deathSoundVolume);
    }

    private void SpawnBullets()
    {
        float angleStep = 360f / numberOfBullets;
        for (int i = 0; i < numberOfBullets; i++)
        {
            Quaternion bulletRotation = Quaternion.Euler(0f, 0f, i * angleStep);
            GameObject bullet = Instantiate(bulletPrefab, transform.position, bulletRotation);

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.isEnemyBullet = true;
                bulletScript.bulletSpeed = bulletSpeed;
            }

            bullet.layer = LayerMask.NameToLayer("EnemyBullet");
        }
    }
}
