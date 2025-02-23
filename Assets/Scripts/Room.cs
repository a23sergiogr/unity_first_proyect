using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    public Vector2Int RoomIndex { get; set; }
    private Dictionary<Vector2Int, GameObject> doors;

    private void Awake()
    {
        doors = new Dictionary<Vector2Int, GameObject>
        {
            { Vector2Int.up, topDoor },
            { Vector2Int.down, bottomDoor },
            { Vector2Int.left, leftDoor },
            { Vector2Int.right, rightDoor }
        };
    }

    public void OpenDoor(Vector2Int direction, Room connectedRoom)
    {
        if (doors.ContainsKey(direction) && doors[direction] != null)
        {
            doors[direction].SetActive(true);
            Door doorScript = doors[direction].GetComponent<Door>();
            if (doorScript != null)
            {
                doorScript.connectedRoom = connectedRoom;
            }
        }
    }
}
