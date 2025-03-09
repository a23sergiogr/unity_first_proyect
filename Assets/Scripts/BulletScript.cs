using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private float bulletSpeed;
    private float range;
    private Vector2 moveDirection;
    private float lifeTime;
    private float damage;
    private bool isEnemyBullet = false;
    private Sprite sprite;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        lifeTime = Time.time + range;
    }

    void Update()
    {
        transform.Translate(moveDirection * bulletSpeed * Time.deltaTime);

        if (Time.time >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void SetBulletSpeed(float speed)
    {
        bulletSpeed = speed;
    }

    public void SetRange(float bulletRange)
    {
        range = bulletRange;
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void SetIsEnemyBullet(bool isEnemyBullet)
    {
        this.isEnemyBullet = isEnemyBullet;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    public void SetSprite(Sprite newSprite)
    {

        spriteRenderer = GetComponent<SpriteRenderer>();

        sprite = newSprite;
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }

        if (!isEnemyBullet)
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.reciveDmg(damage);
                }

                Destroy(gameObject);
            }
        }
        else
        {
            if (collision.CompareTag("Player"))
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(Mathf.CeilToInt(damage));
                }

                Destroy(gameObject);
            }
        }
    }
}
