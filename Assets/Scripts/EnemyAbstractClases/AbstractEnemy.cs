using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.AI;

public abstract class AbstractEnemy : MonoBehaviour, Enemy
{
    public Transform player;
    public new GameObject animation;
    protected float lifePoints = 1.5f;
    protected int attackDamage = 1;

    [SerializeField] protected GameObject attackHitbox;
    [SerializeField] protected GameObject damageTextPrefab;

    private readonly float moveSpeed = 2f;

    protected NavMeshAgent agent;

    protected virtual void Start()
    {
        Debug.Log("Enemigo creado");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    protected virtual void Update()
    {
        if (player == null || agent == null || !agent.enabled) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            Debug.LogWarning($"[{name}] Agent is NOT on NavMesh!");
        }
    }

    public abstract void Attack();

    public virtual void reciveDmg(float damage)
    {
        lifePoints -= damage;
        if (lifePoints <= 0)
        {
            RoomManager.Instance.GetPlayerRoom()?.CheckEnemies();
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerHealth = collision.GetComponent<PlayerController>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }
}
