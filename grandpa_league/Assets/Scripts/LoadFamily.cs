using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class LoadFamily : MonoBehaviour 
{
	public GameObject family_panel;
	public GameObject content_panel;
	public GameObject prefab_content_button;
	public GameObject character_stat_panel;

	private GameObject[] prefab_content_panel_instance;

	public void DisplayFamily ()
	{
		int family_size = 5;

		// Fit scroll panel to correct size
		prefab_content_panel_instance = new GameObject[family_size];

		float prefab_height = prefab_content_button.GetComponent<RectTransform> ().rect.height;
		float parent_height = content_panel.GetComponent<RectTransform> ().rect.height;

		float current_lower_x = content_panel.GetComponent<RectTransform> ().offsetMin.x;
		float new_lower_y = parent_height - (float)family_size * prefab_height;

		content_panel.GetComponent<RectTransform>().offsetMin = new Vector2(current_lower_x, new_lower_y);

		// Instantiate panels for family members
		for (int i = 0; i 	< family_size; i++) 
		{
			prefab_content_panel_instance[i] = Instantiate(prefab_content_button) as GameObject;
			prefab_content_panel_instance[i].transform.SetParent(content_panel.transform, false);

			// Move to correct location
			float height = prefab_content_panel_instance[i].GetComponent<RectTransform> ().rect.height;
			float current_x = prefab_content_panel_instance[i].GetComponent<RectTransform> ().anchoredPosition.x;
			float current_y = prefab_content_panel_instance[i].GetComponent<RectTransform> ().anchoredPosition.y;
			prefab_content_panel_instance[i].GetComponent<RectTransform> ().anchoredPosition = new Vector2 (current_x, current_y - (float)i * height);

			// Add character display image to button
			prefab_content_panel_instance[i].GetComponent<Button>().onClick.AddListener(() => 
				{
					character_stat_panel.SetActive (true);
					// Add specific stat text setting here
					//character_stat_panel.GetComponents<Text>()
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
}