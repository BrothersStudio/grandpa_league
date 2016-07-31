using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LoadMailPanel : MonoBehaviour
{
    public GameObject mailPanel;
    public GameObject contentPanel;
    public GameObject mailButtonPrefab;

    public GameObject mailTextPanel;
    public GameObject mailCloseButton;
    public GameObject mailDeleteButton;

    public GameObject mailImage;

    private GameObject[] prefabContentPanel;

    public GameObject ModalBlockingPanel;
    public Canvas MainCanvas;

    public void Awake()
    {
        mailCloseButton.GetComponent<Button>().onClick.RemoveAllListeners();
        mailCloseButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            ModalBlockingPanel.SetActive(false);
            MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
        });
    }

    public void DisplayAllMail(List<Mail> mailbox)
    {
        if (this.prefabContentPanel != null)
            this.RemoveMailPanels();
        int mailSize = mailbox.Count;

        float prefabHeight = mailButtonPrefab.GetComponent<RectTransform>().rect.height;
        float parentHeight = mailButtonPrefab.GetComponent<RectTransform>().rect.height;

        if(mailSize * prefabHeight > parentHeight)
        {
            float currentLowerX = contentPanel.GetComponent<RectTransform>().offsetMin.x;
            float newLowerY = parentHeight - (float)mailSize * prefabHeight;

           contentPanel.GetComponent<RectTransform>().offsetMin = new Vector2(currentLowerX, 0);
        }

        prefabContentPanel = new GameObject[mailSize];

        int panelInd = 0;
        foreach (Mail mailInstance in mailbox)
        {
            Mail mail = mailInstance;

            MakePanel(mail, panelInd);

            Button thisButton = prefabContentPanel[panelInd].GetComponent<Button>();
            prefabContentPanel[panelInd].GetComponent<Button>().onClick.RemoveAllListeners();
            prefabContentPanel[panelInd].GetComponent<Button>().onClick.AddListener(() =>
            {     
                ModalBlockingPanel.SetActive(true);
                MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
                mailTextPanel.SetActive(true);
                mail.Read = true;
                thisButton.image.color = new Color(179, 181, 121);

                mailTextPanel.transform.Find("Date").GetComponent<Text>().text = "Date: " + mail.StringDate;
                mailTextPanel.transform.Find("Sender").GetComponent<Text>().text = "From: " + mail.Sender;
                mailTextPanel.transform.Find("Subject").GetComponent<Text>().text = "Subject: " + mail.Subject;
                mailTextPanel.transform.Find("MessageText").GetComponent<Text>().text = mail.Message;

                if (mail.Image != null)
                {
                    Sprite mailSprite = Resources.Load<Sprite>("mail/" + mail.Image);
                    mailImage.GetComponent<Image>().sprite = mailSprite;
                    mailImage.SetActive(true);
                }
                else
                {
                    mailImage.SetActive(false);
                }

                mailDeleteButton.GetComponent<Button>().onClick.RemoveAllListeners();
                mailDeleteButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    mailbox.Remove(mail);
                    ModalBlockingPanel.SetActive(false);
                    MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    this.DisplayAllMail(mailbox);
                });
            });
            panelInd++;
        }

    }

    public void RemoveMailPanels()
    {
        for (int i = 0; i < prefabContentPanel.Length; i++)
        {
            Destroy(prefabContentPanel[i]);
        }
        Array.Clear(prefabContentPanel, 0, prefabContentPanel.Length);
    }

    private void MakePanel(Mail mail, int panelInd)
    {
        prefabContentPanel[panelInd] = Instantiate(mailButtonPrefab) as GameObject;
        prefabContentPanel[panelInd].transform.SetParent(contentPanel.transform, false);

        // Move to correct location
        float height = prefabContentPanel[panelInd].GetComponent<RectTransform>().rect.height;
        float current_x = prefabContentPanel[panelInd].GetComponent<RectTransform>().anchoredPosition.x;
        float current_y = prefabContentPanel[panelInd].GetComponent<RectTransform>().anchoredPosition.y;
        prefabContentPanel[panelInd].GetComponent<RectTransform>().anchoredPosition =
            new Vector2(current_x, current_y - (float)panelInd * height);

        // Set name
        prefabContentPanel[panelInd].GetComponentInChildren<Text>().text = string.Format("{0}\n{1}", mail.StringDate, mail.Subject);

        if (!mail.Read)
        {
            prefabContentPanel[panelInd].GetComponent<MailAlertPlayer>().PlayNotification();
        }
        
    }
}