using System.Collections;
using UnityEngine;

public class BossScript : MonoBehaviour, Enemy
{
    [SerializeField] public Transform player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject Hitbox;
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite bulletSprite;
    private const float attackCooldown = 2f;
    private const float jumpForce = 10f;
    private const float jumpCooldown = 3f;
    private const float contactDmg = 1f;
    private const float bulletDmg = 1f;
    private float lifePoints = 40f;
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
        if (!isAttacking && Time.time >= nextAttackTime) 
        {
            int attackType = Random.Range(0, 4); 
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
                    StartCoroutine(ChargeAttack()); 
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
                bulletScript.SetSprite(bulletSprite);
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
        const float JumpTime = 2f;
        const float JumpTimeAnimation = 0.6f;
        const float JumpTimeAnimation2 = 0.2f;

        StartJumpAttackAnimation();

        yield return new WaitForSeconds(JumpTimeAnimation);

        animator.SetBool("AirTime", true);

        DisableBossColliderAndSprite();

        Vector2 startPosition = transform.position;
        Vector2 targetPosition = player.position;


        float elapsedTime = 0f;
        while (elapsedTime < JumpTime)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / JumpTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        EndJumpAttackAnimation();
        yield return new WaitForSeconds(JumpTimeAnimation2);

        animator.SetTrigger("ReturnToNormal");

        yield return new WaitForSeconds(jumpCooldown);

        animator.ResetTrigger("ReturnToNormal");

        isAttacking = false;
    }

    private void StartJumpAttackAnimation()
    {
        animator.SetTrigger("JumpAttack");
        isAttacking = true; 
        isJumping = true;
    }

    private void DisableBossColliderAndSprite()
    {
        bossCollider.enabled = false;
    }

    private void EndJumpAttackAnimation()
    {
        animator.SetBool("AirTime", false);
        animator.ResetTrigger("JumpAttack");
        bossCollider.enabled = true;
        isJumping = false;
    }


    private IEnumerator SpinAttack()
    {
        isAttacking = true; 
        for (int i = 0; i < 360; i += 20)
        {
            float angle = i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

            if (bulletScript != null)
            {
                bulletScript.SetSprite(bulletSprite);
                bulletScript.SetDirection(direction);
                bulletScript.SetBulletSpeed(5f);
                bulletScript.SetRange(10f);
                bulletScript.SetDamage(bulletDmg);
                bulletScript.SetIsEnemyBullet(true);
            }

            yield return new WaitForSeconds(0.1f);
        }
        isAttacking = false;
    }

    private IEnumerator ChargeAttack() 
    {
        isAttacking = true; 
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

        yield return new WaitForSeconds(1f); 
        isAttacking = false; 
    }

    public virtual void reciveDmg(float damage)
    {
        lifePoints -= damage;
        if (lifePoints <= 0)
        {
            RoomManager.Instance.GetPlayerRoom()?.CheckEnemies();
            Death();
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

    private IEnumerator Death() 
    {
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}