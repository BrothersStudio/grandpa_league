#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

class QualificationToolTip : MonoBehaviour
{
    private string ToolTipText = "";
    public GameObject ToolTipPanel;

    void OnMouseEnter()
    {
        ToolTipPanel.SetActive(true);
        ToolTipPanel.transform.Find("ToolTipText").GetComponent<Text>().text = ToolTipText;
    }

    void OnMouseExit()
    {
        ToolTipPanel.SetActive(false);
    }

    public void SetToolTipText(string newText)
    {
        ToolTipPanel.transform.Find("ToolTipText").GetComponent<Text>().text = newText;
        this.ToolTipText = newText;
    }
}
