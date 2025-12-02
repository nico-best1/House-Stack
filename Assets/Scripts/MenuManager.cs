using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menús")]
    public GameObject mainMenuPanel;
    public GameObject instructionsPanel;

    [Header("Botones del menú principal")]
    public Button startButton;
    public Button instructionsButton;
    public Button exitButton;

    [Header("Botones del menú de instrucciones")]
    public Button backButton;

    [Header("UI")]
    public TMP_Text instruction;

    void Start()
    {
        mainMenuPanel.SetActive(true);
        instructionsPanel.SetActive(false);

        startButton.onClick.AddListener(OnStartClicked);
        instructionsButton.onClick.AddListener(OnInstructionsClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        backButton.onClick.AddListener(OnBackClicked);
    }

    void OnStartClicked()
    {
        SceneManager.LoadScene("Game");
    }

    void OnInstructionsClicked()
    {
        mainMenuPanel.SetActive(false);
        instructionsPanel.SetActive(true);
    }

    void OnBackClicked()
    {
        instructionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    void OnExitClicked()
    {
        Application.Quit();
    }
}
