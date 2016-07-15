using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Main : MonoBehaviour {

	public Button[] days;
	private int current_day;

	public Text month_title;
	private string[] months;
	private int current_month;

	public void Awake()
	{
		InitializeMonthNames ();
		InitializeHighlight ();
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
		
	private void InitializeHighlight()
	{
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
