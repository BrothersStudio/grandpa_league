using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
    {
    public Button newGame;
    public Button loadGame;
    public Button exit;

    public GameObject panel;
    public InputField inputField;
    public Button confirmButton;
    public Button cancelButton;

    void Start()
    {
        panel.SetActive(false);
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void OnLoadGameClicked()
    {
    }

    public void OnNewGameClicked()
    {
        panel.SetActive(true);
    }

    public void OnCancelButtonClicked()
    {
        panel.SetActive(false);
    }

    public void OnConfirmButtonClicked()
    {
        PlayerPrefs.SetString("name", inputField.text);
        SceneManager.LoadScene(2);
    }
}
