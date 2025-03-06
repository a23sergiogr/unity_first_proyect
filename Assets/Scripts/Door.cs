using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2Int direction;
    public Room connectedRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entró en la puerta.");

            // Verifica si la habitación conectada es válida
            if (connectedRoom == null)
            {
                Debug.LogError("La habitación conectada no está asignada.");
                return;
            }

            // Genera el contenido de la habitación
            GameObject content = connectedRoom.GenerateContent();

            if (content == null)
            {
                Debug.LogWarning("La habitación está vacía o ya está completa. No se generará contenido adicional.");
            }
            else
            {
                Debug.Log("Generando contenido de la habitación.");
                connectedRoom.GenerateEnvironment(content);
            }

            // Mueve al jugador a la nueva habitación
            RoomManager.Instance.MoveToRoom(connectedRoom, direction);

            if (content != null)
            {
                Debug.Log("Generando enemigos en la habitación.");
                connectedRoom.GenerateEnemies(content);

                Debug.Log("Ocultando puertas de la habitación.");
                connectedRoom.HideDoors();
            }
        }
    }
}