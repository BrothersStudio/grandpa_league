using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Main : MonoBehaviour {

	public Button[] days;
	private int current_day;

	public void Awake()
	{
		InitializeHighlight ();
	}

	public void InitializeHighlight()
	{
		current_day = 0;
		days [0].image.color = Color.red;
	}

	public void AdvanceDayHighlight()
	{
		days [current_day].image.color = Color.white;
		if (current_day == days.Length - 1) 
		{
			current_day = 0;
		}
		else
		{
			current_day++;
		}
		days [current_day].image.color = Color.red;
	}
}
