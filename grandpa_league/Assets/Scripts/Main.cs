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

	public void Awake()
	{
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
            //DISPLAY THE DESCRIPTION OF THE EVENT AND PROMPT USER FOR INPUT
            string name = ev.EventName;
            string description = ev.EventDescription;
            var day = m_dataManager.Calendar.GetCurrentDay();
            Debug.Log(Qualification.GetQualificationString(ev.Requirements.Qualification) + " " + day["month"] + "/" + day["day"] + "/" + day["year"]);

            //LOOP THROUGH ALL OF THE REQUIREMENTS FOR EVENT AND PROMPT USER FOR INPUT IF NEEDED
            ev.Requirements.Accept = true;
            ev.Requirements.Child = m_dataManager.PlayerFamily.Children[0];

            //EXECUTE THE EVENT
            Outcome eventOutcome = ev.RunEvent(m_dataManager);

            //CHECK THE OUTCOME
            if (eventOutcome.Status == (int)Enums.EventOutcome.SUCCESS)
            {
                Debug.Log(eventOutcome.OutcomeDescription);
                continue;
                //OUTPUT STRING FOR EVENT HERE (TODO will be something like ev.GetString(eventOutCome);
                //if it is a "soft" fail e.g. not enough money or child too young then start loop over
            }
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
