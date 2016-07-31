#define DEBUG

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Main : MonoBehaviour {

	public Button[] days;
	private int current_day = 0;
	private int current_month = 1;
	private int display_month = 1;
	public Text month_title;
	public GameObject month_forward_button;
	public GameObject month_back_button;

	public GameObject family_panel;
	public GameObject trading_panel;
	public GameObject league_panel;
    public GameObject mail_panel;
	public GameObject abilities_panel;

    private static DataManager m_dataManager;

    /* user input panel objects */
    public Canvas EventCanvas;
    public GameObject AbilitiesButton;

    public GameObject known_event_display_panel;

    public GameObject EventOutcomePanel;
    public GameObject OutcomeTextbox;
    public GameObject OkButton;

    public GameObject characterButtonPrefab;
    public GameObject ChildSelectPanel;
    public GameObject ParentSelectPanel;
    public GameObject GrandpaSelectPanel;
    public GameObject childBackButton;
    public GameObject parentBackButton;
    public GameObject grandpaBackButton;
    public GameObject SelectionModalBlockingPanel;

    public GameObject user_input_panel;
    public GameObject AcceptButton;
    public GameObject RejectButton;
    public GameObject SelectParentButton;
    public GameObject SelectChildButton;
    public GameObject AgeRequirementText;
    public GameObject SelectGrandpaButton;
    public GameObject MoneyInputField;
    public GameObject CurrentMoneyText;
    public GameObject EventTitleText;
    public GameObject EventDescriptionText;
    public GameObject EventFamilyPanelButton;

    public GameObject ModalBlockingPanel;
    public Canvas MainCanvas;

    public GameObject SettingsPanel;
    public GameObject SaveButton;
    public GameObject ResumeButton;
    public GameObject QuitButton;

    public Camera SceneCamera;

    //declaring variable for the audio player for mail
    //public AudioSource AudioPlayer;

	public void Awake()
	{
        user_input_panel.SetActive(false);

        if (PlayerPrefs.GetString("load") == "load")
        {
            try
            {
                this.Load();
                Dictionary<string, int> currentDate = m_dataManager.Calendar.GetCurrentDay();
                this.current_month = currentDate["month"];
                this.current_day = currentDate["day"] - 1;
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            PlayerPrefs.DeleteKey("load");
        }
        else
        {
            m_dataManager = new DataManager(PlayerPrefs.GetString("name"));
            PlayerPrefs.DeleteKey("name");
        }

        this.DisplayContent("mail");

#if (DEBUG)
        Globals.RunUnitTests();
#endif
        InitializeHighlight ();
	}

    public void AdvanceDay()
    {
        StartCoroutine(SimulateDay());

        if (m_dataManager.PlayerInfo.ABILITIES_DISABLED && current_month >= 3)
            AbilitiesButton.GetComponent<Button>().interactable = true;

        foreach(Ability abilities in m_dataManager.Abilities)
        {
            if(abilities.CurrentCooldown > 0)
            {
                abilities.CurrentCooldown--;
            }
        }

        if(current_month == 12 && current_day == 27)
        {
            SceneCamera.transform.position = new Vector3(SceneCamera.transform.position.x, SceneCamera.transform.position.y, 100);
        }
    }

    public IEnumerator SimulateDay()
    {
        foreach (SimulationEvent curEvent in m_dataManager.Calendar.GetEventsForCurrentDay())
        {
            SimulationEvent ev = curEvent;
            Debug.Assert(ev != null);
            var day = m_dataManager.Calendar.GetCurrentDay();
            string debugString = String.Format("Currently running event \"{0}\", ID: {1}  on date {2}/{3}/{4}. Has qualification {5} and age requirement {6}-{7}.\n Requires Random: Parent? {8}, Child? {9}, Grandpa? {10} \n Requires: Money? {11}, Accept/Reject? {12}, Child? {16} Parent? {17} \n Also has minMonth {13} and maxMonth {14}. Priority {15}",
                                    ev.EventName, ev.EventId, day["month"], day["day"], day["year"], Qualification.GetQualificationString(ev.Requirements.Qualification), ev.Requirements.MinAge, ev.Requirements.MaxAge, ev.Requirements.RandomParent, ev.Requirements.RandomChild, ev.Requirements.RandomGrandpa,
                                    ev.Requirements.ReqMoney, ev.Requirements.ReqAccept, ev.EventMonth, ev.EventMonthMax, ev.Priority, ev.Requirements.ReqChild, ev.Requirements.ReqParent);
            Debug.Log(debugString);

            if (m_dataManager.BlacklistYear.Contains(ev.EventId) || m_dataManager.BlacklistForever.Contains(ev.EventId))
            {
                Debug.Log(string.Format("Event {0} blacklisted... skipping", ev.EventName));
                continue;
            }

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
                {
                    Debug.Log(String.Format("No one has qual {0} AND meet age requirements, skipping event", ev.Requirements.Qualification));
                    continue;      //immediately exit the event since no one has the qualificaiton STOP. THE FUNCTION. STOP HAVING IT BE RUN.
                }

                if (qualChar.GetType() == typeof(Child) && ev.Requirements.RandomChild)
                    ev.Requirements.Child = (Child)qualChar;
                else if (qualChar.GetType() == typeof(Parent) && ev.Requirements.RandomParent)
                    ev.Requirements.Parent = (Parent)qualChar;
                else if (qualChar.GetType() == typeof(Grandpa) && ev.Requirements.RandomGrandpa)
                    ev.Requirements.Grandpa = (Grandpa)qualChar;
            }
            //at this point the function has checked the event qualification requirements and chosen the first qualifying member

            // Check if ANY character has the required age
            bool noneEligible = true;
            foreach (Child child in m_dataManager.PlayerFamily.Children)
            {
                if (child.Age >= ev.Requirements.MinAge && child.Age <= ev.Requirements.MaxAge)
                {
                    noneEligible = false;
                    break;
                }
            }
            if (noneEligible)
            {
                Debug.Log(String.Format("No child exists that matches age requirements, skipping event", ev.Requirements.Qualification));
                continue;
            }

            //now we will loop through and check if we need any random things..(without overwriting the random char we just possibly chose)
            if (ev.Requirements.RandomChild && ev.Requirements.Child == null)
            {
                ev.Requirements.Child = m_dataManager.PlayerFamily.GetRandomEligibleChild(ev.Requirements.MinAge, ev.Requirements.MaxAge);
                if (ev.Requirements.Child == null)
                {
                    Debug.Log("No eligible children found, skipping event");
                    continue;
                }
                Debug.Log(String.Format("chose child:{0} for event", ev.Requirements.Child.Name));
            }
            if (ev.Requirements.RandomParent && ev.Requirements.Parent == null)
            {
                ev.Requirements.Parent = m_dataManager.PlayerFamily.GetRandomParent();
                if (ev.Requirements.Parent == null)
                {
                    Debug.Log("No Parent found, skipping event");
                    continue;
                }
                Debug.Log(String.Format("chose parent:{0} for event", ev.Requirements.Parent.Name));

            }
            if (ev.Requirements.RandomGrandpa && ev.Requirements.Grandpa == null)
            {
                ev.Requirements.Grandpa = m_dataManager.LeagueFamilies[Constants.RANDOM.Next(0, m_dataManager.LeagueFamilies.Count)].Grandpa;
                Debug.Log(String.Format("chose grandpa:{0} for event", ev.Requirements.Grandpa.Name));
            }

            //DISPLAY THE DESCRIPTION OF THE EVENT AND PROMPT USER FOR INPUT
            //Use requirement object to generate the panel which will get the input from the user, display the name and discription (if necessary)
            if (ev.Requirements.HasInputRequirements() || ev.Priority == 2)
            {
                ev.FormatEventDescription(m_dataManager);

				CreateAndDisplayInputPanel(ev);
                Globals.UserInputting = true;

                ModalBlockingPanel.SetActive(true);
                MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
                yield return StartCoroutine("WaitForUserConfirm");
                Globals.UserInputting = false;
                MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                ModalBlockingPanel.SetActive(false);
            }

            //EXECUTE THE EVENT
            Outcome eventOutcome = ev.RunEvent(m_dataManager);

            
            //send mail to mail panel using eventOutcome.Mail
            if (eventOutcome.Mail != null)
            {
                m_dataManager.PlayerFamily.Mailbox.Insert(0, eventOutcome.Mail);
                this.DisplayContent("mail");
                //AudioClip mail = (AudioClip)Resources.Load("mailpop");
                //AudioPlayer.PlayClipAtPoint(mail, Camera.main.transform.position);
                
            }

            //CHECK THE OUTCOME
            if (eventOutcome.Status == (int)Enums.EventOutcome.PASS_BLACKLIST_FOREVER)
            {
                m_dataManager.BlacklistForever.Add(ev.EventId);
                continue;
            }
            else if (eventOutcome.Status == (int)Enums.EventOutcome.PASS_BLACKLIST_YEAR)
            {
                m_dataManager.BlacklistYear.Add(ev.EventId);
                continue;
            }
            else if (eventOutcome.Status == (int)Enums.EventOutcome.PASS)
                continue;
            else if (eventOutcome.Status == (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR || eventOutcome.Status == (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR)
            {
                m_dataManager.BlacklistYear.Add(ev.EventId);
            }
            else if (eventOutcome.Status == (int)Enums.EventOutcome.SUCCESS_BLACKLIST_FOREVER || eventOutcome.Status == (int)Enums.EventOutcome.FAILURE_BLACKLIST_FOREVER)
            {
                m_dataManager.BlacklistForever.Add(ev.EventId);
            }

            if (ev.Priority != 0)
            {
                CreateAndDisplayResultPanel(eventOutcome);
                Globals.UserInputting = true;

                ModalBlockingPanel.SetActive(true);
                MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
                yield return StartCoroutine("WaitForUserConfirm");
                MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                ModalBlockingPanel.SetActive(false);
            }
            Debug.Log(String.Format("event {0} completed", ev.EventName));
            ev.ResetEventFields();
            ev.FinishedExecution = true;
        }

        LeagueManager.SimulateDay(m_dataManager);   //move league standings around and stuff
        m_dataManager.Calendar.AdvanceDay();        //once all the event processing done we update the calendar day
		AdvanceDayHighlight();
    }

    public void CreateAndDisplayResultPanel(Outcome eventOutcome)
    {
        EventOutcomePanel.SetActive(true);
        OutcomeTextbox.GetComponent<Text>().text = eventOutcome.OutcomeDescription;
		OkButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
        OkButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            EventOutcomePanel.SetActive(false);
            Globals.UserInputting = false;
            this.RemoveModalBacking();
        });

        if(eventOutcome.Status == (int)Enums.EventOutcome.SUCCESS || eventOutcome.Status == (int)Enums.EventOutcome.SUCCESS_BLACKLIST_FOREVER || eventOutcome.Status == (int)Enums.EventOutcome.SUCCESS_BLACKLIST_YEAR)
        {
            //PLAY HAPPY SOUND HERE
        }
        else if(eventOutcome.Status == (int)Enums.EventOutcome.FAILURE || eventOutcome.Status == (int)Enums.EventOutcome.FAILURE_BLACKLIST_FOREVER || eventOutcome.Status == (int)Enums.EventOutcome.FAILURE_BLACKLIST_YEAR)
        {
            //PLAY SAD SOUND HERE
        }
    }
		
	public void CreateAndDisplayInputPanel(SimulationEvent ev)
    {
        Child selectedChild = ev.Requirements.Child;
        Parent selectedParent = ev.Requirements.Parent;
        Grandpa selectedGrandpa = ev.Requirements.Grandpa;

        MoneyInputField.GetComponent<InputField>().text = "0";
        SelectChildButton.GetComponentInChildren<Text>().text = "Select Child";
        SelectChildButton.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
        SelectParentButton.GetComponentInChildren<Text>().text = "Select Parent";
        SelectParentButton.GetComponentInChildren<Text>().color = new Color(255, 0, 0);
        SelectGrandpaButton.GetComponentInChildren<Text>().text = "Select Grandpa";
        SelectGrandpaButton.GetComponentInChildren<Text>().color = new Color(255, 0, 0);

        user_input_panel.SetActive(true);
        EventTitleText.GetComponent<Text>().text = ev.EventName;
        EventDescriptionText.GetComponent<Text>().text = ev.EventDescription;

		List<GameObject> child_buttons = new List<GameObject>();
		List<GameObject> parent_buttons = new List<GameObject>();
		List<GameObject> grandpa_buttons = new List<GameObject>();

		SelectChildButton.GetComponent<Button>().onClick.RemoveAllListeners();
        SelectChildButton.GetComponent<Button>().onClick.AddListener(() =>
        {

            ChildSelectPanel.SetActive(true);
            SelectionModalBlockingPanel.SetActive(true);
            EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            int curChild = 0;
            foreach (Child child in m_dataManager.PlayerFamily.Children)
            {
				Child ch = child;

				if (ch.Age > ev.Requirements.MaxAge || ch.Age < ev.Requirements.MinAge)
					continue;
					
                GameObject childButton = Instantiate(characterButtonPrefab) as GameObject;
                childButton.GetComponentInChildren<Text>().text= ch.Name;
                childButton.transform.SetParent(ChildSelectPanel.transform, false);

				childButton.GetComponent<Button>().onClick.RemoveAllListeners();
                childButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedChild = ch;
                    ChildSelectPanel.SetActive(false);
                    SelectionModalBlockingPanel.SetActive(false);
                    EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    SelectChildButton.GetComponentInChildren<Text>().text = ch.Name;
                    SelectChildButton.GetComponentInChildren<Text>().color = new Color (255, 255, 255);
                    if ((!SelectParentButton.activeSelf || (SelectParentButton.activeSelf && selectedParent != null)) && (!SelectGrandpaButton.activeSelf || (SelectGrandpaButton.activeSelf && selectedGrandpa != null)))
                        AcceptButton.GetComponent<Button>().interactable = true;

					foreach (GameObject button in child_buttons)
					{
						Destroy(button);
					}
					child_buttons.Clear();
                });
                float height = childButton.GetComponent<RectTransform>().rect.height;
                float current_x = childButton.GetComponent<RectTransform>().anchoredPosition.x;
                childButton.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1);
                float current_y = childButton.GetComponent<RectTransform>().anchoredPosition.y;
                childButton.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(current_x, (current_y - (float)curChild * height) - 80);
                childButton.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                childButton.GetComponent<Button>().image.color = new Color(100, 180, 100);

				child_buttons.Add(childButton);

                curChild++;
            }
			
			childBackButton.GetComponent<Button>().onClick.RemoveAllListeners();
            childBackButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                ChildSelectPanel.SetActive(false);
                SelectionModalBlockingPanel.SetActive(false);
                EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

				foreach (GameObject button in child_buttons)
				{
					Destroy(button);
				}
				child_buttons.Clear();
            });
        });

		SelectParentButton.GetComponent<Button>().onClick.RemoveAllListeners();
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

				parentButton.GetComponent<Button>().onClick.RemoveAllListeners();
                parentButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedParent = par;
                    ParentSelectPanel.SetActive(false);
                    SelectionModalBlockingPanel.SetActive(false);
                    EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    SelectParentButton.GetComponentInChildren<Text>().text = par.Name;
                    SelectParentButton.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                    if ((!SelectChildButton.activeSelf || (SelectChildButton.activeSelf && selectedChild != null)) && (!SelectGrandpaButton.activeSelf || (SelectGrandpaButton.activeSelf && selectedGrandpa != null)))
                        AcceptButton.GetComponent<Button>().interactable = true;

					foreach (GameObject button in parent_buttons)
					{
						Destroy(button);
					}
					parent_buttons.Clear();
                });
                float height = parentButton.GetComponent<RectTransform>().rect.height;
                float current_x = parentButton.GetComponent<RectTransform>().anchoredPosition.x;
                parentButton.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1);
                float current_y = parentButton.GetComponent<RectTransform>().anchoredPosition.y;
                parentButton.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(current_x, (current_y - (float)curParent * height) - 80);
                parentButton.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                parentButton.GetComponent<Button>().image.color = new Color(100, 180, 100);

				parent_buttons.Add(parentButton);

                curParent++;
            }
			parentBackButton.GetComponent<Button>().onClick.RemoveAllListeners();
            parentBackButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                ParentSelectPanel.SetActive(false);
                SelectionModalBlockingPanel.SetActive(false);
                EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

				foreach (GameObject button in parent_buttons)
				{
					Destroy(button);
				}
				parent_buttons.Clear();
            });
        });

		SelectGrandpaButton.GetComponent<Button>().onClick.RemoveAllListeners();
        SelectGrandpaButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            GrandpaSelectPanel.SetActive(true);
            SelectionModalBlockingPanel.SetActive(true);
            EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            int curFamily = 0;
            foreach (Family family in m_dataManager.LeagueFamilies)
            {
                Grandpa grandpa = family.Grandpa;
                GameObject grandpaButton = Instantiate(characterButtonPrefab) as GameObject;
                grandpaButton.GetComponentInChildren<Text>().text = grandpa.Name;
                grandpaButton.transform.SetParent(GrandpaSelectPanel.transform, false);

				grandpaButton.GetComponent<Button>().onClick.RemoveAllListeners();
                grandpaButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedGrandpa = grandpa;
                    GrandpaSelectPanel.SetActive(false);
                    SelectionModalBlockingPanel.SetActive(false);
                    EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
                    SelectGrandpaButton.GetComponentInChildren<Text>().text = grandpa.Name;
                    SelectGrandpaButton.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                    if ((!SelectChildButton.activeSelf || (SelectChildButton.activeSelf && selectedChild != null)) && (!SelectParentButton.activeSelf || (SelectParentButton.activeSelf && selectedParent != null)))
                        AcceptButton.GetComponent<Button>().interactable = true;

					foreach (GameObject button in grandpa_buttons)
					{
						Destroy(button);
					}
					grandpa_buttons.Clear();
                });
                float height = grandpaButton.GetComponent<RectTransform>().rect.height;
                float current_x = grandpaButton.GetComponent<RectTransform>().anchoredPosition.x;
                grandpaButton.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1);
                float current_y = grandpaButton.GetComponent<RectTransform>().anchoredPosition.y;
                grandpaButton.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(current_x, (current_y - (float)curFamily * height) - 80);
                grandpaButton.GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                grandpaButton.GetComponent<Button>().image.color = new Color(100, 180, 100);

				grandpa_buttons.Add(grandpaButton);

                curFamily++;
            }
			grandpaBackButton.GetComponent<Button>().onClick.RemoveAllListeners();
            grandpaBackButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                ParentSelectPanel.SetActive(false);
                SelectionModalBlockingPanel.SetActive(false);
                EventCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

				foreach (GameObject button in grandpa_buttons)
				{
					Destroy(button);
				}
				grandpa_buttons.Clear();
            });
        });

		AcceptButton.GetComponent<Button>().onClick.RemoveAllListeners();
        AcceptButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Globals.UserInputting = false;
            ev.Requirements.Accept = true;
            ev.Requirements.Money = MoneyInputField.GetComponent<InputField>().text == "" ? 0 : Int32.Parse(MoneyInputField.GetComponent<InputField>().text);
            ev.Requirements.Child = selectedChild;
            ev.Requirements.Parent = selectedParent;
            ev.Requirements.Grandpa = selectedGrandpa;
        });

		RejectButton.GetComponent<Button>().onClick.RemoveAllListeners();
        RejectButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Globals.UserInputting = false;
            ev.Requirements.Accept = false;
        });

        MoneyInputField.GetComponent<InputField>().onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateMoneyInput(input, addedChar); };
        CurrentMoneyText.GetComponent<Text>().text = "$ " +  m_dataManager.PlayerFamily.Grandpa.Money.ToString() + ".00";

        if (ev.Requirements.ReqAccept)
            RejectButton.SetActive(true);
        else
            RejectButton.SetActive(false);

        if (ev.Requirements.ReqMoney)
        {
            MoneyInputField.SetActive(true);
            CurrentMoneyText.SetActive(true);
        }
        else
        {
            MoneyInputField.SetActive(false);
            CurrentMoneyText.SetActive(false);
        }
        if (ev.Requirements.ReqChild && !ev.Requirements.RandomChild)
        {
            SelectChildButton.SetActive(true);
            
            if(ev.Requirements.MinAge > 0)
            {
                AgeRequirementText.SetActive(true);
                AgeRequirementText.GetComponent<Text>().text = string.Format("Ages: {0}-{1}", ev.Requirements.MinAge, ev.Requirements.MaxAge);

            }
        }
        else
        {
            SelectChildButton.SetActive(false);
            AgeRequirementText.SetActive(false);
        }

        if (ev.Requirements.ReqParent && !ev.Requirements.RandomParent)
            SelectParentButton.SetActive(true);
        else
            SelectParentButton.SetActive(false);

        if (ev.Requirements.ReqGrandpa && !ev.Requirements.RandomGrandpa)
            SelectGrandpaButton.SetActive(true);
        else
            SelectGrandpaButton.SetActive(false);

        if (SelectParentButton.activeSelf || SelectChildButton.activeSelf || SelectGrandpaButton.activeSelf)
            AcceptButton.GetComponent<Button>().interactable = false;
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
        while (Globals.UserInputting)
        {
            yield return null;
        }
        yield break;
    }

    public void PopupSettingsMenu()
    {
        SettingsPanel.SetActive(true);
        ModalBlockingPanel.SetActive(true);
        MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
		SaveButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
        SaveButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            this.Save();
        });
		ResumeButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
        ResumeButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            SettingsPanel.SetActive(false);
            ModalBlockingPanel.SetActive(false);
            MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
        });
		QuitButton.GetComponent<Button> ().onClick.RemoveAllListeners ();
        QuitButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    public void DisplayContent(string type)
	{
		if (type == "family") 
		{
			family_panel.GetComponent<LoadFamilyPanel> ().DisplayFamily (m_dataManager.PlayerFamily, true);
		} 
		else if (type == "family_event") 
		{
			family_panel.GetComponent<LoadFamilyPanel> ().DisplayFamily (m_dataManager.PlayerFamily, true, true);
		}
		else if (type == "trading") 
		{
			trading_panel.GetComponent<LoadTradingPanel> ().DisplayAllFamilies (m_dataManager.PlayerFamily, m_dataManager.LeagueFamilies);
		}
		else if (type == "league")
		{
			league_panel.GetComponent<LoadLeaguePanel> ().DisplayLeagueStandings (m_dataManager.PlayerFamily, m_dataManager.LeagueFamilies);
		}
        else if (type == "mail")
        {
            mail_panel.GetComponent<LoadMailPanel>().DisplayAllMail(m_dataManager.PlayerFamily.Mailbox);
        }
		else if (type == "abilities")
		{
			abilities_panel.GetComponent<LoadAbilitiesPanel>().DisplayAbilities(m_dataManager);
		}
	}

	private void HighlightKnownEvents(int month)
	{
		for (int i = 0; i < 28; i++)
		{
			days [i].image.color = Color.white;
			days [i].GetComponent<Button>().onClick.RemoveAllListeners();
		}

		List<int> knownEvents = m_dataManager.Calendar.GetKnownEventDaysForMonth(month);
		foreach (int known_event_day_instance in knownEvents) 
		{
			int known_event_day = known_event_day_instance;

			days [known_event_day].image.color = Color.green;
			days [known_event_day].GetComponent<Button>().onClick.RemoveAllListeners();
			days [known_event_day].GetComponent<Button>().onClick.AddListener(() =>
			{
                ModalBlockingPanel.SetActive(true);
                MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

                known_event_display_panel.transform.Find("Event Text").GetComponent<Text>().text = "";
				known_event_display_panel.SetActive(true);

				List<SimulationEvent> events_of_day = m_dataManager.Calendar.GetEventsForDay(known_event_day+1, month);
				int num_known_events = 0;
				foreach (SimulationEvent this_event_instance in events_of_day)
				{
					SimulationEvent this_event = this_event_instance;

					if (this_event.EventType == 1)
					{
						num_known_events++;
						string event_string = this_event.EventName;
						known_event_display_panel.transform.Find("Event Text").GetComponent<Text>().text += ("\n" + event_string);
					}
					if (num_known_events == 3)
					{
						break;
					}
				}
			});
		}
	}

	private void InitializeHighlight()
	{     
		month_title.text = Constants.MONTH_NAMES[this.current_month];

		month_back_button.SetActive(false);

		HighlightKnownEvents(this.current_month);

		days [current_day].image.color = Color.red;
	}

	public void AdvanceDayHighlight()
	{
		if (current_day == days.Length - 1) 
		{
			current_day = 0;
			current_month++;
			if (current_month > 12)
			{
				current_month = 1;
			}

			for (int i = 0; i < 28; i++) 
			{
				days [i].image.color = Color.white;
				days [i].GetComponent<Button>().onClick.RemoveAllListeners();
			}
        }
		else
		{
			current_day++;
			days[current_day - 1].image.color = Color.white;
		}

		display_month = current_month;
		ChangeDisplayMonth (0);

        days [current_day].image.color = Color.red;
    }

	public void ChangeDisplayMonth(int change)
	{
		display_month = display_month + change;

		month_back_button.SetActive(true);
		month_forward_button.SetActive(true);

		if (display_month == 1) 
		{
			month_back_button.SetActive(false);
		} 
		else if (display_month == 12) 
		{
			month_forward_button.SetActive(false);
		}

		month_title.text = Constants.MONTH_NAMES[display_month];
		HighlightKnownEvents (display_month);
		if (display_month == current_month) 
		{
			days [current_day].image.color = Color.red;
		}
	}

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream myFile = File.Open(Application.persistentDataPath + "/manager.gpa", FileMode.OpenOrCreate);

        bf.Serialize(myFile, m_dataManager);
        myFile.Close();
    }

    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/manager.gpa"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream myFile = File.Open(Application.persistentDataPath + "/manager.gpa", FileMode.Open);
            DataManager loadedManager = (DataManager)bf.Deserialize(myFile);
            myFile.Close();

            m_dataManager = loadedManager;
        }
    }

    public static DataManager GetDataManager()
    {
        return m_dataManager;
    }

    public void RemoveModalBacking()
    {
        ModalBlockingPanel.SetActive(false);
        MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
