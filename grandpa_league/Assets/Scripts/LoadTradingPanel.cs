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

	public GameObject offer_money_field;
	public GameObject receive_money_field;

	private List<GameObject> player_family_button_list = new List<GameObject>();
	private List<GameObject> enemy_family_button_list = new List<GameObject>();
	private List<GameObject> enemy_family_subfamily_list = new List<GameObject>();

	private List<Parent> offer_parents = new List<Parent> ();
	private List<Child> offer_children = new List<Child> ();
	private int offer_money = 0;

	private Family receive_family;
	private List<Parent> receive_parents = new List<Parent> ();
	private List<Child> receive_children = new List<Child> ();
	private int receive_money = 0;

	private List<Family> leagueFamilies;

	public void DisplayAllFamilies (Family PlayerFamily, List<Family> LeagueFamilies) 
	{
		leagueFamilies = LeagueFamilies;
		DisplayPlayerFamily (PlayerFamily);
		DisplayEnemyFamilies ();
	}

	public void DestroyPanels ()
	{
		ClearOffers ();

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

		if (family_size * prefab_height > parent_height) 
		{
			float current_lower_x = player_family_content_panel.GetComponent<RectTransform> ().offsetMin.x;
			float new_lower_y = parent_height - (float)family_size * prefab_height;

			player_family_content_panel.GetComponent<RectTransform> ().offsetMin = new Vector2 (current_lower_x, new_lower_y);
		}

		// Only display parents and children
		foreach (Parent parent_instance in PlayerFamily.Parents) 
		{
			Parent parent = parent_instance;

			MakePanel (parent, player_family_button_list, player_family_content_panel);

			SetParentButton (parent, player_family_button_list, player_offer_content_panel, player_family_content_panel, player_family_button_list.Count - 1, true);
		}

		foreach (Child child_instance in PlayerFamily.Children) 
		{
			Child child = child_instance;

			MakePanel (child, player_family_button_list, player_family_content_panel);

			SetChildButton (child, player_family_button_list, player_offer_content_panel, player_family_content_panel, player_family_button_list.Count - 1, true);
		}

		// Display money
		offer_money_field.transform.Find ("Placeholder").GetComponent<Text> ().text = "Currently Held: $" + PlayerFamily.Grandpa.Money.ToString();
	}

	public void DisplayEnemyFamilies ()
	{
		ClearOffers ();

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

		foreach (Family family_instance in leagueFamilies) 
		{
			int current_panel_ind = enemy_family_button_list.Count;
			Family family = family_instance;

			MakeFamiliesPanel (family, enemy_family_button_list, enemy_families_content_panel);

			// Add subfamily display activation to button
			enemy_family_button_list[enemy_family_button_list.Count - 1].GetComponent<Button>().onClick.AddListener(() => 
				{
					// Enemy families
					foreach (GameObject button in enemy_family_button_list)
					{
						Destroy(button);
					}
					enemy_family_button_list.Clear();

					foreach (Parent parent_instance in family.Parents)
					{
						Parent parent = parent_instance;

						MakePanel (parent, enemy_family_subfamily_list, enemy_families_content_panel);

						SetParentButton (parent, enemy_family_subfamily_list, enemy_offer_content_panel, enemy_families_content_panel, enemy_family_subfamily_list.Count - 1, false);
					}

					foreach (Child child_instance in family.Children)
					{
						Child child = child_instance;

						MakePanel (child, enemy_family_subfamily_list, enemy_families_content_panel);

						SetChildButton (child, enemy_family_subfamily_list, enemy_offer_content_panel, enemy_families_content_panel, enemy_family_subfamily_list.Count - 1, false);
					}

					// Display money
					receive_money_field.transform.Find ("Placeholder").GetComponent<Text> ().text = "Currently Held: $" + family.Grandpa.Money.ToString();
				});
		}

		receive_money_field.transform.Find ("Placeholder").GetComponent<Text> ().text = "No family selected";
	}

	public void SendOffer()
	{
		int.TryParse (offer_money_field.transform.Find ("Offer Text").GetComponent<Text> ().text, out offer_money);
		int.TryParse (receive_money_field.transform.Find ("Receive Text").GetComponent<Text> ().text, out receive_money);

		Debug.Log ("Trade offer sent:");
		Debug.Log ("Your parents: ");
		foreach (Parent parent in offer_parents) 
		{
			Debug.Log (parent.Name);
		}
		Debug.Log ("Your children: ");
		foreach (Child child in offer_children) 
		{
			Debug.Log (child.Name);
		}
		Debug.Log ("With $" + offer_money);

		Debug.Log ("\nOffered for: ");
		Debug.Log ("Their parents: ");
		foreach (Parent parent in receive_parents) 
		{
			Debug.Log (parent.Name);
		}
		Debug.Log ("Their children: ");
		foreach (Child child in receive_children) 
		{
			Debug.Log (child.Name);
		}
		Debug.Log ("With $" + receive_money);

		ClearOffers ();
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

	private void MakeFamiliesPanel(Family family, List<GameObject> prefab_list, GameObject parent_panel)
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

	private void SetChildButton(Child child, List<GameObject> button_list, GameObject offer_panel, GameObject family_panel, int buttonInd, bool player)
	{
		// Add character display activation to button
		button_list[buttonInd].GetComponent<Button>().onClick.RemoveAllListeners();
		button_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
			{
				button_list[buttonInd].transform.SetParent(offer_panel.transform, false);

				if (player)
				{
					Debug.Log("Add player child to offer");
					offer_children.Add(child);
				}
				else // enemy
				{
					Debug.Log("Add enemy child to offer");
					receive_children.Add(child);
				}

				button_list[buttonInd].GetComponent<Button>().onClick.RemoveAllListeners();
				button_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
					{
						if (player)
						{
							Debug.Log("Remove player child from offer");
							offer_children.Remove(child);
						}
						else // enemy
						{
							Debug.Log("Remove enemy child from offer");
							receive_children.Remove(child);
						}

						button_list[buttonInd].transform.SetParent(family_panel.transform, false);
						SetChildButton(child, button_list, offer_panel, family_panel, buttonInd, player);
					});
			});
	}

	private void SetParentButton(Parent parent, List<GameObject> button_list, GameObject offer_panel, GameObject family_panel, int buttonInd, bool player)
	{
		// Add character display activation to button
		button_list[buttonInd].GetComponent<Button>().onClick.RemoveAllListeners();
		button_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
			{
				button_list[buttonInd].transform.SetParent(offer_panel.transform, false);

				if (player)
				{
					Debug.Log("Add player parent to offer");
					offer_parents.Add(parent);
				}
				else // enemy
				{
					Debug.Log("Add enemy parent to offer");
					receive_parents.Add(parent);
				}

				button_list[buttonInd].GetComponent<Button>().onClick.RemoveAllListeners();
				button_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
					{
						if (player)
						{
							Debug.Log("Remove player parent from offer");
							offer_parents.Remove(parent);
						}
						else // enemy
						{
							Debug.Log("Remove enemy parent from offer");
							receive_parents.Remove(parent);
						}

						button_list[buttonInd].transform.SetParent(family_panel.transform, false);
						SetParentButton(parent, button_list, offer_panel, family_panel, buttonInd, player);
					});
			});
	}

	private void ClearOffers()
	{
		offer_parents.Clear ();
		offer_children.Clear ();
		offer_money = 0;

		receive_parents.Clear ();
		receive_children.Clear ();
		receive_money = 0;

		offer_money_field.transform.Find ("Offer Text").GetComponent<Text> ().text = "";
	}
}
