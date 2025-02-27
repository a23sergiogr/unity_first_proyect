using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private NavMeshSurface navMeshSurface;
    [SerializeField] GameObject roomPrefab;
    [SerializeField] private int maxRooms = 20;
    [SerializeField] private int minRooms = 10;

    int roomWidth = 20;
    int roomHeight = 12;

    [SerializeField] int gridSizeX = 10;
    [SerializeField] int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    private int[,] roomGrid;

    private int roomCount;

    private bool generationComplete = false;

    [SerializeField] private List<GameObject> roomContentPrefab = new List<GameObject>();

    [SerializeField] private List<GameObject> bossRoomContentPrefab = new List<GameObject>();

    [SerializeField] private List<GameObject> treasureRoomContentPrefab = new List<GameObject>();

    private Room currentRoom;

    void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
        }
        else if (roomCount < minRooms)
        {
            Debug.Log("RoomCount was less then the minimun amount of rooms. Trying again.");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms created");
            generationComplete = true;
            AssingContentToRoom();
            GenerateTreasureRoom();
        }
    }

    private void AssingContentToRoom()
    {
        roomObjects.ForEach(room => 
        {
            Room script = room.GetComponent<Room>();
            script.Content = roomContentPrefab[Random.Range(0, roomContentPrefab.Count)];
            script.Position = room.transform;
        });
    }

    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;
        var initialRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount}";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (roomCount >= maxRooms)
            return false;

        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
            return false;

        if (CountAndjacentRooms(roomIndex) > 1)
            return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y);

        return true;
    }

    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        Room leftRoomScript = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScript = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScript = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScript = GetRoomScriptAt(new Vector2Int(x, y - 1));

        if (x > 0 && roomGrid[x - 1, y] != 0 && leftRoomScript != null)
        {
            newRoomScript.OpenDoor(Vector2Int.left, leftRoomScript);
            leftRoomScript.OpenDoor(Vector2Int.right, newRoomScript);
        }
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0 && rightRoomScript != null)
        {
            newRoomScript.OpenDoor(Vector2Int.right, rightRoomScript);
            rightRoomScript.OpenDoor(Vector2Int.left, newRoomScript);
        }
        if (y > 0 && roomGrid[x, y - 1] != 0 && bottomRoomScript != null)
        {
            newRoomScript.OpenDoor(Vector2Int.down, bottomRoomScript);
            bottomRoomScript.OpenDoor(Vector2Int.up, newRoomScript);
        }
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0 && topRoomScript != null)
        {
            newRoomScript.OpenDoor(Vector2Int.up, topRoomScript);
            topRoomScript.OpenDoor(Vector2Int.down, newRoomScript);
        }
    }

    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
            return roomObject.GetComponent<Room>();
        return null;
    }

    private int CountAndjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        if (x > 0 && roomGrid[x - 1, y] != 0) count++;
        if (x < gridSizeX && roomGrid[x + 1, y] != 0) count++;
        if (y > 0 && roomGrid[x, y - 1] != 0) count++;
        if (y < gridSizeY && roomGrid[x, y + 1] != 0) count++;

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;

        return new Vector3(roomWidth * (gridX - gridSizeX / 2), roomHeight * (gridY - gridSizeY / 2), 0);
    }

    private void OnDrawGizmos()
    {
        Color gizmoColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmoColor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }

    public Room GetPlayerRoom()
    {
        return currentRoom;
    }

    public void SetPlayerRoom(Room newRoom)
    {
        currentRoom = newRoom;
    }

    public void MoveToRoom(Room newRoom, Vector2Int direction)
    {
        if (newRoom == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("Transportando");
            player.transform.position = newRoom.transform.position;

            ActivateOnlyCurrentRoom(newRoom);
            navMeshSurface.BuildNavMesh();
        }
    }
    private void ActivateOnlyCurrentRoom(Room currentRoom)
    {
        foreach (var room in roomObjects)
        {
            room.SetActive(room == currentRoom.gameObject);
        }
    }

    private void GenerateBossRoom()
    {
        Room farthestRoom = null;
        float maxDistance = 0f;

        foreach (var room in roomObjects)
        {
            float distance = Vector3.Distance(room.transform.position, Vector3.zero);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestRoom = room.GetComponent<Room>();
            }
        }

        if (farthestRoom != null)
        {
            farthestRoom.typeRoom = TypeRoom.BOSS;
            //farthestRoom.SetAsBossRoom();
            Debug.Log("Boss room set at: " + farthestRoom.RoomIndex);
        }
    }

    private void GenerateTreasureRoom()
    {
        foreach (var room in roomObjects)
        {
            Room roomScript = room.GetComponent<Room>();
            if (CountAndjacentRooms(roomScript.RoomIndex) == 1 && roomScript.typeRoom == TypeRoom.NORMAL)
            {
                SetAsTreasureRoom(roomScript);
                Debug.Log("Treasure room set at: " + roomScript.RoomIndex + " '" + roomScript.name + "'");
                break;
            }
        }
    }

    private void SetAsTreasureRoom(Room room)
    {
        room.Content = treasureRoomContentPrefab[Random.Range(0, treasureRoomContentPrefab.Count)];
    }
}
