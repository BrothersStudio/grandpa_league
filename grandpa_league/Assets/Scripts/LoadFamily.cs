using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadFamily : MonoBehaviour 
{
	public GameObject family_panel;
	public GameObject content_panel;
	public GameObject prefab_content_button;

	public GameObject grandpa_stat_panel;
	public GameObject parent_stat_panel;
	public GameObject child_stat_panel;

	private GameObject[] prefab_content_panel_instance;

	public void DisplayFamily (Family PlayerFamily)
	{
		int family_size = PlayerFamily.FamilySize;

		// Fit scroll panel to correct size
		float prefab_height = prefab_content_button.GetComponent<RectTransform> ().rect.height;
		float parent_height = content_panel.GetComponent<RectTransform> ().rect.height;

		float current_lower_x = content_panel.GetComponent<RectTransform> ().offsetMin.x;
		float new_lower_y = parent_height - (float)family_size * prefab_height;

		content_panel.GetComponent<RectTransform>().offsetMin = new Vector2(current_lower_x, new_lower_y);

		prefab_content_panel_instance = new GameObject[family_size];

		int panel_ind = 0;
		MakePanel (PlayerFamily.Grandpa, panel_ind);

		// Add character display activation to button
		prefab_content_panel_instance[panel_ind].GetComponent<Button>().onClick.AddListener(() => 
			{
				grandpa_stat_panel.SetActive (true);
				parent_stat_panel.SetActive (false);
				child_stat_panel.SetActive (false);

				grandpa_stat_panel.transform.Find("Name").GetComponent<Text>().text = PlayerFamily.Grandpa.Name;
				grandpa_stat_panel.transform.Find("Age").GetComponent<Text>().text = "Age: " + PlayerFamily.Grandpa.Age;
				grandpa_stat_panel.transform.Find("Insanity").GetComponent<Text>().text = "Insanity: " + PlayerFamily.Grandpa.Insanity;
				grandpa_stat_panel.transform.Find("Wisdom").GetComponent<Text>().text = "Wisdom: " + PlayerFamily.Grandpa.Wisdom;
				grandpa_stat_panel.transform.Find("Money").GetComponent<Text>().text = "Money: $" + PlayerFamily.Grandpa.Money;
				grandpa_stat_panel.transform.Find("Pride").GetComponent<Text>().text = "Pride: " + PlayerFamily.Grandpa.Pride;
			});

		foreach (Parent parent_instance in PlayerFamily.Parents) 
		{
			panel_ind++;
			Parent parent = parent_instance;

			MakePanel (parent, panel_ind);

			// Add character display activation to button
			prefab_content_panel_instance[panel_ind].GetComponent<Button>().onClick.AddListener(() => 
				{
					grandpa_stat_panel.SetActive (false);
					parent_stat_panel.SetActive (true);
					child_stat_panel.SetActive (false);

					parent_stat_panel.transform.Find("Name").GetComponent<Text>().text = parent.Name;
					parent_stat_panel.transform.Find("Age").GetComponent<Text>().text = "Age: " + parent.Age;
					parent_stat_panel.transform.Find("Popularity").GetComponent<Text>().text = "Popularity: " + parent.Popularity;
					parent_stat_panel.transform.Find("Intelligence").GetComponent<Text>().text = "Intelligence: " + parent.Intelligence;
					parent_stat_panel.transform.Find("Love").GetComponent<Text>().text = "Love: " + parent.Love;
				});
		}

		foreach (Child child_instance in PlayerFamily.Children) 
		{
			panel_ind++;
			Child child = child_instance;

			MakePanel (child, panel_ind);

			// Add character display activation to button
			prefab_content_panel_instance[panel_ind].GetComponent<Button>().onClick.AddListener(() => 
				{
					grandpa_stat_panel.SetActive (false);
					parent_stat_panel.SetActive (false);
					child_stat_panel.SetActive (true);

					child_stat_panel.transform.Find("Name").GetComponent<Text>().text = child.Name;
					child_stat_panel.transform.Find("Age").GetComponent<Text>().text = "Age: " + child.Age;
					child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().text = "Cuteness: " + child.Cuteness;
					child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().text = "Intelligence: " + child.Intelligence;
					child_stat_panel.transform.Find("Artistry").GetComponent<Text>().text = "Artistry: " + child.Artistry;
					child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().text = "Athleticism: " + child.Athleticism;
					child_stat_panel.transform.Find("Popularity").GetComponent<Text>().text = "Popularity: " + child.Popularity;
				});
		}
	}
		
	public void RemoveFamilyPanels ()
	{
		for (int i = 0; i < prefab_content_panel_instance.Length; i++)
		{
			Destroy (prefab_content_panel_instance [i]);
		}
		Array.Clear(prefab_content_panel_instance, 0, prefab_content_panel_instance.Length);
	}

	private void MakePanel<T>(T member, int panel_ind) where T : Character
	{
		prefab_content_panel_instance[panel_ind] = Instantiate(prefab_content_button) as GameObject;
		prefab_content_panel_instance[panel_ind].transform.SetParent(content_panel.transform, false);

		// Move to correct location
		float height = prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().rect.height;
		float current_x = prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition.x;
		float current_y = prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition.y;
		prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition = 
			new Vector2 (current_x, current_y - (float)panel_ind * height);

		// Set name
		prefab_content_panel_instance[panel_ind].GetComponentInChildren<Text>().text = member.Name;
	}
}