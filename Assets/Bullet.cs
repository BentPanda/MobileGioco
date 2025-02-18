using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 20f;
    public Rigidbody2D rb;
    public GameObject explosionEffectPrefab;

    public bool isEnemyBullet = false;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        rb.linearVelocity = transform.up * bulletSpeed;

        Destroy(gameObject, 2f);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (isEnemyBullet)
        {
            // logika pocisku
            if (hitInfo.CompareTag("Enemy"))
            {
                EnemyController enemy = hitInfo.GetComponent<EnemyController>();
                HardEnemyController hardEnemy = hitInfo.GetComponent<HardEnemyController>();
                BulletEnemyController bulletEnemy = hitInfo.GetComponent<BulletEnemyController>();

                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                }
                else if (hardEnemy != null)
                {
                    hardEnemy.TakeDamage(1);
                }
                else if (bulletEnemy != null)
                {
                    bulletEnemy.TakeDamage(1);
                }

                if (explosionEffectPrefab != null)
                {
                    Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
                }

                Destroy(gameObject);
            }
        }
        else
        {
            // zadawanie obra¿en wrogom
            if (hitInfo.CompareTag("Enemy"))
            {
                EnemyController enemy = hitInfo.GetComponent<EnemyController>();
                HardEnemyController hardEnemy = hitInfo.GetComponent<HardEnemyController>();
                BulletEnemyController bulletEnemy = hitInfo.GetComponent<BulletEnemyController>();

                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                }
                else if (hardEnemy != null)
                {
                    hardEnemy.TakeDamage(1);
                }
                else if (bulletEnemy != null)
                {
                    bulletEnemy.TakeDamage(1);
                }

                // shake camery
                CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
                cameraShake?.StartShake(0.2f);

                Destroy(gameObject);
            }
        }
    }
}
