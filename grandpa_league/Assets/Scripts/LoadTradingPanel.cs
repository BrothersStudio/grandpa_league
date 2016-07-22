using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadTradingPanel : MonoBehaviour {

	public GameObject prefab_family_list_button;

	public GameObject player_family_content_panel;
	public GameObject enemy_families_content_panel;
	public GameObject player_offer_content_panel;
	public GameObject enemy_offer_content_panel;

	private List<GameObject> player_family_button_list = new List<GameObject>();
	private List<GameObject> enemy_family_button_list = new List<GameObject>();
	private List<GameObject> enemy_family_subfamily_list = new List<GameObject>();

	private List<GameObject> player_offer_list = new List<GameObject>();
	private List<GameObject> enemy_offer_list = new List<GameObject>();

	public void DisplayAllFamilies (Family PlayerFamily, List<Family> LeagueFamilies) 
	{
		DisplayPlayerFamily (PlayerFamily);
		DisplayEnemyFamilies (LeagueFamilies);
	}

	public void DestroyPanels ()
	{
		// Player Family
		foreach (GameObject button in player_family_button_list)
		{
			Destroy(button);
		}
		player_family_button_list.Clear();

		// Enemy families
		foreach (GameObject button in enemy_family_button_list)
		{
			Destroy(button);
		}
		enemy_family_button_list.Clear();

		// Enemy sub families
		foreach (GameObject button in enemy_family_subfamily_list)
		{
			Destroy(button);
		}
		enemy_family_subfamily_list.Clear();
	}

	private void DisplayPlayerFamily (Family PlayerFamily)
	{
		int family_size = PlayerFamily.FamilySize - 1;

		// Fit scroll panel to family size
		float prefab_height = prefab_family_list_button.GetComponent<RectTransform> ().rect.height;
		float parent_height = player_family_content_panel.GetComponent<RectTransform> ().rect.height;

		float current_lower_x = player_family_content_panel.GetComponent<RectTransform> ().offsetMin.x;
		float new_lower_y = parent_height - (float)family_size * prefab_height;

		player_family_content_panel.GetComponent<RectTransform>().offsetMin = new Vector2(current_lower_x, new_lower_y);

		// Only display parents and children
		foreach (Parent parent_instance in PlayerFamily.Parents) 
		{
			Parent parent = parent_instance;

			MakePanel (parent, player_family_button_list, player_family_content_panel);

			// Add character display activation to button
			int buttonInd = player_family_button_list.Count - 1;
			player_family_button_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
				{
					MakePanel (parent, player_offer_list, player_offer_content_panel);

					int newButtonInd = player_offer_list.Count - 1;
					player_offer_list[newButtonInd].GetComponent<Button>().onClick.AddListener(() => 
						{
							Destroy(player_offer_list[newButtonInd]);
							player_offer_list.RemoveAt(newButtonInd);
							player_family_button_list[buttonInd].SetActive(true);
						});
					player_family_button_list[buttonInd].SetActive(false);
				});
		}

		foreach (Child child_instance in PlayerFamily.Children) 
		{
			Child child = child_instance;

			MakePanel (child, player_family_button_list, player_family_content_panel);

			// Add character display activation to button
			int buttonInd = player_family_button_list.Count - 1;
			player_family_button_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
				{
					MakePanel (child, player_offer_list, player_offer_content_panel);

					int newButtonInd = player_offer_list.Count - 1;
					player_offer_list[newButtonInd].GetComponent<Button>().onClick.AddListener(() => 
						{
							Destroy(player_offer_list[newButtonInd]);
							player_offer_list.RemoveAt(newButtonInd);
							player_family_button_list[buttonInd].SetActive(true);
						});
					player_family_button_list[buttonInd].SetActive(false);
				});
		}
	}

	private void DisplayEnemyFamilies (List<Family> leagueFamilies)
	{

		foreach (Family family_instance in leagueFamilies) 
		{
			// Need to reinstantiate this for button
			int current_panel_ind = enemy_family_button_list.Count;
			Family family = family_instance;

			MakeFamilyPanel (family, enemy_family_button_list, enemy_families_content_panel);

			// Add subfamily display activation to button
			enemy_family_button_list[enemy_family_button_list.Count - 1].GetComponent<Button>().onClick.AddListener(() => 
				{
					int family_ind = enemy_family_button_list.Count - 1;

					// Clear old enemy families subinstances if they exist
					foreach (GameObject button in enemy_family_subfamily_list)
					{
						Destroy (button);
					}
					enemy_family_subfamily_list.Clear();

					foreach (Parent parent_instance in family.Parents)
					{
						Parent parent = parent_instance;

						MakePanel (parent, enemy_family_subfamily_list, enemy_families_content_panel, family_ind);

						int buttonInd = enemy_family_subfamily_list.Count - 1;
						// Add character display activation to button
						enemy_family_subfamily_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
							{
								foreach (GameObject button in enemy_family_subfamily_list)
								{
									Destroy (button);
								}
								enemy_family_subfamily_list.Clear();

								MakePanel (parent, enemy_offer_list, enemy_offer_content_panel);
								int newButtonInd = enemy_offer_list.Count - 1;
								enemy_offer_list[newButtonInd].GetComponent<Button>().onClick.AddListener(() => 
									{
										Destroy(enemy_offer_list[newButtonInd]);
										enemy_offer_list.RemoveAt(newButtonInd);
										enemy_family_subfamily_list[buttonInd].SetActive(true);
									});
								enemy_family_subfamily_list[buttonInd].SetActive(false);
							});
					}

					foreach (Child child_instance in family.Children)
					{
						Child child = child_instance;

						MakePanel (child, enemy_family_subfamily_list, enemy_families_content_panel, family_ind);

						int buttonInd = enemy_family_subfamily_list.Count - 1;
						// Add character display activation to button
						enemy_family_subfamily_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
							{
								MakePanel (child, enemy_offer_list, enemy_offer_content_panel);
								int newButtonInd = enemy_offer_list.Count - 1;
								enemy_offer_list[newButtonInd].GetComponent<Button>().onClick.AddListener(() => 
									{
										Destroy(enemy_offer_list[newButtonInd]);
										enemy_offer_list.RemoveAt(newButtonInd);
										enemy_family_subfamily_list[buttonInd].SetActive(true);
									});
								enemy_family_subfamily_list[buttonInd].SetActive(false);
							});
					}

					// Move other family buttons down
					float last_y = enemy_family_subfamily_list[enemy_family_subfamily_list.Count - 1].GetComponent<RectTransform> ().anchoredPosition.y;
					for (int i = family_ind; i < leagueFamilies.Count; i++)
					{
						float height = enemy_family_button_list[i].GetComponent<RectTransform> ().rect.height;
						float current_x = enemy_family_button_list[i].GetComponent<RectTransform> ().anchoredPosition.x;
						float current_y = enemy_family_button_list[i].GetComponent<RectTransform> ().anchoredPosition.y;
						enemy_family_button_list[i].GetComponent<RectTransform> ().anchoredPosition = 
							new Vector2 (current_x, last_y - (float)(i - current_panel_ind) * height);
					}
				});
		}
	}

	private void MakePanel<T>(T member, List<GameObject> prefab_list, GameObject parent_panel, int offset = 0) where T : Character
	{
		
		GameObject new_button = Instantiate(prefab_family_list_button) as GameObject;
		new_button.transform.SetParent(parent_panel.transform, false);

		// Move to correct location
		float height = new_button.GetComponent<RectTransform> ().rect.height;
		float current_x = new_button.GetComponent<RectTransform> ().anchoredPosition.x;
		float current_y = new_button.GetComponent<RectTransform> ().anchoredPosition.y;
		new_button.GetComponent<RectTransform> ().anchoredPosition = 
			new Vector2 (current_x, current_y - (float)(prefab_list.Count + offset) * height);

		// Set name
		new_button.GetComponentInChildren<Text>().text = member.Name;

		prefab_list.Add (new_button);
	}

	private void MakeFamilyPanel(Family family, List<GameObject> prefab_list, GameObject parent_panel)
	{

		GameObject new_button = Instantiate(prefab_family_list_button) as GameObject;
		new_button.transform.SetParent(parent_panel.transform, false);

		// Move to correct location
		float height = new_button.GetComponent<RectTransform> ().rect.height;
		float current_x = new_button.GetComponent<RectTransform> ().anchoredPosition.x;
		float current_y = new_button.GetComponent<RectTransform> ().anchoredPosition.y;
		new_button.GetComponent<RectTransform> ().anchoredPosition = 
			new Vector2 (current_x, current_y - (float)prefab_list.Count * height);

		// Set name
		new_button.GetComponentInChildren<Text>().text = family.FamilyName;

		prefab_list.Add (new_button);
	}
}
