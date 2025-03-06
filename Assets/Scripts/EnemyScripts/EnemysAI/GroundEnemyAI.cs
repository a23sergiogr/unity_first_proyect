using UnityEngine;
using UnityEngine.AI;

public class GroundEnemyAI : MonoBehaviour
{
    [SerializeField] Transform player;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Busca al jugador en la escena
    }

    void Update()
    {
        agent.SetDestination(player.position);

        // Hace que el enemigo mire hacia el jugador en 2D
        Vector3 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
