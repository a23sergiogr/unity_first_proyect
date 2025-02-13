using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected float fireRate = 1f;
    [SerializeField] protected float damage = 1f;
    protected float nextFireTime = 0f;


    public abstract void Fire(Vector2 direction);
    public abstract void Fire2(Vector2 direction);
}
