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

	public void DisplayAbilities(DataManager manager)
	{
		
		int ability_ind = 1;
		foreach (Ability ability_instance in manager.Abilities) 
		{
			Ability ability = ability_instance;

			ability_panel.transform.Find("Ability_Button " + ability_ind.ToString()).GetComponent<Image>().sprite = Resources.Load <Sprite> ("Ability_icons/" + ability.Picture);
			ability_panel.transform.Find("Ability_Button " + ability_ind.ToString()).GetComponentInChildren<Text>().text = ability.Name;
			ability_panel.transform.Find ("Cooldown " + ability_ind.ToString ()).GetComponent<Text> ().text = ability.CurrentCooldown == 0 ? "Available Now" : string.Format ("{0} days until available\n", ability.CurrentCooldown.ToString ());
			ability_panel.transform.Find ("Insanity Cost " + ability_ind.ToString ()).GetComponent<Text> ().text = "Insanity Cost: " + ability.InsanityCost.ToString ();
			ability_panel.transform.Find ("Money Cost " + ability_ind.ToString ()).GetComponent<Text> ().text = "Money Cost: " + ability.MoneyCost.ToString ();

			Button new_button = ability_panel.transform.Find ("Ability_Button " + ability_ind.ToString ()).GetComponent<Button> ();
            if (ability.CurrentCooldown != 0)
                new_button.GetComponent<Button>().interactable = false;
            new_button.GetComponent<Button>().onClick.RemoveAllListeners();
			new_button.GetComponent<Button>().onClick.AddListener (() => 
				{
                    StartCoroutine(EventInputPanel(game_controller, ability));
                });
					
			ability_ind++;
		}
	}

    public IEnumerator EventInputPanel(GameObject control, Ability ability)
    {
        control.GetComponent<Main>().CreateAndDisplayInputPanel(ability.Event);
        Globals.UserInputting = true;

        control.GetComponent<Main>().MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
        yield return StartCoroutine("WaitForUserConfirm");
        Globals.UserInputting = false;
        control.GetComponent<Main>().MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

        if(ability.Event.Requirements.Accept == false)
        {
            //DestroyButtons();
            ability_panel.SetActive(false);
            yield return true;
        }

        Outcome eventOutcome;
        if (Main.GetDataManager().PlayerFamily.Grandpa.Money - ability.MoneyCost < 0)
        {
            eventOutcome = new Outcome((int)Enums.EventOutcome.FAILURE, "Hey pal, you don't have the cash...");
        }
        else if (Main.GetDataManager().PlayerFamily.Grandpa.Insanity + ability.InsanityCost > 100)
        {
            eventOutcome = new Outcome((int)Enums.EventOutcome.FAILURE, "Woah pal, you don't seem all here...Come back when you're in the right state of mind..");
        }
        else
        {
            eventOutcome = ability.Event.RunEvent(Main.GetDataManager());
            if(eventOutcome.Status == (int)Enums.EventOutcome.SUCCESS)
            {
                Main.GetDataManager().PlayerFamily.Grandpa.Money -= ability.MoneyCost;
                Main.GetDataManager().PlayerFamily.Grandpa.Insanity += ability.InsanityCost;
            }
        }

        if (eventOutcome.Mail != null)
        {
            Main.GetDataManager().PlayerFamily.Mailbox.Insert(0, eventOutcome.Mail);
            control.GetComponent<Main>().DisplayContent("mail");

        }

        if (eventOutcome.OutcomeDescription != "")
        {
            control.GetComponent<Main>().CreateAndDisplayResultPanel(eventOutcome);
            Globals.UserInputting = true;

            control.GetComponent<Main>().MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            yield return StartCoroutine("WaitForUserConfirm");
            Globals.UserInputting = false;
            control.GetComponent<Main>().MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }


        if(eventOutcome.Status == (int)Enums.EventOutcome.SUCCESS)
        {
            ability.CurrentCooldown = ability.MaxCooldown;
            Main.GetDataManager().PlayerFamily.Grandpa.Insanity += ability.InsanityCost;
            Main.GetDataManager().PlayerFamily.Grandpa.Money -= ability.MoneyCost;
        }

        //DestroyButtons();
        ability_panel.SetActive(false);
    }

    private IEnumerator WaitForUserConfirm()
    {
        while (Globals.UserInputting)
        {
            yield return null;
        }
        yield break;
    }

	/*
    public void DestroyButtons()
	{
		foreach (GameObject button in ability_prefab_instance_list) 
		{
			Destroy (button);
		}
		ability_prefab_instance_list.Clear ();
	}*/
}
