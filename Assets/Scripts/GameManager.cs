using UnityEngine;
using UnityEngine.SceneManagement;
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
            restartButton.onClick.AddListener(RestartGame);
            restartButton.gameObject.SetActive(false); 
        }
    }

    public void PlayerDeath()
    {
        Debug.Log("Jugador ha muerto. Activando bot�n de reinicio.");

        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true); 
        }
    }

    public void RestartGame()
    {
        Debug.Log("Reiniciando juego...");

        SceneManager.LoadScene("MainMenu");
    }
}