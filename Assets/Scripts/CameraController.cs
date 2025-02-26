using UnityEngine;

public class CameraController : MonoBehaviour
{
    private RoomManager roomManager;
    private Transform targetRoom;

    private void Start()
    {
        roomManager = RoomManager.Instance;
    }

    private void LateUpdate()
    {
        if (roomManager == null) return;

        Room playerRoom = roomManager.GetPlayerRoom();
        if (playerRoom != null)
        {
            targetRoom = playerRoom.transform;
            transform.position = new Vector3(targetRoom.position.x, targetRoom.position.y, transform.position.z);
        }
    }
}
