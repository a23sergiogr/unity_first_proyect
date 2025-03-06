using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2Int direction;
    public Room connectedRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entr� en la puerta.");

            // Verifica si la habitaci�n conectada es v�lida
            if (connectedRoom == null)
            {
                Debug.LogError("La habitaci�n conectada no est� asignada.");
                return;
            }

            // Genera el contenido de la habitaci�n
            GameObject content = connectedRoom.GenerateContent();

            if (content == null)
            {
                Debug.LogWarning("La habitaci�n est� vac�a o ya est� completa. No se generar� contenido adicional.");
            }
            else
            {
                Debug.Log("Generando contenido de la habitaci�n.");
                connectedRoom.GenerateEnvironment(content);
            }

            // Mueve al jugador a la nueva habitaci�n
            RoomManager.Instance.MoveToRoom(connectedRoom, direction);

            if (content != null)
            {
                Debug.Log("Generando enemigos en la habitaci�n.");
                connectedRoom.GenerateEnemies(content);

                Debug.Log("Ocultando puertas de la habitaci�n.");
                connectedRoom.HideDoors();
            }
        }
    }
}