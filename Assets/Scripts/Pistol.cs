using System.Collections;
using UnityEngine;

public class Pistol : Weapon
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletRange = 2f;
    [SerializeField] private float bulletDmgMultiplier = 1.5f;

    [SerializeField] private GameObject closeAttackHitbox;
    [SerializeField] private float closeAttackDmgMultiplier = 1f;


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

            GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            BulletScript bulletScript = newBullet.GetComponent<BulletScript>();

            if (bulletScript != null)
            {
                bulletScript.SetDirection(direction);
                bulletScript.SetBulletSpeed(bulletSpeed);
                bulletScript.SetRange(bulletRange);
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
