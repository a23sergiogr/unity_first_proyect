using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject canvas;
    public Button restartButton;
    public PauseManager pauseManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false); 
        }
    }

    public void PlayerDeath()
    {
        Debug.Log("Jugador ha muerto. Activando botón de reinicio.");

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true); 
        }

        pauseManager.TogglePause();
    }

    public void RestartGame()
    {
        Debug.Log("Reiniciando juego...");

        pauseManager.TogglePause();

        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}