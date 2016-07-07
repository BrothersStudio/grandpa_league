using UnityEngine;
using System.Collections;
 
public class TrainingPanelAppear : MonoBehaviour 
{
    private Panel PanelObject; // Assign in inspector

    void Start() 
    {
        PanelObject = GameObject.Find("TrainingPanel").GetComponent<Panel>();
        PanelObject.GetComponent<Panel>().enabled = false;
    }
 
    public void TogglePanel()
    {
        if (PanelObject.enabled == true) {
            PanelObject.GetComponent<Panel>().enabled = false;
        } else 
            PanelObject.GetComponent<Panel>().enabled = true;
        } 
    }
}