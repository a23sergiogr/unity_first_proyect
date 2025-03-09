using System.Collections;
using UnityEngine;

public class FloatingHeadScript : AbstractEnemy
{
    private int rangeAttackDamage = 2;
    private float rangeAttackCooldown = 2f;
    private bool readyToAttack = false;
    private bool waitingForAttack = true;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Sprite bulletSprite;


    protected override void Update()
    {
        base.Update();

        Vector2 enemyGlobalPosition = transform.parent != null ? transform.parent.TransformPoint(transform.localPosition) : transform.position;

        Vector2 playerGlobalPosition = player.position;

        if (waitingForAttack)
        {
            StartCoroutine(rangeAttack());
        }

        float distance = Vector2.Distance(enemyGlobalPosition, playerGlobalPosition);

        if (distance < 5 && readyToAttack)
        {
            Shoot();
            readyToAttack = false;
            waitingForAttack = true;
        }
    }



    private IEnumerator rangeAttack()
    {
        waitingForAttack = false;
        yield return new WaitForSeconds(rangeAttackCooldown);
        readyToAttack = true;
    }

    private void Shoot()
    {
        if (player == null) return; 

        Vector2 enemyGlobalPosition = transform.position;

        Vector2 playerGlobalPosition = player.position;

        Vector2 direction = (playerGlobalPosition - enemyGlobalPosition).normalized;

        GameObject newBullet = Instantiate(bulletPrefab, enemyGlobalPosition, Quaternion.identity);
        BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

        if (bulletScript != null)
        {
            bulletScript.SetSprite(bulletSprite);
            bulletScript.SetDamage(rangeAttackDamage);
            bulletScript.SetDirection(direction);
            bulletScript.SetBulletSpeed(5f);
            bulletScript.SetRange(10f);
            bulletScript.SetIsEnemyBullet(true);
        }
    }


    public override void Attack()
    {
        attackHitbox.SetActive(true);
    }
}
