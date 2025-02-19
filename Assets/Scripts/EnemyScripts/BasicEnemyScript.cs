using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BasicEnemyScript : AbstractEnemy
{

    private float moveSpeed = 5f;
    private float lifePoints = 1.5f;
    private float attackDamage = 10f;
    [SerializeField] Transform player;
    [SerializeField] GameObject attackHitbox;
    [SerializeField] GameObject damageTextPrefab;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public override void attack()
    {
        attackHitbox.SetActive(true);
    }

    public override void reciveDmg(float damage)
    {
        lifePoints = lifePoints - damage;
        ShowDamageText(damage);
        if (lifePoints < 0)
        {
            Destroy(gameObject);
        }
    }

    private void ShowDamageText(float damage)
    {
        GameObject damageTextInstance = Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
        damageTextInstance.GetComponent<DamageText>().SetDamageText(damage);
        damageTextInstance.transform.SetParent(GameObject.Find("Canvas").transform, false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Asegúrate de que el jugador tenga la etiqueta "Player"
        {
            PlayerController playerHealth = collision.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}
