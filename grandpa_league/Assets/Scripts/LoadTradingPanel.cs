using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadTradingPanel : MonoBehaviour {

	public GameObject prefab_family_list_button;

	public GameObject player_family_content_panel;
	public GameObject enemy_families_content_panel;
	public GameObject player_offer_content_panel;
	public GameObject enemy_offer_content_panel;

	private int parents_in_player_panel;
	private int parents_in_enemy_panel;
	private int children_in_player_panel;
	private int children_in_enemy_panel;

	private List<GameObject> player_family_parent_button_list = new List<GameObject>();
	private List<GameObject> player_family_child_button_list = new List<GameObject>();
	private List<GameObject> enemy_family_button_list = new List<GameObject>();
	private List<GameObject> enemy_family_subfamily_parent_list = new List<GameObject>();
	private List<GameObject> enemy_family_subfamily_child_list = new List<GameObject>();

	public GameObject offer_money_field;
	public GameObject receive_money_field;

	private Family offer_family;
	private List<Parent> offer_parents = new List<Parent> ();
	private List<Child> offer_children = new List<Child> ();
	private int offer_money = 0;

	private Family receive_family;
	private List<Parent> receive_parents = new List<Parent> ();
	private List<Child> receive_children = new List<Child> ();
	private int receive_money = 0;

	private List<Family> leagueFamilies;

    public void Start()
    {
        offer_money_field.GetComponent<InputField>().onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateMoneyInput(input, addedChar); };
    }

    private char ValidateMoneyInput(string input, char addedChar)
    {
        int result;
        int.TryParse(input + addedChar, out result);
        if (result > Main.GetDataManager().PlayerFamily.Grandpa.Money)
        {
            offer_money_field.GetComponent<InputField>().textComponent.color = new Color(244, 0, 0);
            return '\0';
        }

        offer_money_field.GetComponent<InputField>().textComponent.color = new Color(233, 233, 233);
        return addedChar;
    }

    public void DisplayAllFamilies (Family PlayerFamily, List<Family> LeagueFamilies) 
	{
		offer_family = PlayerFamily;
		leagueFamilies = LeagueFamilies;

		DisplayPlayerFamily (PlayerFamily);
		DisplayEnemyFamilies ();
	}


	public void SendOffer()
	{
        if ((this.offer_children.Count == 0 && this.offer_parents.Count == 0) && (this.receive_children.Count == 0 && this.receive_parents.Count == 0) || this.receive_family == null)
        {
            Debug.Log("No children or parents found in offer for either side (empty offer) aborting...");
            return;
        }

        //int.TryParse (offer_money_field.transform.Find ("Offer Text").GetComponent<Text> ().text, out offer_money);
		//int.TryParse (receive_money_field.transform.Find ("Receive Text").GetComponent<Text> ().text, out receive_money);
        receive_money = receive_money_field.GetComponent<InputField>().text == "" ? 0 : Int32.Parse(receive_money_field.GetComponent<InputField>().text);
        offer_money = offer_money_field.GetComponent<InputField>().text == "" ? 0 : Int32.Parse(offer_money_field.GetComponent<InputField>().text);

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

		Trade thisTrade = new Trade ();

		thisTrade.SetOffer (
			offer_family, offer_children, offer_parents, offer_money,
			receive_family, receive_children, receive_parents, receive_money);

		thisTrade.ConfirmOffer ();

		ClearOffers ();
	}

	public void DestroyPanels ()
	{
		ClearOffers ();

		// Player Family
		foreach (GameObject button in player_family_parent_button_list)
		{
			Destroy(button);
		}
		player_family_parent_button_list.Clear();

		foreach (GameObject button in player_family_child_button_list)
		{
			Destroy(button);
		}
		player_family_child_button_list.Clear();

		// Enemy families
		foreach (GameObject button in enemy_family_button_list)
		{
			Destroy(button);
		}
		enemy_family_button_list.Clear();

		// Enemy sub families
		foreach (GameObject button in enemy_family_subfamily_parent_list)
		{
			Destroy(button);
		}
		enemy_family_subfamily_parent_list.Clear();

		foreach (GameObject button in enemy_family_subfamily_child_list)
		{
			Destroy(button);
		}
		enemy_family_subfamily_child_list.Clear();
	}

	private void DisplayPlayerFamily (Family PlayerFamily)
	{
		int family_size = PlayerFamily.FamilySize - 1;
		parents_in_player_panel = 0;
		children_in_player_panel = 0;

		// Fit scroll panel to family size if needed
		float prefab_height = prefab_family_list_button.GetComponent<RectTransform> ().rect.height;
		float parent_height = player_family_content_panel.GetComponent<RectTransform> ().rect.height;

		if (family_size * prefab_height > parent_height) 
		{
			float current_lower_x = player_family_content_panel.GetComponent<RectTransform> ().offsetMin.x;
			float new_lower_y = parent_height - (float)family_size * prefab_height;

			player_family_content_panel.GetComponent<RectTransform> ().offsetMin = new Vector2 (current_lower_x, new_lower_y);
		}

		// Only display parents and children
		int current_inds = 0;
		foreach (Parent parent_instance in PlayerFamily.Parents) 
		{
			Parent parent = parent_instance;

			parents_in_player_panel++;

			MakePanel (parent, player_family_parent_button_list, player_family_content_panel, current_inds, 1);

			SetParentButton (parent, player_family_parent_button_list, player_offer_content_panel, player_family_content_panel, player_family_parent_button_list.Count - 1, true);

			current_inds++;
		}
		if (PlayerFamily.Parents.Count == 1) 
		{
			player_family_parent_button_list [player_family_parent_button_list.Count - 1].GetComponent<Button> ().interactable = false;
		}
			
		foreach (Child child_instance in PlayerFamily.Children) 
		{
			Child child = child_instance;

            children_in_player_panel++;

			MakePanel (child, player_family_child_button_list, player_family_content_panel, current_inds, 2);

			SetChildButton (child, player_family_child_button_list, player_offer_content_panel, player_family_content_panel, player_family_child_button_list.Count - 1, true);

			current_inds++;
		}
		if (PlayerFamily.Children.Count == 1) 
		{
			player_family_child_button_list [player_family_child_button_list.Count - 1].GetComponent<Button> ().interactable = false;
		}
			
		offer_money_field.transform.Find ("Placeholder").GetComponent<Text> ().text = "Currently Held: $" + PlayerFamily.Grandpa.Money.ToString();
	}

	public void DisplayEnemyFamilies ()
	{
		ClearOffers ();
		parents_in_enemy_panel = 0;
		children_in_enemy_panel = 0;

		// Enemy families
		foreach (GameObject button in enemy_family_button_list)
		{
			Destroy(button);
		}
		enemy_family_button_list.Clear();

		// Enemy sub families
		foreach (GameObject button in enemy_family_subfamily_parent_list)
		{
			Destroy(button);
		}
		enemy_family_subfamily_parent_list.Clear();

		foreach (GameObject button in enemy_family_subfamily_child_list)
		{
			Destroy(button);
		}
		enemy_family_subfamily_child_list.Clear();

		foreach (Family family_instance in leagueFamilies) 
		{
			Family family = family_instance;

			MakeFamiliesPanel (family, enemy_family_button_list, enemy_families_content_panel);

			// Add subfamily display activation to button
			enemy_family_button_list[enemy_family_button_list.Count - 1].GetComponent<Button>().onClick.AddListener(() => 
				{
					receive_family = family;

					// Enemy families
					foreach (GameObject button in enemy_family_button_list)
					{
						Destroy(button);
					}
					enemy_family_button_list.Clear();

					int current_inds = 0;
					foreach (Parent parent_instance in family.Parents)
					{
						Parent parent = parent_instance;

						parents_in_enemy_panel++;

						MakePanel (parent, enemy_family_subfamily_parent_list, enemy_families_content_panel, current_inds, 1);

						SetParentButton (parent, enemy_family_subfamily_parent_list, enemy_offer_content_panel, enemy_families_content_panel, enemy_family_subfamily_parent_list.Count - 1, false);

						current_inds++;
					}
					if (family.Parents.Count == 1) 
					{
						enemy_family_subfamily_parent_list [enemy_family_subfamily_parent_list.Count - 1].GetComponent<Button> ().interactable = false;
					}

					foreach (Child child_instance in family.Children)
					{
						Child child = child_instance;

						children_in_enemy_panel++;

						MakePanel (child, enemy_family_subfamily_child_list, enemy_families_content_panel, current_inds, 2);

						SetChildButton (child, enemy_family_subfamily_child_list, enemy_offer_content_panel, enemy_families_content_panel, enemy_family_subfamily_child_list.Count - 1, false);

						current_inds++;
					}
					if (family.Children.Count == 1) 
					{
						enemy_family_subfamily_child_list [enemy_family_subfamily_child_list.Count - 1].GetComponent<Button> ().interactable = false;
					}

					// Display money
					receive_money_field.transform.Find ("Placeholder").GetComponent<Text> ().text = "Currently Held: $" + family.Grandpa.Money.ToString();
				});
		}

		receive_money_field.transform.Find ("Placeholder").GetComponent<Text> ().text = "No family selected";
	}

	private void MakePanel<T>(T member, List<GameObject> prefab_list, GameObject parent_panel, int button_inds, int color) where T : Character
	{
		
		GameObject new_button = Instantiate(prefab_family_list_button) as GameObject;
		new_button.transform.SetParent(parent_panel.transform, false);

		// Move to correct location
		float height = new_button.GetComponent<RectTransform> ().rect.height;
		float current_x = new_button.GetComponent<RectTransform> ().anchoredPosition.x;
		float current_y = new_button.GetComponent<RectTransform> ().anchoredPosition.y;
		new_button.GetComponent<RectTransform> ().anchoredPosition = 
			new Vector2 (current_x, current_y - (float)(button_inds) * height);

		// Set color
		Color button_color = new Color ();
		button_color.a = 1;
		switch (color) 
		{
		case 1:
			button_color.r = (float)(71.0/255.0);
			button_color.g = (float)(170.0/255.0);
			button_color.b = (float)(255.0/255.0);
			break;
		case 2:
			button_color.r = (float)(64.0/255.0);
			button_color.g = (float)(200.0/255.0);
			button_color.b = (float)(30.0/255.0);
			break;
		}
		new_button.GetComponent<Image> ().color = button_color;

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

	private void SetChildButton(Child child, List<GameObject> child_button_list, GameObject offer_panel, GameObject family_panel, int buttonInd, bool player)
	{
        // Add character display activation to button
        child_button_list[buttonInd].GetComponent<Button>().onClick.RemoveAllListeners();
        child_button_list[buttonInd].GetComponent<Button>().onClick.AddListener(() =>
			{
                child_button_list[buttonInd].transform.SetParent(offer_panel.transform, false);
				Vector2 newOffsetMax = child_button_list[buttonInd].GetComponent<RectTransform>().offsetMax;
				newOffsetMax[0] += 25;
				child_button_list[buttonInd].GetComponent<RectTransform>().offsetMax = newOffsetMax;

				if (player)
				{
					this.offer_children.Add(child);
					children_in_player_panel--;
				}
				else // enemy
				{
					this.receive_children.Add(child);
					children_in_enemy_panel--;
				}

				if (player && children_in_player_panel == 1)
				{
					foreach (GameObject button in child_button_list)
					{
						if (!button.transform.IsChildOf(offer_panel.transform))
						{
							button.GetComponent<Button>().interactable = false;
						}
					}
				}
				else if (!player && children_in_enemy_panel == 1)
				{
					foreach (GameObject button in child_button_list)
					{
						if (!button.transform.IsChildOf(offer_panel.transform))
						{
							button.GetComponent<Button>().interactable = false;
						}
					}
				}
					
				child_button_list[buttonInd].GetComponent<Button>().onClick.RemoveAllListeners();
				child_button_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
					{
						if (player)
						{
                            this.offer_children.Remove(child);
							children_in_player_panel++;
						}
						else // enemy
						{
                            this.receive_children.Remove(child);
							children_in_enemy_panel++;
						}

						foreach (GameObject button in child_button_list)
						{
							if (!button.transform.IsChildOf(offer_panel.transform))
							{
								button.GetComponent<Button>().interactable = true;
							}
						}

						child_button_list[buttonInd].transform.SetParent(family_panel.transform, false);
						Vector2 oldOffsetMax = child_button_list[buttonInd].GetComponent<RectTransform>().offsetMax;
						oldOffsetMax[0] -= 25;
						child_button_list[buttonInd].GetComponent<RectTransform>().offsetMax = oldOffsetMax;
						SetChildButton(child, child_button_list, offer_panel, family_panel, buttonInd, player);
					});
			});
	}

	private void SetParentButton(Parent parent, List<GameObject> parent_button_list, GameObject offer_panel, GameObject family_panel, int buttonInd, bool player)
	{
		// Add character display activation to button
		parent_button_list[buttonInd].GetComponent<Button>().onClick.RemoveAllListeners();
		parent_button_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
			{
				parent_button_list[buttonInd].transform.SetParent(offer_panel.transform, false);
				Vector2 newOffsetMax = parent_button_list[buttonInd].GetComponent<RectTransform>().offsetMax;
				newOffsetMax[0] += 25;
				parent_button_list[buttonInd].GetComponent<RectTransform>().offsetMax = newOffsetMax;

				if (player)
				{
					Debug.Log(String.Format("Add player parent {0} to offer", parent.Name));
                    this.offer_parents.Add(parent);
					parents_in_player_panel--;
				}
				else // enemy
				{
					Debug.Log("Add enemy parent to offer");
                    this.receive_parents.Add(parent);
					parents_in_enemy_panel--;
				}

				if (player && parents_in_player_panel == 1)
				{
					foreach (GameObject button in parent_button_list)
					{
						if (!button.transform.IsChildOf(offer_panel.transform))
						{
							button.GetComponent<Button>().interactable = false;
						}
					}
				}
				else if (!player && parents_in_enemy_panel == 1)
				{
					foreach (GameObject button in parent_button_list)
					{
						if (!button.transform.IsChildOf(offer_panel.transform))
						{
							button.GetComponent<Button>().interactable = false;
						}
					}
				}

				parent_button_list[buttonInd].GetComponent<Button>().onClick.RemoveAllListeners();
				parent_button_list[buttonInd].GetComponent<Button>().onClick.AddListener(() => 
					{
						if (player)
						{
							Debug.Log("Remove player parent from offer");
                            this.offer_parents.Remove(parent);
							parents_in_player_panel++;
						}
						else // enemy
						{
							Debug.Log("Remove enemy parent from offer");
                            this.receive_parents.Remove(parent);
							parents_in_enemy_panel++;
						}

						foreach (GameObject button in parent_button_list)
						{
							if (!button.transform.IsChildOf(offer_panel.transform))
							{
								button.GetComponent<Button>().interactable = true;
							}
						}

						parent_button_list[buttonInd].transform.SetParent(family_panel.transform, false);
						Vector2 oldOffsetMax = parent_button_list[buttonInd].GetComponent<RectTransform>().offsetMax;
						oldOffsetMax[0] -= 25;
						parent_button_list[buttonInd].GetComponent<RectTransform>().offsetMax = oldOffsetMax;
						SetParentButton(parent, parent_button_list, offer_panel, family_panel, buttonInd, player);
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
