using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    [SerializeField] private GameObject topDoor;
    [SerializeField] private GameObject bottomDoor;
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

    [SerializeField] private GameObject topClosedDoor;
    [SerializeField] private GameObject bottomClosedDoor;
    [SerializeField] private GameObject leftClosedDoor;
    [SerializeField] private GameObject rightClosedDoor;

    private List<GameObject> createdDoors = new List<GameObject>();

    public Vector2Int RoomIndex { get; set; }
    private Dictionary<Vector2Int, GameObject> doors;
    private Dictionary<Vector2Int, GameObject> closedDoors;

    public GameObject Content { get; set; }
    public Transform Position { get; set; }

    public bool isComplete = false;

    private TypeRoom _typeRoom = TypeRoom.NORMAL;
    public TypeRoom typeRoom
    {
        get { return _typeRoom; }
        set { _typeRoom = value; }
    }

    private void Awake()
    {
        typeRoom = TypeRoom.NORMAL;

        doors = new Dictionary<Vector2Int, GameObject>
        {
            { Vector2Int.up, topDoor },
            { Vector2Int.down, bottomDoor },
            { Vector2Int.left, leftDoor },
            { Vector2Int.right, rightDoor }
        };

        closedDoors = new Dictionary<Vector2Int, GameObject>
        {
            { Vector2Int.up, topClosedDoor },
            { Vector2Int.down, bottomClosedDoor },
            { Vector2Int.left, leftClosedDoor },
            { Vector2Int.right, rightClosedDoor }
        };

        HideDoors();
    }

    public GameObject GenerateContent()
    {
        if (!isComplete)
        {
            if (Content == null)
                return null;

            Debug.Log("Generando contenido para la habitaci�n.");
            GameObject contentInstance = Instantiate(Content, Position);
            contentInstance.transform.localPosition = Vector3.zero;

            return contentInstance;
        }
        else
        {
            Debug.Log("La habitaci�n ya est� completa. No se generar� contenido adicional.");
            return null;
        }
    }

    public void GenerateEnemies(GameObject contentInstance)
    {
        foreach (Transform child in contentInstance.transform)
        {
            if (child.CompareTag("Enemy"))
            {
                Debug.Log("Creando Enemigos");
                child.gameObject.SetActive(true); // Activa los enemigos

                NavMeshAgent navMeshAgent = child.gameObject.GetComponent<NavMeshAgent>();
                if (navMeshAgent != null)
                {
                    navMeshAgent.enabled = false; // Desactiva el NavMeshAgent
                    StartCoroutine(ActivateNavMeshAgentAfterDelay(navMeshAgent, 1f)); // Espera 1 segundo
                }
            }
        }
    }

    private IEnumerator ActivateNavMeshAgentAfterDelay(NavMeshAgent navMeshAgent, float delay)
    {
        yield return new WaitForSeconds(delay); // Espera el tiempo especificado
        navMeshAgent.enabled = true; // Reactiva el NavMeshAgent
        Debug.Log("NavMeshAgent activado despu�s de " + delay + " segundos.");
    }

    public void GenerateEnvironment(GameObject contentInstance)
    {
        foreach (Transform child in contentInstance.transform)
        {
            if (!child.CompareTag("Enemy"))
            {
                Debug.Log("Creando obstaculos");
                child.gameObject.SetActive(true); // Activa el resto del entorno
            }
        }
    }


    public void OpenDoor(Vector2Int direction, Room connectedRoom)
    {
        if (doors.ContainsKey(direction) && doors[direction] != null)
        {
            doors[direction].SetActive(true);
            if (closedDoors.ContainsKey(direction) && closedDoors[direction] != null)
            {
                closedDoors[direction].SetActive(false);
            }

            Door doorScript = doors[direction].GetComponent<Door>();
            createdDoors.Add(doors[direction]);

            if (doorScript != null)
            {
                doorScript.connectedRoom = connectedRoom;
                doorScript.direction = direction;
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

        foreach (var closedDoor in closedDoors.Values)
        {
            if (closedDoor != null)
            {
                closedDoor.SetActive(true);
            }
        }
    }

    public void ShowDoors()
    {
        foreach (var door in createdDoors)
        {
            if (door != null)
            {
                door.SetActive(true);

                foreach (var kvp in doors)
                {
                    if (kvp.Value == door)
                    {
                        if (closedDoors.ContainsKey(kvp.Key) && closedDoors[kvp.Key] != null)
                        {
                            closedDoors[kvp.Key].SetActive(false);
                        }
                        break;
                    }
                }
            }
        }
    }

    public void CheckEnemies()
    {
        StartCoroutine(waitAndCount());
    }

    private IEnumerator waitAndCount()
    {
        yield return new WaitForSeconds(0.5f);
        if (this == RoomManager.Instance.GetPlayerRoom())
        {
            int enemyCount = CountEnemiesInChildren(transform);
            Debug.Log("Enemigos en la habitaci�n: " + enemyCount);

            if (enemyCount == 0)
            {
                ShowDoors();
                isComplete = true;
            }
        }
    }

    private int CountEnemiesInChildren(Transform parent)
    {
        int count = 0;
        foreach (Transform child in parent)
        {
            if (child.CompareTag("Enemy"))
            {
                count++;
            }
            count += CountEnemiesInChildren(child);
        }
        return count;
    }

    public void changeColor(Color color)
    {
        SpriteRenderer[] walls = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sr in walls)
        {
            if (sr.CompareTag("Wall"))
            {
                sr.color = color;
            }
        }
    }
}
