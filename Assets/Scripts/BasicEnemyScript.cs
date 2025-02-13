using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyScript : AbstractEnemy
{

    private float moveSpeed = 5f;
    private float lifePoints = 1.5f;
    private float attackDamage = 1f;
    [SerializeField] Transform player;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public override void reciveDmg(float damage)
    {
        lifePoints = lifePoints - damage;
        Debug.Log(lifePoints);
        if (lifePoints < 0)
        {
            Destroy(gameObject);
        }
    }
}
