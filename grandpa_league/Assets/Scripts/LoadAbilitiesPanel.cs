using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class LoadAbilitiesPanel : MonoBehaviour {

	public GameObject ability_button_prefab;
	private List<GameObject> ability_prefab_instance_list = new List<GameObject>();
	public GameObject ability_panel;

	public GameObject game_controller;

	public void DisplayAbilities(DataManager manager)
	{
		int ability_ind = 0;
		foreach (Ability ability_instance in manager.Abilities) 
		{
			Ability ability = ability_instance;

			GameObject new_button = Instantiate(ability_button_prefab) as GameObject;
			new_button.transform.SetParent(ability_panel.transform, false);

			// Move to correct location
			float width = new_button.GetComponent<RectTransform> ().rect.width;
			float current_x = new_button.GetComponent<RectTransform> ().anchoredPosition.x;
			float current_y = new_button.GetComponent<RectTransform> ().anchoredPosition.y;
			new_button.GetComponent<RectTransform> ().anchoredPosition = 
				new Vector2 (current_x  + (float)(ability_ind) * width, current_y);

			new_button.GetComponent<Button> ().image = Resources.Load <Image> (ability.Picture);
			new_button.GetComponentInChildren<Text> ().text = String.Format (
				"{0}\n" +
				"{1} days until available\n",
				ability.Name, ability.Cooldown.ToString());
			
			new_button.GetComponent<Button>().onClick.RemoveAllListeners();
			new_button.GetComponent<Button>().onClick.AddListener (() => 
				{
					game_controller.GetComponent<Main> ().CreateAndDisplayInputPanel (ability.Event);
				});

			ability_prefab_instance_list.Add (new_button);
			ability_ind++;
		}
	}

	public void DestroyButtons()
	{
		foreach (GameObject button in ability_prefab_instance_list) 
		{
			Destroy (button);
		}
		ability_prefab_instance_list.Clear ();
	}
}
