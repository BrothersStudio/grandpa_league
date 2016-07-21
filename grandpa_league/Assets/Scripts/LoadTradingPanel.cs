using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadTradingPanel : MonoBehaviour {

	public GameObject prefab_family_list_button;

	public GameObject player_family_content_panel;
	public GameObject enemy_families_content_panel;

	private GameObject[] player_family_button_instance;
	private GameObject[] enemy_family_button_instance;

	private GameObject[] enemy_family_subfamily_instance;

	public void DisplayAllFamilies (Family PlayerFamily, List<Family> leagueFamilies) 
	{
		DisplayPlayerFamily (PlayerFamily);
		DisplayEnemyFamilies (leagueFamilies);
	}

	public void DestroyPanels ()
	{
		// Player Family
		for (int i = 0; i < player_family_button_instance.Length; i++)
		{
			Destroy (player_family_button_instance [i]);
		}
		Array.Clear(player_family_button_instance, 0, player_family_button_instance.Length);

		// Enemy Families
		for (int i = 0; i < enemy_family_button_instance.Length; i++)
		{
			Destroy (enemy_family_button_instance [i]);
		}
		Array.Clear(enemy_family_button_instance, 0, enemy_family_button_instance.Length);

		// Clear old enemy families subinstances if they exist
		if (enemy_family_subfamily_instance != null)
		{
			for (int i = 0; i < enemy_family_subfamily_instance.Length; i++)
			{
				Destroy (enemy_family_subfamily_instance [i]);
			}
			Array.Clear(enemy_family_subfamily_instance, 0, enemy_family_subfamily_instance.Length);
		}
	}

	private void DisplayPlayerFamily (Family PlayerFamily)
	{
		int family_size = PlayerFamily.FamilySize - 1;

		// Fit scroll panel to correct size
		float prefab_height = prefab_family_list_button.GetComponent<RectTransform> ().rect.height;
		float parent_height = player_family_content_panel.GetComponent<RectTransform> ().rect.height;

		float current_lower_x = player_family_content_panel.GetComponent<RectTransform> ().offsetMin.x;
		float new_lower_y = parent_height - (float)family_size * prefab_height;

		player_family_content_panel.GetComponent<RectTransform>().offsetMin = new Vector2(current_lower_x, new_lower_y);

		player_family_button_instance = new GameObject[family_size];

		// Only display parents and children
		int panel_ind = -1;
		foreach (Parent parent_instance in PlayerFamily.Parents) 
		{
			panel_ind++;
			Parent parent = parent_instance;

			MakePanel (parent, panel_ind);

			// Add character display activation to button
			player_family_button_instance[panel_ind].GetComponent<Button>().onClick.AddListener(() => 
				{

				});
		}

		foreach (Child child_instance in PlayerFamily.Children) 
		{
			panel_ind++;
			Child child = child_instance;

			MakePanel (child, panel_ind);

			// Add character display activation to button
			player_family_button_instance[panel_ind].GetComponent<Button>().onClick.AddListener(() => 
				{

				});
		}
	}

	private void DisplayEnemyFamilies (List<Family> leagueFamilies)
	{
		int max_members = 0;
		foreach (Family family in leagueFamilies) 
		{
			max_members += family.FamilySize;
		}

		// Fit scroll panel to correct size
		float prefab_height = prefab_family_list_button.GetComponent<RectTransform> ().rect.height;
		float parent_height = enemy_families_content_panel.GetComponent<RectTransform> ().rect.height;

		float current_lower_x = enemy_families_content_panel.GetComponent<RectTransform> ().offsetMin.x;
		float new_lower_y = parent_height - (float)max_members * prefab_height;

		enemy_families_content_panel.GetComponent<RectTransform>().offsetMin = new Vector2(current_lower_x, new_lower_y);

		enemy_family_button_instance = new GameObject[leagueFamilies.Count];

		int panel_ind = -1;
		foreach (Family family_instance in leagueFamilies) 
		{
			panel_ind++;

			// Need to reinstantiate this for button
			int current_panel_ind = panel_ind;
			Family family = family_instance;
			List<Family> theseFamilies = leagueFamilies;

			MakeEnemyFamiliesPanel (family, current_panel_ind);

			// Add subfamily display activation to button
			enemy_family_button_instance[current_panel_ind].GetComponent<Button>().onClick.AddListener(() => 
				{
					// Clear old enemy families subinstances if they exist
					if (enemy_family_subfamily_instance != null)
					{
						for (int i = 0; i < enemy_family_subfamily_instance.Length; i++)
						{
							Destroy (enemy_family_subfamily_instance [i]);
						}
						Array.Clear(enemy_family_subfamily_instance, 0, enemy_family_subfamily_instance.Length);
					}

					enemy_family_subfamily_instance = new GameObject[family.FamilySize - 1];

					int family_ind = -1;
					foreach (Parent parent in family.Parents)
					{
						family_ind++;
						enemy_family_subfamily_instance[family_ind] = Instantiate(prefab_family_list_button) as GameObject;
						enemy_family_subfamily_instance[family_ind].transform.SetParent(enemy_families_content_panel.transform, false);

						// Move to correct location
						float height = enemy_family_subfamily_instance[family_ind].GetComponent<RectTransform> ().rect.height;
						float current_x = enemy_family_subfamily_instance[family_ind].GetComponent<RectTransform> ().anchoredPosition.x;
						float current_y = enemy_family_subfamily_instance[family_ind].GetComponent<RectTransform> ().anchoredPosition.y;
						enemy_family_subfamily_instance[family_ind].GetComponent<RectTransform> ().anchoredPosition = 
							new Vector2 (current_x, current_y - (float)(family_ind + current_panel_ind + 1) * height);

						// Set name
						enemy_family_subfamily_instance[family_ind].GetComponentInChildren<Text>().text = parent.Name;
					}

					foreach (Child child in family.Children)
					{
						family_ind++;
						enemy_family_subfamily_instance[family_ind] = Instantiate(prefab_family_list_button) as GameObject;
						enemy_family_subfamily_instance[family_ind].transform.SetParent(enemy_families_content_panel.transform, false);

						// Move to correct location
						float height = enemy_family_subfamily_instance[family_ind].GetComponent<RectTransform> ().rect.height;
						float current_x = enemy_family_subfamily_instance[family_ind].GetComponent<RectTransform> ().anchoredPosition.x;
						float current_y = enemy_family_subfamily_instance[family_ind].GetComponent<RectTransform> ().anchoredPosition.y;
						enemy_family_subfamily_instance[family_ind].GetComponent<RectTransform> ().anchoredPosition = 
							new Vector2 (current_x, current_y - (float)(family_ind + current_panel_ind + 1) * height);

						// Set name
						enemy_family_subfamily_instance[family_ind].GetComponentInChildren<Text>().text = child.Name;
					}

					float last_y = enemy_family_subfamily_instance[family_ind].GetComponent<RectTransform> ().anchoredPosition.y;
					for (int i = current_panel_ind + 1; i < theseFamilies.Count; i++)
					{
						float height = enemy_family_button_instance[i].GetComponent<RectTransform> ().rect.height;
						float current_x = enemy_family_button_instance[i].GetComponent<RectTransform> ().anchoredPosition.x;
						float current_y = enemy_family_button_instance[i].GetComponent<RectTransform> ().anchoredPosition.y;
						enemy_family_button_instance[i].GetComponent<RectTransform> ().anchoredPosition = 
							new Vector2 (current_x, last_y - (float)(i - current_panel_ind) * height);
					}
				});
		}
	}

	private void MakePanel<T>(T member, int panel_ind) where T : Character
	{
		player_family_button_instance[panel_ind] = Instantiate(prefab_family_list_button) as GameObject;
		player_family_button_instance[panel_ind].transform.SetParent(player_family_content_panel.transform, false);

		// Move to correct location
		float height = player_family_button_instance[panel_ind].GetComponent<RectTransform> ().rect.height;
		float current_x = player_family_button_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition.x;
		float current_y = player_family_button_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition.y;
		player_family_button_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition = 
			new Vector2 (current_x, current_y - (float)panel_ind * height);

		// Set name
		player_family_button_instance[panel_ind].GetComponentInChildren<Text>().text = member.Name;
	}

	private void MakeEnemyFamiliesPanel(Family family, int panel_ind)
	{
		enemy_family_button_instance[panel_ind] = Instantiate(prefab_family_list_button) as GameObject;
		enemy_family_button_instance[panel_ind].transform.SetParent(enemy_families_content_panel.transform, false);

		// Move to correct location
		float height = enemy_family_button_instance[panel_ind].GetComponent<RectTransform> ().rect.height;
		float current_x = enemy_family_button_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition.x;
		float current_y = enemy_family_button_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition.y;
		enemy_family_button_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition = 
			new Vector2 (current_x, current_y - (float)panel_ind * height);

		// Set name
		enemy_family_button_instance[panel_ind].GetComponentInChildren<Text>().text = family.FamilyName;
	}
}
