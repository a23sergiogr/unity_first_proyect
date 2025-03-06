using UnityEngine;

public class CameraController : MonoBehaviour
{
    private RoomManager roomManager;
    private Transform targetRoom;
    private bool isMoving = false; 

    private void Start()
    {
        roomManager = RoomManager.Instance;
    }

    private void LateUpdate()
    {
        if (roomManager == null || isMoving) return; 

        Room playerRoom = roomManager.GetPlayerRoom();
        if (playerRoom != null)
        {
            targetRoom = playerRoom.transform;
            transform.position = new Vector3(targetRoom.position.x, targetRoom.position.y, transform.position.z);
        }
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }
}
