using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 20f;
    public Rigidbody2D rb;
    public GameObject explosionEffectPrefab;
    public bool isEnemyBullet = false;
    public interface IDamageable
{
    void TakeDamage(int damage);
}


    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.linearVelocity = transform.up * bulletSpeed;
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Dzia³amy tylko, gdy trafiamy w obiekt oznaczony jako "Enemy"
        if (!hitInfo.CompareTag("Enemy"))
            return;

        // U¿ywamy interfejsu, aby usun¹æ koniecznoœæ sprawdzania ka¿dego typu wroga osobno
        IDamageable damageable = hitInfo.GetComponent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(1);

        if (isEnemyBullet)
        {
            if (explosionEffectPrefab != null)
                Instantiate(explosionEffectPrefab, transform.position, transform.rotation);
        }
        else
        {
            CameraShake cameraShake = Camera.main.GetComponent<CameraShake>();
            cameraShake?.StartShake(0.2f);
        }

        Destroy(gameObject);
    }
}
