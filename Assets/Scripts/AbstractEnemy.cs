using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{

    public abstract void reciveDmg(float damage);
    public abstract void attack();
}
