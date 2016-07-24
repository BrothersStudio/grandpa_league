using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public struct Version
{ 
    public const string ReleaseVersion = "Alpha";
    public const int MajorVersion = 0;
    public const int MinorVersion = 1;
    public const int BuildNumber = 1;
}


public class MainMenu : MonoBehaviour
{

    public Button newGame;
    public Button loadGame;
    public Button exit;

    public Text versionInfo;
    public GameObject panel;
    public InputField inputField;
    public Button confirmButton;
    public Button cancelButton;

    void Start()
    {
        versionInfo.text = string.Format("Version: {0} v{1}.{2}.{3}", Version.ReleaseVersion, Version.MajorVersion, Version.MinorVersion, Version.BuildNumber);
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
