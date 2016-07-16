using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	public Button[] days;
	private int current_day;

	public Text month_title;
	private string[] months;
	private int current_month;

    private static DataManager m_dataManager = new DataManager("player1");
    
	public void Awake()
	{
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
			if (current_month == 12)
			{
				current_month = 0;
			}
			month_title.text = months[current_month];
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
            //DISPLAY THE DESCRIPTION OF THE EVENT AND PROMPT USER FOR INPUT
            string name = ev.EventName;
            string description = ev.EventDescription;

            //LOOP THROUGH ALL OF THE REQUIREMENTS FOR EVENT AND PROMPT USER FOR INPUT IF NEEDED
            ev.Requirements.Accept = true;

            //EXECUTE THE EVENT
            int eventOutcome = ev.RunEvent(m_dataManager);

            //CHECK THE OUTCOME
            if (eventOutcome == (int)Enums.EventOutcome.SUCCESS)
            {
                continue;
                //OUTPUT STRING FOR EVENT HERE (TODO will be something like ev.GetString(eventOutCome);
                //if it is a "soft" fail e.g. not enough money or child too young then start loop over
            }
        }

        m_dataManager.Calendar.AdvanceDay();    //once all the event processing done we update the calendar day
    }
	
	private void InitializeHighlight()
	{
		InitializeMonthNames ();        
		current_day = 0;
		days [0].image.color = Color.red;

		current_month = 0;
		month_title.text = months[current_month];
	}

	private void InitializeMonthNames()
	{
		months = new string[12];
		months [0]  = "January";
		months [1]  = "February";
		months [2]  = "March";
		months [3]  = "April";
		months [4]  = "May";
		months [5]  = "June";
		months [6]  = "July";
		months [7]  = "August";
		months [8]  = "September";
		months [9]  = "October";
		months [10] = "November";
		months [11] = "December";
	}
}
