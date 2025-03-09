using System.Collections;
using UnityEngine;

public class Shotgun : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected float damage = 0.2f;
    [SerializeField] private int bulletCount = 5;
    [SerializeField] private float spreadAngle = 30f;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletRange = 2f;

    [SerializeField] private GameObject closeAttackHitbox;

    [SerializeField] private Sprite bulletSprite;

    private void Start()
    {
        closeAttackHitbox.SetActive(false);
    }

    public override Transform GetFirePoint() 
    { 
        return firePoint;
    }

    public override void Fire(Vector2 direction)
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;

            for (int i = 0; i < bulletCount; i++)
            {
                float angleOffset = Random.Range(-spreadAngle / 2, spreadAngle / 2);
                Vector2 spreadDirection = Quaternion.Euler(0, 0, angleOffset) * direction;

                GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
                BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

                if (bulletScript != null)
                {
                    bulletScript.SetSprite(bulletSprite);
                    bulletScript.SetDamage(damage);
                    bulletScript.SetDirection(spreadDirection);
                    bulletScript.SetBulletSpeed(bulletSpeed);
                    bulletScript.SetRange(bulletRange);
                }
            }
        }
    }

    public override void Fire2(Vector2 direction)
    {
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;

            StartCoroutine(ActivateHitbox());
        }
    }

    private IEnumerator ActivateHitbox()
    {
        closeAttackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        closeAttackHitbox.SetActive(false);
    }

}
