using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadFamily : MonoBehaviour 
{
	public GameObject parent_panel;
	public GameObject prefab_panel;

	public void DisplayFamily () 
	{
		int family_size = 5;

		float prefab_height = prefab_panel.GetComponent<RectTransform> ().rect.height;
		float parent_height = parent_panel.GetComponent<RectTransform> ().rect.height;

		float current_lower_x = parent_panel.GetComponent<RectTransform> ().offsetMin.x;
		float new_lower_y = (float)family_size * prefab_height - parent_height;

		parent_panel.GetComponent<RectTransform>().offsetMin = new Vector2(current_lower_x, new_lower_y);

		for (int i = 0; i < family_size; i++) 
		{
			GameObject member_panel = Instantiate(prefab_panel) as GameObject;
			member_panel.transform.SetParent(parent_panel.transform, false);

			float height = member_panel.GetComponent<RectTransform> ().rect.height;
			float current_x = member_panel.GetComponent<RectTransform> ().anchoredPosition.x;
			float current_y = member_panel.GetComponent<RectTransform> ().anchoredPosition.y;
			member_panel.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (current_x, current_y - (float)i * height);
		}
	}
}