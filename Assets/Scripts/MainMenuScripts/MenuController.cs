using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public Button newRunButton;
    public Button exitButton;

    private Button selectedButton; 
    private Button[] buttons; 

    void Start()
    {
        // Asignar los eventos a los botones
        newRunButton.onClick.AddListener(OnNewRunClicked);
        exitButton.onClick.AddListener(OnExitClicked);

        // Guardar los botones en un array
        buttons = new Button[] { newRunButton, exitButton };

        selectedButton = newRunButton;
        UpdateButtonSelection();
    }

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            SelectPreviousButton();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) 
        {
            SelectNextButton();
        }

        if (Input.GetKeyDown(KeyCode.E)) 
        {
            ExecuteSelectedButton();
        }
    }

    private void SelectPreviousButton()
    {
        int currentIndex = System.Array.IndexOf(buttons, selectedButton);
        int previousIndex = (currentIndex - 1 + buttons.Length) % buttons.Length; 
        selectedButton = buttons[previousIndex];
        UpdateButtonSelection();
    }

    private void SelectNextButton()
    {
        int currentIndex = System.Array.IndexOf(buttons, selectedButton);
        int nextIndex = (currentIndex + 1) % buttons.Length;
        selectedButton = buttons[nextIndex];
        UpdateButtonSelection();
    }

    private void UpdateButtonSelection()
    {
        foreach (Button button in buttons)
        {
            button.GetComponent<Image>().color = Color.white; 
        }

        selectedButton.GetComponent<Image>().color = Color.yellow; 
    }

    private void ExecuteSelectedButton()
    {
        if (selectedButton == newRunButton)
        {
            OnNewRunClicked();
        }
        else if (selectedButton == exitButton)
        {
            OnExitClicked();
        }
    }

    private void OnNewRunClicked()
    {
        Debug.Log("New Run");
        SceneManager.LoadScene("GameScene");
    }

    private void OnExitClicked()
    {
        Debug.Log("Exit");
        Application.Quit();
    }
}
