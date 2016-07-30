using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public struct Version
{ 
    public const string ReleaseVersion = "Alpha";
    public const int MajorVersion = 0;
    public const int MinorVersion = 1;
    public const int BuildNumber = 317;
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

        if (File.Exists(Application.persistentDataPath + "/manager.gpa"))
        {
            loadGame.interactable = true;
        }

    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void OnLoadGameClicked()
    {
        PlayerPrefs.SetString("load", "load");
        SceneManager.LoadScene(2);
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
