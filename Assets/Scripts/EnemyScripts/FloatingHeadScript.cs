using System.Collections;
using UnityEngine;

public class FloatingHeadScript : AbstractEnemy
{

    private float lifePoints = 1.5f;
    private float attackDamage = 10f;
    private float rangeAttackDamage = 20f;
    private float rangeAttackCooldown = 2f;
    private bool readyToAttack = true;
    [SerializeField] Transform player;
    [SerializeField] GameObject attackHitbox;
    [SerializeField] GameObject bulletPrefab;

    void Update()
    {
            if (Vector2.Distance(transform.position, player.position) < 6 && readyToAttack)
            {
                shoot();
                StartCoroutine(rangeAttack());
            } 
            
    }

    private IEnumerator rangeAttack()
    {
        readyToAttack = false;
        yield return new WaitForSeconds(rangeAttackCooldown);
        readyToAttack = true;
    }

    private void shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

        if (bulletScript != null)
        {
            bulletScript.SetDamage(rangeAttackDamage);
            bulletScript.SetDirection(player.position - transform.position);
            bulletScript.SetBulletSpeed(5f);
            bulletScript.SetRange(10f);
            bulletScript.SetIsEnemyBullet(true);
        }
    }

    public override void attack()
    {
        attackHitbox.SetActive(true);
    }

    public override void reciveDmg(float damage)
    {
        lifePoints = lifePoints - damage;
        //ShowDamageText(damage);
        if (lifePoints < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerHealth = collision.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}
