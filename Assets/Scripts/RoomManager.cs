using System.Collections;
using System.Collections.Generic;
using NavMeshPlus.Components;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    [SerializeField] private List<NavMeshSurface> navMeshSurfaceList = new List<NavMeshSurface>();
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

    [SerializeField] private TileBase floorTile; // Tile para el suelo

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
            Debug.Log("RoomCount was less then the minimum amount of rooms. Trying again.");
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            Debug.Log($"Generation complete, {roomCount} rooms created");
            generationComplete = true;
            GenerateBossRoom();
            GenerateTreasureRoom();
            AssignContentToRoom();
            ActivateOnlyCurrentRoom(roomObjects[0].GetComponent<Room>());
        }
    }

    private void AssignContentToRoom()
    {
        foreach (var room in roomObjects)
        {
            Room script = room.GetComponent<Room>();
            if (script.typeRoom == TypeRoom.NORMAL && script.Content == null) // Verifica si el contenido ya fue asignado
            {
                script.Content = roomContentPrefab[Random.Range(0, roomContentPrefab.Count)];
                script.Position = room.transform;
            }
        }
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
        Room roomScript = initialRoom.GetComponent<Room>();
        roomScript.RoomIndex = roomIndex;
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

        if (CountAdjacentRooms(roomIndex) > 1)
            return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(roomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        Room roomScript = newRoom.GetComponent<Room>();
        roomScript.RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);


        OpenDoors(newRoom, x, y);

        // Verifica si el contenido ya fue asignado
        if (roomScript.Content == null)
        {
            roomScript.Content = roomContentPrefab[Random.Range(0, roomContentPrefab.Count)];
            roomScript.Position = newRoom.transform;
        }

        return true;
    }

    private void RegenerateRooms()
    {
        foreach (var room in roomObjects)
        {
            Destroy(room);
        }
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

    private int CountAdjacentRooms(Vector2Int roomIndex)
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
        Debug.Log("Jugador entró en " + gameObject.name);
        currentRoom = newRoom;
        MoveCameraToRoom(newRoom);
    }

    private void MoveCameraToRoom(Room room)
    {
        Debug.Log("Moviendo cámara con SmoothDamp");
        StartCoroutine(MoveCameraSmooth(room.transform.position, 1f));
    }

    private IEnumerator MoveCameraSmooth(Vector3 targetPosition, float duration)
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) yield break;

        CameraController cameraController = mainCamera.GetComponent<CameraController>();
        if (cameraController != null) cameraController.SetMoving(true);

        Vector3 velocity = Vector3.zero;
        Vector3 startPosition = mainCamera.transform.position;
        targetPosition.z = startPosition.z;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPosition, ref velocity, 0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;

        if (cameraController != null) cameraController.SetMoving(false);
    }

    public void MoveToRoom(Room newRoom, Vector2Int direction)
    {
        if (newRoom == null)
        {
            Debug.LogError("La habitación a la que se intenta mover es nula.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("No se encontró al jugador en la escena.");
            return;
        }

        // Calcula la nueva posición del jugador
        Vector3 newPosition = newRoom.transform.position;

        float offsetX = 7.7f;
        float offsetY = 3.7f;

        if (direction == Vector2Int.left) newPosition += new Vector3(offsetX, 0, 0);
        if (direction == Vector2Int.right) newPosition += new Vector3(-offsetX, 0, 0);
        if (direction == Vector2Int.up) newPosition += new Vector3(0, -offsetY, 0);
        if (direction == Vector2Int.down) newPosition += new Vector3(0, offsetY, 0);

        // Mueve al jugador a la nueva posición
        SetPlayerRoom(newRoom);
        player.transform.position = newPosition;
        Debug.Log($"Jugador movido a la habitación en posición: {newPosition}.");

        // Activa solo la habitación actual
        ActivateOnlyCurrentRoom(newRoom);
        Debug.Log("Habitación activada: " + newRoom.name);
        newRoom.CheckEnemies();

        // Reconstruye la malla de navegación
        Debug.Log("Construyendo NavMesh para la habitación actual.");
        foreach (var navMeshSurface in navMeshSurfaceList)
        {
            navMeshSurface.BuildNavMesh();
        }
    }

    private void ActivateOnlyCurrentRoom(Room currentRoom)
    {
        foreach (var room in roomObjects)
        {
            bool shouldActivate = room == currentRoom.gameObject || room.GetComponent<Room>().isComplete;
            room.SetActive(shouldActivate);

            if (shouldActivate && room.GetComponent<Room>().Content == null)
            {
                // Asigna el contenido solo si no está asignado
                Room script = room.GetComponent<Room>();
                if (script.typeRoom == TypeRoom.NORMAL)
                {
                    script.Content = roomContentPrefab[Random.Range(0, roomContentPrefab.Count)];
                    script.Position = room.transform;
                }
            }
        }
    }

    private void GenerateBossRoom()
    {
        Room farthestRoom = null;
        GameObject roomObject = null;
        float maxDistance = 0f;

        foreach (var room in roomObjects)
        {
            float distance = Vector3.Distance(room.transform.position, Vector3.zero);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestRoom = room.GetComponent<Room>();
                roomObject = room;
            }
        }

        if (farthestRoom != null)
        {
            farthestRoom.changeColor(new Color(254f / 255f, 25f / 255f, 55f));
            farthestRoom.isComplete = false;
            farthestRoom.typeRoom = TypeRoom.BOSS;
            farthestRoom.Content = bossRoomContentPrefab[Random.Range(0, bossRoomContentPrefab.Count)];
            farthestRoom.Position = roomObject.transform;
            Debug.Log("Boss room set at: " + farthestRoom.RoomIndex + " '" + farthestRoom.name + "'");
        }
    }

    private void GenerateTreasureRoom()
    {
        foreach (var room in roomObjects)
        {
            Room roomScript = room.GetComponent<Room>();
            if (roomScript.name == "Room-1")
            {
                roomScript.typeRoom = TypeRoom.START;
                roomScript.isComplete = true;
                roomScript.changeColor(new Color(0f, 254f / 255f, 0f));
            }
            if (CountAdjacentRooms(roomScript.RoomIndex) == 1 && roomScript.typeRoom == TypeRoom.NORMAL)
            {
                roomScript.changeColor(new Color(254f / 255f, 190f / 255f, 0f));
                roomScript.typeRoom = TypeRoom.TREASURE;
                roomScript.Content = treasureRoomContentPrefab[Random.Range(0, treasureRoomContentPrefab.Count)];
                roomScript.Position = room.transform;
                Debug.Log("Treasure room set at: " + roomScript.RoomIndex + " '" + roomScript.name + "'");
                break;
            }
        }
    }
}
