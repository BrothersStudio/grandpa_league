using UnityEngine;
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
    public GameObject user_input_panel;
    bool panelUp = false;

    public void Update()
    {
        while (panelUp)
            break;
    }

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
        foreach (SimulationEvent ev in m_dataManager.Calendar.GetEventsForCurrentDay())
        {
            Debug.Assert(ev != null);
            var day = m_dataManager.Calendar.GetCurrentDay();
            string debugString = String.Format("Currently running event \"{0}\", ID: {1}  on date {2}/{3}/{4}. Has qualification {5} and age requirement {6}-{7}.\n Requires Random: Parent? {8}, Child? {9}, Grandpa? {10} \n Requires: Money? {11}, Accept/Reject? {12} \n Also has minMonth {13} and maxMonth {14}. Priority {15}",
                                    ev.EventName, ev.EventId, day["month"], day["day"], day["year"], Qualification.GetQualificationString(ev.Requirements.Qualification), ev.Requirements.MinAge, ev.Requirements.MaxAge, ev.Requirements.RandomParent, ev.Requirements.RandomChild, ev.Requirements.RandomGrandpa,
                                    ev.Requirements.ReqMoney, ev.Requirements.ReqAccept, ev.EventMonth, ev.EventMonthMax, ev.Priority);
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
                ev.Requirements.Child = m_dataManager.PlayerFamily.GetRandomEligibleChild(ev.Requirements.MinAge, ev.Requirements.MaxAge);
            if (ev.Requirements.RandomParent && ev.Requirements.Parent == null)
                ev.Requirements.Parent = m_dataManager.PlayerFamily.GetRandomParent();
            if (ev.Requirements.RandomGrandpa && ev.Requirements.Grandpa == null)
                ev.Requirements.Grandpa = m_dataManager.LeagueFamilies[Constants.RANDOM.Next(0, m_dataManager.LeagueFamilies.Count)].Grandpa;


            //DISPLAY THE DESCRIPTION OF THE EVENT AND PROMPT USER FOR INPUT
            //Use requirement object to generate the panel which will get the input from the user, display the name and discription (if necessary)

            user_input_panel.GetComponentsInChildren<Button>()[0].onClick.AddListener(() =>
            {
                panelUp = false;
            });
            user_input_panel.SetActive(true);
            panelUp = true;


            //EXECUTE THE EVENT
            Outcome eventOutcome = ev.RunEvent(m_dataManager);
            Debug.Log("event completed");
            //CHECK THE OUTCOME
            if (eventOutcome.Status == (int)Enums.EventOutcome.PASS)
                break;

            //Otherwise display the outcome panel with text eventOutcome.OutcomeDescription
            //send mail to mail panel using eventOutcome.Mail
        }

        m_dataManager.Calendar.AdvanceDay();    //once all the event processing done we update the calendar day
		AdvanceDayHighlight();
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
