using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [SerializeField] GameObject topDoor;
    [SerializeField] GameObject bottomDoor;
    [SerializeField] GameObject leftDoor;
    [SerializeField] GameObject rightDoor;

    List<GameObject> createdDoors = new List<GameObject>();

    public Vector2Int RoomIndex { get; set; }
    private Dictionary<Vector2Int, GameObject> doors;

    public GameObject Content { get; set; }
    public Transform Position { get; set; }

    public bool isComplete = false;

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

    public void GenerateContent()
    {
        if (!isComplete) 
        {
            GameObject contentInstance = Instantiate(Content, Position);
            contentInstance.transform.localPosition = Vector3.zero;
            HideDoors();
        }
    }

    public void OpenDoor(Vector2Int direction, Room connectedRoom)
    {
        if (doors.ContainsKey(direction) && doors[direction] != null)
        {
            doors[direction].SetActive(true);
            Door doorScript = doors[direction].GetComponent<Door>();

            createdDoors.Add(doors[direction]);

            if (doorScript != null)
            {
                doorScript.connectedRoom = connectedRoom;
            }
        }
    }

    public void HideDoors()
    {
        foreach (var door in doors.Values)
        {
            if (door != null)
            {
                door.SetActive(false); 
            }
        }
    }

    public void ShowDoors()
    {
        foreach (var door in doors.Values)
        {
            if (createdDoors.Contains(door))
            {
                door.SetActive(true); 
            }
        }
    }
    public void CheckEnemies()
    {
        GameObject variableEntities = GameObject.FindGameObjectWithTag("VariableEntities");
        if (variableEntities != null)
        {
            int enemyCount = 0;

            foreach (Transform child in variableEntities.transform)
            {
                if (child.CompareTag("Enemy"))
                {
                    enemyCount++;
                }
            }

            Debug.Log("Enemigos en la habitación: " + enemyCount);

            if (enemyCount == 1)
            {
                ShowDoors();
                isComplete = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RoomManager.Instance.SetPlayerRoom(this);
            Debug.Log("Jugador entró en " + gameObject.name);
        }
    }
}