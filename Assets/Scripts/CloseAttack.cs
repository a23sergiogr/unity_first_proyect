using UnityEngine;

public class CloseAttack : MonoBehaviour
{
    private float damage = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.reciveDmg(damage);
            }
        }
    }
}
