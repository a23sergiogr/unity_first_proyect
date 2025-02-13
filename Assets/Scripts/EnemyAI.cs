using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private float moveSpeed = 3f;
    [SerializeField] Transform player;

    void Start()
    {
        
    }

    void Update()
    {
        if (player != null) 
        { 
            
            Vector2 direction = (player.position - transform.position).normalized;

            transform.Translate(direction * moveSpeed * Time.deltaTime);

        }
    }
}
