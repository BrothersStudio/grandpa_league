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

		MakePanel (PlayerFamily.Children[0]);
		//MakePanel (PlayerFamily.Grandpa);
		//MakePanel (PlayerFamily.Parents[1]);

		// Instantiate panel for grandpa
		/*
		int panel_ind = 0;
		prefab_content_panel_instance[panel_ind] = Instantiate(prefab_content_button) as GameObject;
		prefab_content_panel_instance[panel_ind].transform.SetParent(content_panel.transform, false);

		// Move to correct location
		float height = prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().rect.height;
		float current_x = prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition.x;
		float current_y = prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition.y;
		prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition = 
			new Vector2 (current_x, current_y - (float)panel_ind * height);

		// Set name
		prefab_content_panel_instance[panel_ind].GetComponentInChildren<Text>().text = PlayerFamily.Grandpa.Name;

		// Add character display activation to button
		prefab_content_panel_instance[panel_ind].GetComponent<Button>().onClick.AddListener(() => 
			{
				grandpa_stat_panel.SetActive (true);
				// Add specific stat text setting here
				//character_stat_panel.GetComponents<Text>()
			});
		
		foreach (Parent parent in PlayerFamily.Parents) 
		{
			
		}*/
	}
		
	public void RemoveFamilyPanels ()
	{
		for (int i = 0; i < prefab_content_panel_instance.Length; i++)
		{
			Destroy (prefab_content_panel_instance [i]);
		}
		Array.Clear(prefab_content_panel_instance, 0, prefab_content_panel_instance.Length);
	}

	private void MakePanel<T>(T member)
	{
		Type listType = typeof(T);
		if (listType == typeof(Grandpa)) 
		{
			Debug.Log ("Found Grandpa!");
		} 
		else if (listType == typeof(Parent)) 
		{
			Debug.Log ("Found Parent!");
		} 
		else if (listType == typeof(Child)) 
		{
			Debug.Log ("Found Child!");
		} 
		else {
			Debug.LogError ("No type found in family panel creation. Something is really wrong.");
		}
	}
}