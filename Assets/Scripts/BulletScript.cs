using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private float bulletSpeed;
    private float range;
    private Vector2 moveDirection;

    private float lifeTime;
    private float damage = 1f; 

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            AbstractEnemy enemy = collision.GetComponent<AbstractEnemy>();
            if (enemy != null)
            {
                enemy.reciveDmg(damage); 
            }

            Destroy(gameObject);
        }

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
