using System.Collections;
using TMPro;
using UnityEngine;

public class BasicEnemyScript : AbstractEnemy
{
    public override void Attack()
    {
        attackHitbox.SetActive(true);
    }
}