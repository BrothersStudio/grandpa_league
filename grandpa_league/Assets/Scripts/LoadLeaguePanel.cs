using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadLeaguePanel : MonoBehaviour {

	public GameObject league_panel;

	public GameObject league_placement_button_prefab;
	public GameObject league_placement_score_prefab;

	public GameObject family_panel;
	public GameObject family_panel_content;

	private List<GameObject> league_placement_button_prefab_list = new List<GameObject>();
	private List<GameObject> league_placement_score_prefab_list = new List<GameObject>();

	public void DisplayLeagueStandings(Family PlayerFamily, List<Family> LeagueFamilies)
	{
		List<Family> theseFamilies = new List<Family>();
		foreach (Family family in LeagueFamilies) 
		{
			theseFamilies.Add (family);
		}
		theseFamilies.Add (PlayerFamily);

		List<double> grandpa_prides = new List<double>();
		foreach (Family family in theseFamilies) 
		{
			grandpa_prides.Add (family.Grandpa.Pride);
		}
		grandpa_prides.Sort ();
		grandpa_prides.Reverse ();

		int placement_number = 0;
		foreach (double pride in grandpa_prides) 
		{
			foreach (Family family in theseFamilies) 
			{
				if (pride == family.Grandpa.Pride) 
				{
					MakeFamilyButton (family, league_placement_button_prefab_list, league_panel, placement_number);

					DisplayFamily (family, placement_number);

					MakeScoreText (family, league_placement_score_prefab_list, league_panel, placement_number);
					theseFamilies.Remove (family);
					break;
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

	private void MakeFamilyButton(Family family, List<GameObject> prefab_list, GameObject parent_panel, int button_inds)
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
		new_button.GetComponentInChildren<Text>().text = " " + family.Grandpa.Name;

		prefab_list.Add (new_button);
	}

	private void MakeScoreText(Family family, List<GameObject> prefab_list, GameObject parent_panel, int button_inds)
	{
		GameObject new_score = Instantiate(league_placement_score_prefab) as GameObject;
		new_score.transform.SetParent(parent_panel.transform, false);

		// Move to correct location
		float height = new_score.GetComponent<RectTransform> ().rect.height;
		float current_x = new_score.GetComponent<RectTransform> ().anchoredPosition.x;
		float current_y = new_score.GetComponent<RectTransform> ().anchoredPosition.y;
		new_score.GetComponent<RectTransform> ().anchoredPosition = 
			new Vector2 (current_x, current_y - (float)button_inds * height);

		// Set name
		new_score.GetComponentInChildren<Text>().text = family.Grandpa.Pride.ToString();

		prefab_list.Add (new_score);
	}

	private void DisplayFamily(Family family, int button_inds)
	{
		league_placement_button_prefab_list[button_inds].GetComponent<Button>().onClick.AddListener(() =>
			{
				family_panel.SetActive(true);
				family_panel_content.GetComponent<LoadFamilyPanel> ().DisplayFamily (family);
			});
	}

	public void DestroyPanels ()
	{
		foreach (GameObject button in league_placement_button_prefab_list) 
		{
			Destroy (button);
		}
		league_placement_button_prefab_list.Clear ();

		foreach (GameObject button in league_placement_score_prefab_list) 
		{
			Destroy (button);
		}
		league_placement_score_prefab_list.Clear ();
	}
}

