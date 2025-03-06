using System.Collections;
using UnityEngine;

public class BossScript : MonoBehaviour, Enemy
{
    [SerializeField] public Transform player;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpCooldown = 3f;

    private float lifePoints = 20f;
    private float contactDmg = 1f;
    private float nextAttackTime = 2f;
    private bool isJumping = false;

    private Collider2D bossCollider;
    private SpriteRenderer bossSprite;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossCollider = GetComponent<Collider2D>();
        bossSprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
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
                    StartCoroutine(WaveAttack()); // Nuevo ataque
                    break;
            }
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private IEnumerator ShootWave()
    {
        // Disparar 3 oleadas de balas en forma de X, +, X
        for (int i = 0; i < 3; i++)
        {
            ShootPattern(i % 2 == 0 ? "X" : "+");
            yield return new WaitForSeconds(0.5f);
        }
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
                bulletScript.SetDirection(direction); // Establece la dirección de la bala
                bulletScript.SetBulletSpeed(5f);
                bulletScript.SetRange(10f);
                bulletScript.SetIsEnemyBullet(true);
            }
        }
    }

    private IEnumerator JumpAttack()
    {
        // Desactivar colisión y sprite durante el salto
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

        // Reactivar colisión y sprite al terminar el salto
        bossCollider.enabled = true;
        bossSprite.enabled = true;
        isJumping = false;

        yield return new WaitForSeconds(jumpCooldown);
    }

    private IEnumerator SpinAttack()
    {
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
                bulletScript.SetIsEnemyBullet(true);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator WaveAttack()
    {
        // Nuevo ataque: Onda expansiva que daña al jugador si está cerca
        Debug.Log("Ataque de onda expansiva");

        float waveRadius = 5f; // Radio de la onda
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, waveRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                PlayerController playerComponent = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                playerComponent.TakeDamage(Mathf.CeilToInt(contactDmg));
            }
        }

        yield return new WaitForSeconds(1f); // Tiempo de espera después del ataque
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
}