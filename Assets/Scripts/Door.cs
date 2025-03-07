using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2Int direction;
    public Room connectedRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            if (connectedRoom == null)
            {
                return;
            }

            GameObject content = connectedRoom.GenerateContent();

            if (content != null)
            {
                connectedRoom.GenerateEnvironment(content);
            }

            RoomManager.Instance.MoveToRoom(connectedRoom, direction);

            if (content != null)
            {
                connectedRoom.GenerateEnemies(content);
                connectedRoom.HideDoors();
            }
        }
    }
}