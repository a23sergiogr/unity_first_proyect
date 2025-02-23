using UnityEngine;

public abstract class AbstractEnemy : MonoBehaviour
{

    public Transform player;
    public GameObject animation;
    public abstract void reciveDmg(float damage);
    public abstract void attack();
}
