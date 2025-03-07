using System.Collections;
using UnityEngine;

public class BossScript : MonoBehaviour, Enemy
{
    [SerializeField] public Transform player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject Hitbox;
    private float attackCooldown = 2f;
    private float jumpForce = 10f;
    private float jumpCooldown = 3f;
    private float lifePoints = 40f;
    private float contactDmg = 1f;
    private float bulletDmg = 1f;
    private float nextAttackTime;
    private bool isJumping = false;
    private bool isAttacking = false; // Nuevo: Estado para controlar si el jefe está atacando
    private Collider2D bossCollider;
    private SpriteRenderer bossSprite;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossCollider = GetComponent<Collider2D>();
        bossSprite = GetComponent<SpriteRenderer>();
        nextAttackTime = Time.time + attackCooldown;
    }

    void Update()
    {
        if (!isAttacking && Time.time >= nextAttackTime) // Solo atacar si no está atacando ya
        {
            int attackType = Random.Range(0, 4); // Cambiado a 4 para incluir el nuevo ataque
            switch (attackType)
            {
                case 0:
                    StartCoroutine(ShootWave());
                    break;
                case 1:
                    StartCoroutine(JumpAttack());
                    break;
                case 2:
                    StartCoroutine(SpinAttack());
                    break;
                case 3:
                    StartCoroutine(ChargeAttack()); // Nuevo ataque
                    break;
            }
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private IEnumerator ShootWave()
    {
        isAttacking = true; // Bloquear nuevos ataques
        for (int i = 0; i < 3; i++)
        {
            ShootPattern(i % 2 == 0 ? "X" : "+");
            yield return new WaitForSeconds(0.5f);
        }
        isAttacking = false; // Permitir nuevos ataques
    }

    private void ShootPattern(string pattern)
    {
        int bullets = 4;
        float angleStep = 360f / bullets;

        for (int i = 0; i < bullets; i++)
        {
            float angle = 0;
            if (pattern == "+")
                angle = i * angleStep;
            else
                angle = i * angleStep + 45f;

            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

            if (bulletScript != null)
            {
                bulletScript.SetDirection(direction);
                bulletScript.SetBulletSpeed(5f);
                bulletScript.SetRange(10f);
                bulletScript.SetDamage(bulletDmg);
                bulletScript.SetIsEnemyBullet(true);
            }
        }
    }

    private IEnumerator JumpAttack()
    {
        isAttacking = true; // Bloquear nuevos ataques
        bossCollider.enabled = false;
        bossSprite.enabled = false;
        isJumping = true;

        Vector2 startPosition = transform.position;
        Vector2 targetPosition = player.position;
        float jumpTime = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < jumpTime)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / jumpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bossCollider.enabled = true;
        bossSprite.enabled = true;
        isJumping = false;

        yield return new WaitForSeconds(jumpCooldown);
        isAttacking = false; // Permitir nuevos ataques
    }

    private IEnumerator SpinAttack()
    {
        isAttacking = true; // Bloquear nuevos ataques
        for (int i = 0; i < 360; i += 10)
        {
            float angle = i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

            if (bulletScript != null)
            {
                bulletScript.SetDirection(direction);
                bulletScript.SetBulletSpeed(5f);
                bulletScript.SetRange(10f);
                bulletScript.SetDamage(bulletDmg);
                bulletScript.SetIsEnemyBullet(true);
            }

            yield return new WaitForSeconds(0.1f);
        }
        isAttacking = false; // Permitir nuevos ataques
    }

    private IEnumerator ChargeAttack() // Nuevo ataque
    {
        isAttacking = true; // Bloquear nuevos ataques
        Vector2 startPosition = transform.position;
        Vector2 targetPosition = player.position;
        float chargeTime = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < chargeTime)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / chargeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f); // Esperar un momento después de cargar
        isAttacking = false; // Permitir nuevos ataques
    }

    public virtual void reciveDmg(float damage)
    {
        lifePoints -= damage;
        if (lifePoints <= 0)
        {
            RoomManager.Instance.GetPlayerRoom()?.CheckEnemies();
            Destroy(gameObject);
        }
    }
    
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerHealth = collision.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(Mathf.CeilToInt(contactDmg));
            }
        }
    }
}