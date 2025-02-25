using UnityEngine;

public class Door : MonoBehaviour
{
    public Vector2Int direction;
    public Room connectedRoom;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entro en puerta");
            RoomManager.Instance.MoveToRoom(connectedRoom, direction);
            connectedRoom.GenerateContent();
        }
    }
}
