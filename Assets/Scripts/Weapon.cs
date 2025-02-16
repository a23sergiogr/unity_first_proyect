using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected Transform firePoint;
    protected float nextFireTime = 0f;

    public abstract Transform GetFirePoint();
    public abstract void Fire(Vector2 direction);
    public abstract void Fire2(Vector2 direction);
}
