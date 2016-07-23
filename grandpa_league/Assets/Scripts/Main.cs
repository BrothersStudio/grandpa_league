﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class Main : MonoBehaviour {

	public Button[] days;
	private int current_day = 0;

	public Text month_title;
	private int current_month = 1;

	public GameObject family_content_panel;
	public GameObject trading_panel;

    private static DataManager m_dataManager;

    /* user input panel objects */
    public Canvas EventCanvas;
    public GameObject EventOutcomePanel;
    public GameObject OutcomeTextbox;
    public GameObject OkButton;

    public GameObject characterButtonPrefab;
    public GameObject ChildSelectPanel;
    public GameObject ParentSelectPanel;
    public GameObject SelectionModalBlockingPanel;

    public GameObject user_input_panel;
    public GameObject AcceptButton;
    public GameObject RejectButton;
    public GameObject SelectParentButton;
    public GameObject SelectChildButton;
    public GameObject MoneyInputField;
    public GameObject CurrentMoneyText;
    public GameObject EventTitleText;
    public GameObject EventDescriptionText;

    public GameObject ModalBlockingPanel;
    public Canvas MainCanvas;

    bool userInputting = false;


	public void Awake()
	{
        user_input_panel.SetActive(false);
        m_dataManager = new DataManager(PlayerPrefs.GetString("name"));

        InitializeHighlight ();
        List<Child> currentPlayersChildren = m_dataManager.PlayerFamily.Children;
        Grandpa currentPlayersGrandpa = m_dataManager.PlayerFamily.Grandpa;
        List<Family> currentLeagueFamilies = m_dataManager.LeagueFamilies; 
	}

	public void AdvanceDayHighlight()
	{
		days [current_day].image.color = Color.white;
		if (current_day == days.Length - 1) 
		{
			current_day = 0;
			current_month++;
			if (current_month > 12)
			{
				current_month = 1;
			}
			month_title.text = Constants.MONTH_NAMES[current_month];
		}
		else
		{
			current_day++;
		}
		days [current_day].image.color = Color.red;
	}

    public void AdvanceDay()
    {
        StartCoroutine(SimulateDay());
    }

    public IEnumerator SimulateDay()
    {
        foreach (SimulationEvent ev in m_dataManager.Calendar.GetEventsForCurrentDay())
        {
            Debug.Assert(ev != null);
            var day = m_dataManager.Calendar.GetCurrentDay();
            string debugString = String.Format("Currently running event \"{0}\", ID: {1}  on date {2}/{3}/{4}. Has qualification {5} and age requirement {6}-{7}.\n Requires Random: Parent? {8}, Child? {9}, Grandpa? {10} \n Requires: Money? {11}, Accept/Reject? {12}, Child? {16} Parent? {17} \n Also has minMonth {13} and maxMonth {14}. Priority {15}",
                                    ev.EventName, ev.EventId, day["month"], day["day"], day["year"], Qualification.GetQualificationString(ev.Requirements.Qualification), ev.Requirements.MinAge, ev.Requirements.MaxAge, ev.Requirements.RandomParent, ev.Requirements.RandomChild, ev.Requirements.RandomGrandpa,
                                    ev.Requirements.ReqMoney, ev.Requirements.ReqAccept, ev.EventMonth, ev.EventMonthMax, ev.Priority, ev.Requirements.ReqChild, ev.Requirements.ReqParent);
            Debug.Log(debugString);

            if (ev.Requirements.Qualification != Qualification.GetQualificationByString("NONE"))
            { 
                bool hasQual = false;
                Character qualChar = null;
                foreach (Character ch in m_dataManager.PlayerFamily.GetAllCharacters())
                {
                    foreach (int qual in ch.Qualifications)
                        if (qual == ev.Requirements.Qualification && ch.MeetsAgeRequirement(ev.Requirements.MinAge, ev.Requirements.MaxAge))
                        {
                            qualChar = ch;
                            hasQual = true;
                        }
                    if (hasQual) break;
                }
                if (!hasQual)
                    break;      //immediately exit the event since no one has the qualificaiton STOP. THE FUNCTION. STOP HAVING IT BE RUN.

                if (qualChar.GetType() == typeof(Child))
                    ev.Requirements.Child = (Child)qualChar;
                else if (qualChar.GetType() == typeof(Parent))
                    ev.Requirements.Parent = (Parent)qualChar;
                else if (qualChar.GetType() == typeof(Grandpa))
                    ev.Requirements.Grandpa = (Grandpa)qualChar;
            }
            //at this point the function has checked the event qualification requirements and chosen the first qualifying member

            //now we will loop through and check if we need any random things..(without overwriting the random char we just possibly chose)
            if (ev.Requirements.RandomChild && ev.Requirements.Child == null)
            {
                ev.Requirements.Child = m_dataManager.PlayerFamily.GetRandomEligibleChild(ev.Requirements.MinAge, ev.Requirements.MaxAge);
                Debug.Log(String.Format("chose child:{0} for event", ev.Requirements.Child.Name));
            }
            if (ev.Requirements.RandomParent && ev.Requirements.Parent == null)
            {
                ev.Requirements.Parent = m_dataManager.PlayerFamily.GetRandomParent();
                Debug.Log(String.Format("chose parent:{0} for event", ev.Requirements.Parent.Name));

            }
            if (ev.Requirements.RandomGrandpa && ev.Requirements.Grandpa == null)
            {
                ev.Requirements.Grandpa = m_dataManager.LeagueFamilies[Constants.RANDOM.Next(0, m_dataManager.LeagueFamilies.Count)].Grandpa;
                Debug.Log(String.Format("chose grandpa:{0} for event", ev.Requirements.Grandpa.Name));
            }


            //DISPLAY THE DESCRIPTION OF THE EVENT AND PROMPT USER FOR INPUT
            //Use requirement object to generate the panel which will get the input from the user, display the name and discription (if necessary)

            if (ev.Requirements.HasInputRequirements())
            {
                ev.FormatEventDescription(m_dataManager);
                CreateAndDisplayInputPanel(ev);
                userInputting = true;

                ModalBlockingPanel.SetActive(true);
                MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
                yield return StartCoroutine(WaitForUserConfirm());
                MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                ModalBlockingPanel.SetActive(false);
            }

            //EXECUTE THE EVENT
            Outcome eventOutcome = ev.RunEvent(m_dataManager);

            //CHECK THE OUTCOME
            if((ev.Priority == 1 || ev.Priority == 2)  && eventOutcome.Status != (int)Enums.EventOutcome.PASS)
            {
                CreateAndDisplayResultPanel(eventOutcome);
                userInputting = true;

                ModalBlockingPanel.SetActive(true);
                MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
                yield return StartCoroutine(WaitForUserConfirm());
                MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                ModalBlockingPanel.SetActive(false);
            }
            Debug.Log(String.Format("event {0} completed", ev.EventName));
            


            //Otherwise display the outcome panel with text eventOutcome.OutcomeDescription
            //send mail to mail panel using eventOutcome.Mail
        }

        m_dataManager.Calendar.AdvanceDay();    //once all the event processing done we update the calendar day
		AdvanceDayHighlight();
    }

    private void CreateAndDisplayResultPanel(Outcome eventOutcome)
    {
        EventOutcomePanel.SetActive(true);
        OutcomeTextbox.GetComponent<Text>().text = eventOutcome.OutcomeDescription;
        OkButton.GetComponent<Button>().onClick.AddListener(() =>
        {
                userInputting = false;
        });
    }

    private void CreateAndDisplayInputPanel(SimulationEvent ev)
    {
        Child selectedChild = null;
        Parent selectedParent = null;

        user_input_panel.SetActive(true);
        EventTitleText.GetComponent<Text>().text = ev.EventName;
        EventDescriptionText.GetComponent<Text>().text = ev.EventDescription;

        SelectChildButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            ChildSelectPanel.SetActive(true);
            SelectionModalBlockingPanel.SetActive(true);
            EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            int curChild = 0;
            foreach (Child child in m_dataManager.PlayerFamily.Children)
            {
                Child ch = child;
                GameObject childButton = Instantiate(characterButtonPrefab) as GameObject;
                childButton.GetComponentInChildren<Text>().text= ch.Name;
                childButton.transform.SetParent(ChildSelectPanel.transform, false);
                childButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedChild = ch;
                    ChildSelectPanel.SetActive(false);
                    SelectionModalBlockingPanel.SetActive(false);
                    EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    SelectChildButton.GetComponentInChildren<Text>().text = ch.Name;
                    SelectChildButton.GetComponentInChildren<Text>().color = new Color (255, 255, 255);
                });
                float height = childButton.GetComponent<RectTransform>().rect.height;
                float current_x = childButton.GetComponent<RectTransform>().anchoredPosition.x;
                childButton.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1);
                float current_y = childButton.GetComponent<RectTransform>().anchoredPosition.y;
                childButton.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(current_x, (current_y - (float)curChild * height) - 80);
                childButton.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                childButton.GetComponent<Button>().image.color = new Color(100, 180, 100);
                curChild++;
            }
        });

        SelectParentButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            ParentSelectPanel.SetActive(true);
            SelectionModalBlockingPanel.SetActive(true);
            EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            int curParent = 0;
            foreach (Parent parent in m_dataManager.PlayerFamily.Parents)
            {
                Parent par = parent;
                GameObject parentButton = Instantiate(characterButtonPrefab) as GameObject;
                parentButton.GetComponentInChildren<Text>().text = par.Name;
                parentButton.transform.SetParent(ParentSelectPanel.transform, false);
                parentButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedParent = par;
                    ParentSelectPanel.SetActive(false);
                    SelectionModalBlockingPanel.SetActive(false);
                    EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    SelectParentButton.GetComponentInChildren<Text>().text = par.Name;
                    SelectParentButton.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                });
                float height = parentButton.GetComponent<RectTransform>().rect.height;
                float current_x = parentButton.GetComponent<RectTransform>().anchoredPosition.x;
                parentButton.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1);
                float current_y = parentButton.GetComponent<RectTransform>().anchoredPosition.y;
                parentButton.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(current_x, (current_y - (float)curParent * height) - 80);
                parentButton.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                parentButton.GetComponent<Button>().image.color = new Color(100, 180, 100);
                curParent++;
            }
        });

        AcceptButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            userInputting = false;
            ev.Requirements.Money = Int32.Parse(MoneyInputField.GetComponent<InputField>().text);
            ev.Requirements.Child = selectedChild;
            ev.Requirements.Parent = selectedParent;
        });

        RejectButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            userInputting = false;
            ev.Requirements.Accept = false;
        });

        MoneyInputField.GetComponent<InputField>().onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateMoneyInput(input, addedChar); };
        CurrentMoneyText.GetComponent<Text>().text = "$ " +  m_dataManager.PlayerFamily.Grandpa.Money.ToString() + ".00";
        CurrentMoneyText.SetActive(true);

        if (ev.Requirements.ReqAccept)
            RejectButton.SetActive(true);
        else
            RejectButton.SetActive(false);

        if (ev.Requirements.ReqMoney)
            MoneyInputField.SetActive(true);
        else
            MoneyInputField.SetActive(false);

        if (ev.Requirements.ReqChild && !ev.Requirements.RandomChild)
            SelectChildButton.SetActive(true);
        else
            SelectChildButton.SetActive(false);

        if (ev.Requirements.ReqParent && !ev.Requirements.RandomParent)
            SelectParentButton.SetActive(true);
        else
            SelectParentButton.SetActive(false);
    }
    
    private char ValidateMoneyInput(string input, char addedChar)
    {
        int result;
        int.TryParse(input + addedChar, out result);
        if (result > m_dataManager.PlayerFamily.Grandpa.Money)
        {
            MoneyInputField.GetComponent<InputField>().textComponent.color = new Color(244, 0, 0);
            return '\0';
        }

        MoneyInputField.GetComponent<InputField>().textComponent.color = new Color(233, 233, 233);
        return addedChar;
    }

    private IEnumerator WaitForUserConfirm()
    {
        while (userInputting)
        {
            yield return null;
        }
    }


    public void DisplayContent(string type)
	{
		if (type == "family") 
		{
			family_content_panel.GetComponent<LoadFamilyPanel> ().DisplayFamily (m_dataManager.PlayerFamily);
		} 
		else if (type == "trading") 
		{
			trading_panel.GetComponent<LoadTradingPanel> ().DisplayAllFamilies (m_dataManager.PlayerFamily, m_dataManager.LeagueFamilies);
		}
	}

	private void InitializeHighlight()
	{     
		days [current_day].image.color = Color.red;

		month_title.text = Constants.MONTH_NAMES[1];
	}

    public static DataManager GetDataManager()
    {
        return m_dataManager;
    }
}
