using System.Collections.Generic;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadAbilitiesPanel : MonoBehaviour {

	public GameObject ability_button_prefab;
	private List<GameObject> ability_prefab_instance_list = new List<GameObject>();
	public GameObject ability_panel;

	public GameObject game_controller;
    private bool userInputting = false;

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
				"{1}\n" + 
                "{2} insanity ${3}",
				ability.Name, ability.CurrentCooldown == 0 ? "Available Now" : string.Format("{0} days until available\n",ability.CurrentCooldown.ToString()), ability.InsanityCost, ability.MoneyCost);


            new_button.GetComponent<Button>().onClick.RemoveAllListeners();
			new_button.GetComponent<Button>().onClick.AddListener (() => 
				{
                    StartCoroutine(EventInputPanel(game_controller, ability));
                });

			ability_prefab_instance_list.Add (new_button);
			ability_ind++;
		}
	}

    public IEnumerator EventInputPanel(GameObject control, Ability ability)
    {
        control.GetComponent<Main>().CreateAndDisplayInputPanel(ability.Event);

        userInputting = true;

        control.GetComponent<Main>().MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
        yield return StartCoroutine("WaitForUserConfirm");
        userInputting = false;
        control.GetComponent<Main>().MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

        Outcome eventOutcome = ability.Event.RunEvent(Main.GetDataManager());

        if (eventOutcome.Mail != null)
        {
            Main.GetDataManager().PlayerFamily.Mailbox.Insert(0, eventOutcome.Mail);
            control.GetComponent<Main>().DisplayContent("mail");

        }

        if (eventOutcome.OutcomeDescription != "")
        {
            control.GetComponent<Main>().CreateAndDisplayResultPanel(eventOutcome);
            userInputting = true;

            control.GetComponent<Main>().MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            yield return StartCoroutine("WaitForUserConfirm");
            control.GetComponent<Main>().MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    private IEnumerator WaitForUserConfirm()
    {
        while (userInputting)
        {
            yield return null;
        }
        yield break;
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
