using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadLeaguePanel : MonoBehaviour {

	public GameObject league_panel;

	public GameObject league_placement_button_prefab;

	private List<GameObject> league_placement_button_prefab_list = new List<GameObject>();

	public void DisplayLeagueStandings(Family PlayerFamily, List<Family> LeagueFamilies)
	{
		LeagueFamilies.Add (PlayerFamily);

		List<double> grandpa_prides = new List<double>();
		foreach (Family family in LeagueFamilies) 
		{
			grandpa_prides.Add (family.Grandpa.Pride);
		}
		grandpa_prides.Sort ();

		int placement_number = 0;
		foreach (double pride in grandpa_prides) 
		{
			foreach (Family family in LeagueFamilies) 
			{
				if (pride == family.Grandpa.Pride) 
				{
					MakePanel (family, league_placement_button_prefab_list, league_panel, placement_number);
				}
			}

			// If there are more than 8 families, only display 8.
			placement_number++;
			if (placement_number == 8) 
			{
				break;
			}
		}
	}

	private void MakePanel(Family family, List<GameObject> prefab_list, GameObject parent_panel, int button_inds)
	{

		GameObject new_button = Instantiate(league_placement_button_prefab) as GameObject;
		new_button.transform.SetParent(parent_panel.transform, false);

		// Move to correct location
		float height = new_button.GetComponent<RectTransform> ().rect.height;
		float current_x = new_button.GetComponent<RectTransform> ().anchoredPosition.x;
		float current_y = new_button.GetComponent<RectTransform> ().anchoredPosition.y;
		new_button.GetComponent<RectTransform> ().anchoredPosition = 
			new Vector2 (current_x, current_y - (float)button_inds * height);

		// Set name
		new_button.GetComponentInChildren<Text>().text = family.FamilyName;

		prefab_list.Add (new_button);
	}
}

