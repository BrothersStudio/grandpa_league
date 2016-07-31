using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadFamilyPanel : MonoBehaviour 
{
	public GameObject family_panel;
	public GameObject content_panel;
	public GameObject prefab_content_button;

    public GameObject[] qualification_panel;
    public Sprite[] qual_sprites;

	public GameObject grandpa_stat_panel;
	public GameObject parent_stat_panel;
	public GameObject child_stat_panel;

	public GameObject MainCanvas;

	public Sprite[] insanity_sprites;
	public Sprite[] stat_sprites;

    public GameObject chemistryLabel;
    public GameObject upkeepLabel;
    public GameObject questionLabel;

	private GameObject[] prefab_content_panel_instance;

	private bool isFromEvent = false;

	public void DisplayFamily (Family inputFamily, bool playerFamily, bool fromEvent = false)
	{
		isFromEvent = fromEvent;
		if (!isFromEvent)
			MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;

		int family_size = inputFamily.FamilySize;

		string family_name = inputFamily.FamilyName;

		// Fit scroll panel to correct size
		float prefab_height = prefab_content_button.GetComponent<RectTransform> ().rect.height;
		float parent_height = content_panel.GetComponent<RectTransform> ().rect.height;

		if (family_size * prefab_height > parent_height) 
		{
			float current_lower_x = content_panel.GetComponent<RectTransform> ().offsetMin.x;
			float new_lower_y = parent_height - (float)family_size * prefab_height;

			content_panel.GetComponent<RectTransform> ().offsetMin = new Vector2 (current_lower_x, new_lower_y);
		}

		prefab_content_panel_instance = new GameObject[family_size];

		int panel_ind = 0;
		MakePanel (inputFamily.Grandpa, panel_ind, 0);
		DisplayGrandpaPanel (inputFamily);

        if(playerFamily)
        {
            questionLabel.SetActive(true);
            chemistryLabel.SetActive(true);
            upkeepLabel.SetActive(true);
            chemistryLabel.GetComponent<Text>().text = string.Format("Chemistry: {0:0.00}",inputFamily.Chemistry);
            upkeepLabel.GetComponent<Text>().text = "Upkeep: " + inputFamily.Upkeep;
        }
        else
        {
            questionLabel.SetActive(false);
            chemistryLabel.SetActive(false);
            upkeepLabel.SetActive(false);
        }

		// Add character display activation to button
		prefab_content_panel_instance[panel_ind].GetComponent<Button>().onClick.AddListener(() => 
			{
				DisplayGrandpaPanel (inputFamily);
			});

		foreach (Parent parent_instance in inputFamily.Parents) 
		{
			panel_ind++;
			Parent parent = parent_instance;

			MakePanel (parent, panel_ind, 1);

			// Add character display activation to button
			prefab_content_panel_instance[panel_ind].GetComponent<Button>().onClick.AddListener(() => 
				{
                    parent_stat_panel.transform.Find("PopularityFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
                    parent_stat_panel.transform.Find("IntelligenceFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
                    parent_stat_panel.transform.Find("LoveFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();

                    grandpa_stat_panel.SetActive (false);
					parent_stat_panel.SetActive (true);
					child_stat_panel.SetActive (false);

                    for (int i = 0; i < qualification_panel.Length; i++)
                    {
                        qualification_panel[i].GetComponent<Image>().sprite = null;
                        qualification_panel[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                        qualification_panel[i].GetComponent<Collider2D>().enabled = false;
                    }
                    List<int> visibleQuals = parent.GetVisibleQualifications();
                    int num_quals = visibleQuals.Count > 6 ? 6 : visibleQuals.Count;
                    for (int i = 0; i < num_quals; i++)
                    {
                        qualification_panel[i].GetComponent<Image>().sprite = GetSpriteForQual(visibleQuals[i]);
                        qualification_panel[i].GetComponent<Image>().color = new Color(0, 0, 0, 1);
                        qualification_panel[i].GetComponent<QualificationToolTip>().SetToolTipText(Qualification.GetDisplayName(visibleQuals[i]));
                        qualification_panel[i].GetComponent<Collider2D>().enabled = true;
                    }

                    parent_stat_panel.transform.Find("Name").GetComponent<Text>().text = parent.Name + " " + family_name;
					parent_stat_panel.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load <Sprite> ("Parent_Sprites/" + parent.SpriteName);
					parent_stat_panel.transform.Find("Age").GetComponent<Text>().text = "Age: " + parent.Age;
					parent_stat_panel.transform.Find("Popularity Bar").GetComponent<Image>().sprite = ReturnSpriteForStat(parent.Popularity);
					parent_stat_panel.transform.Find("Intelligence Bar").GetComponent<Image>().sprite = ReturnSpriteForStat(parent.Intelligence);
					parent_stat_panel.transform.Find("Love Bar").GetComponent<Image>().sprite = ReturnSpriteForStat(parent.Love);

                    if (playerFamily)
                        AddParentFocusButtons(parent);
                    else
                        ResetParentFocusButtons(parent);
                });
		}

		foreach (Child child_instance in inputFamily.Children) 
		{
			panel_ind++;
			Child child = child_instance;

			MakePanel (child, panel_ind, 2);

			// Add character display activation to button
			prefab_content_panel_instance[panel_ind].GetComponent<Button>().onClick.AddListener(() => 
				{
                    child_stat_panel.transform.Find("PopularityFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
                    child_stat_panel.transform.Find("IntelligenceFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
                    child_stat_panel.transform.Find("CutenessFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
                    child_stat_panel.transform.Find("AthleticismFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
                    child_stat_panel.transform.Find("ArtistryFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();


                    grandpa_stat_panel.SetActive (false);
					parent_stat_panel.SetActive (false);
					child_stat_panel.SetActive (true);

                    for (int i = 0; i < qualification_panel.Length; i++)
                    {
                        qualification_panel[i].GetComponent<Image>().sprite = null;
                        qualification_panel[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
                        qualification_panel[i].GetComponent<Collider2D>().enabled = false;
                    }
                    List<int> visibleQuals = child.GetVisibleQualifications();
                    int num_quals = visibleQuals.Count > 6 ? 6 : visibleQuals.Count;
                    for (int i = 0; i < num_quals; i++)
                    {
                        qualification_panel[i].GetComponent<Image>().sprite = GetSpriteForQual(visibleQuals[i]);
                        qualification_panel[i].GetComponent<Image>().color = new Color(Constants.RANDOM.Next(1,255)/255, Constants.RANDOM.Next(1, 255) / 255, Constants.RANDOM.Next(1, 255) / 255, 1);
                        qualification_panel[i].GetComponent<QualificationToolTip>().SetToolTipText(Qualification.GetDisplayName(visibleQuals[i]));
                        qualification_panel[i].GetComponent<Collider2D>().enabled = true;
                    }

					child_stat_panel.transform.Find("Name").GetComponent<Text>().text = child.Name + " " + family_name;
					child_stat_panel.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load <Sprite> ("Child_Sprites/" + child.SpriteName);
					child_stat_panel.transform.Find("Age").GetComponent<Text>().text = "Age: " + child.Age;
					child_stat_panel.transform.Find("Cuteness Bar").GetComponent<Image>().sprite = ReturnSpriteForStat(child.Cuteness);
					child_stat_panel.transform.Find("Intelligence Bar").GetComponent<Image>().sprite = ReturnSpriteForStat(child.Intelligence);
					child_stat_panel.transform.Find("Artistry Bar").GetComponent<Image>().sprite = ReturnSpriteForStat(child.Artistry);
					child_stat_panel.transform.Find("Athleticism Bar").GetComponent<Image>().sprite = ReturnSpriteForStat(child.Athleticism);
					child_stat_panel.transform.Find("Popularity Bar").GetComponent<Image>().sprite = ReturnSpriteForStat(child.Popularity);

                    if (playerFamily)
                        AddChildFocusButtons(child);
                    else
                        ResetChildFocusButtons(child);
				});
		}

	}
		
	public void RemoveFamilyPanels ()
	{
		if (!isFromEvent)
			MainCanvas.GetComponent<CanvasGroup>().blocksRaycasts = true;

		for (int i = 0; i < prefab_content_panel_instance.Length; i++)
		{
			Destroy (prefab_content_panel_instance [i]);
		}
		Array.Clear(prefab_content_panel_instance, 0, prefab_content_panel_instance.Length);
	}

	private void DisplayGrandpaPanel(Family inputFamily)
	{
        grandpa_stat_panel.SetActive (true);
        parent_stat_panel.SetActive (false);
        child_stat_panel.SetActive (false);

        for (int i = 0; i < qualification_panel.Length; i++)
        {
            qualification_panel[i].GetComponent<Image>().sprite = null;
            qualification_panel[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
            qualification_panel[i].GetComponent<Collider2D>().enabled = false;
        }
        List<int> visibleQuals = inputFamily.Grandpa.GetVisibleQualifications();
        int num_quals = visibleQuals.Count > 6 ? 6 : visibleQuals.Count;
        for (int i = 0; i < num_quals; i++)
        {
            qualification_panel[i].GetComponent<Image>().sprite = GetSpriteForQual(visibleQuals[i]);
            qualification_panel[i].GetComponent<Image>().color = new Color(0, 0, 0, 1);
            qualification_panel[i].GetComponent<QualificationToolTip>().SetToolTipText(Qualification.GetDisplayName(visibleQuals[i]));
            qualification_panel[i].GetComponent<Collider2D>().enabled = true;
        }

		grandpa_stat_panel.transform.Find("Name").GetComponent<Text>().text = inputFamily.Grandpa.Name;
		grandpa_stat_panel.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load <Sprite> ("Grandpa_Sprites/" + inputFamily.Grandpa.SpriteName);
		grandpa_stat_panel.transform.Find("Insanity Bar").GetComponent<Image>().sprite = ReturnSpriteForStat(inputFamily.Grandpa.Insanity, true);
		grandpa_stat_panel.transform.Find("Wisdom Bar").GetComponent<Image>().sprite = ReturnSpriteForStat(inputFamily.Grandpa.Wisdom);
		grandpa_stat_panel.transform.Find("Money").GetComponent<Text>().text = "Money:\n$" + inputFamily.Grandpa.Money;
		grandpa_stat_panel.transform.Find("Pride").GetComponent<Text>().text = "Pride:\n" + inputFamily.Grandpa.Pride;
	}

	private void MakePanel<T>(T member, int panel_ind, int color) where T : Character
	{
		prefab_content_panel_instance[panel_ind] = Instantiate(prefab_content_button) as GameObject;
		prefab_content_panel_instance[panel_ind].transform.SetParent(content_panel.transform, false);

		// Move to correct location
		float height = prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().rect.height;
		float current_x = prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition.x;
		float current_y = prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition.y;
		prefab_content_panel_instance[panel_ind].GetComponent<RectTransform> ().anchoredPosition = 
			new Vector2 (current_x, current_y - (float)panel_ind * height);

		// Set color
		Color button_color = new Color ();
		button_color.a = 1;
		switch (color) 
		{
		case 0:
			button_color.r = (float)(244.0/255.0);
			button_color.g = (float)(45.0/255.0);
			button_color.b = (float)(39.0/255.0);
			break;
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
		prefab_content_panel_instance [panel_ind].GetComponent<Image> ().color = button_color;

		// Set name
		prefab_content_panel_instance[panel_ind].GetComponentInChildren<Text>().text = member.Name;
	}
		
    private Sprite GetSpriteForQual(int qual)
    {
        return qual_sprites[0];             //TODO DIFFERENT SPRITES FOR QUALS
    }

	private Sprite ReturnSpriteForStat(double stat, bool insanity = false)
	{
		if (stat > 100)
			stat = 100;
		else if (stat < 0)
			stat = 0;
		
		if (insanity)
			return insanity_sprites [(int)Math.Round (stat / 10)];
		else
			return stat_sprites [(int)Math.Round (stat / 10)];
	}

    private void AddParentFocusButtons(Parent par)
    {
        Parent parent = par;

        if (parent.IntelligenceStat.GrowthBonus)
        {
            parent_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.yellow;
            parent_stat_panel.transform.Find("Love").GetComponent<Text>().color = Color.white;
            parent_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
        }
        else if (parent.PopularityStat.GrowthBonus)
        {
            parent_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            parent_stat_panel.transform.Find("Love").GetComponent<Text>().color = Color.white;
            parent_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.yellow;
        }
        else if (parent.LoveStat.GrowthBonus)
        {
            parent_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            parent_stat_panel.transform.Find("Love").GetComponent<Text>().color = Color.yellow;
            parent_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
        }
        else
        {
            parent_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            parent_stat_panel.transform.Find("Love").GetComponent<Text>().color = Color.white;
            parent_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
        }


        parent_stat_panel.transform.Find("IntelligenceFocusButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            parent.IntelligenceStat.GrowthBonus = true;
            parent.PopularityStat.GrowthBonus = false;
            parent.LoveStat.GrowthBonus = false;

            parent_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.yellow;
            parent_stat_panel.transform.Find("Love").GetComponent<Text>().color = Color.white;
            parent_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
        });

        parent_stat_panel.transform.Find("PopularityFocusButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            parent.IntelligenceStat.GrowthBonus = false;
            parent.PopularityStat.GrowthBonus = true;
            parent.LoveStat.GrowthBonus = false;

            parent_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            parent_stat_panel.transform.Find("Love").GetComponent<Text>().color = Color.white;
            parent_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.yellow;
        });

        parent_stat_panel.transform.Find("LoveFocusButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            parent.IntelligenceStat.GrowthBonus = false;
            parent.PopularityStat.GrowthBonus = false;
            parent.LoveStat.GrowthBonus = true;

            parent_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            parent_stat_panel.transform.Find("Love").GetComponent<Text>().color = Color.yellow;
            parent_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
        });
    }

    public void ResetParentFocusButtons(Parent par)
    {
        Parent parent = par;
        parent_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
        parent_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
        parent_stat_panel.transform.Find("Love").GetComponent<Text>().color = Color.white;

        parent_stat_panel.transform.Find("IntelligenceFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
        parent_stat_panel.transform.Find("LoveFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
        parent_stat_panel.transform.Find("PopularityFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
    }

    private void AddChildFocusButtons(Child ch)
    {
        Child child = ch;

        if (child.IntelligenceStat.GrowthBonus)
        {
            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.yellow;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.white;
        }
        else if (child.PopularityStat.GrowthBonus)
        {
            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.yellow;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.white;
        }
        else if (child.CutenessStat.GrowthBonus)
        {
            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.yellow;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.white;
        }
        else if (child.ArtistryStat.GrowthBonus)
        {
            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.yellow;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.white;
        }
        else if (child.AthleticismStat.GrowthBonus)
        {
            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.yellow;
        }
        else
        {
            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.white;
        }


        child_stat_panel.transform.Find("IntelligenceFocusButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            child.IntelligenceStat.GrowthBonus = true;
            child.PopularityStat.GrowthBonus = false;
            child.CutenessStat.GrowthBonus = false;
            child.ArtistryStat.GrowthBonus = false;
            child.AthleticismStat.GrowthBonus = false;


            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.yellow;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.white;
        });

        child_stat_panel.transform.Find("PopularityFocusButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            child.IntelligenceStat.GrowthBonus = false;
            child.PopularityStat.GrowthBonus = true;
            child.CutenessStat.GrowthBonus = false;
            child.ArtistryStat.GrowthBonus = false;
            child.AthleticismStat.GrowthBonus = false;

            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.yellow;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.white;
        });

        child_stat_panel.transform.Find("CutenessFocusButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            child.IntelligenceStat.GrowthBonus = false;
            child.PopularityStat.GrowthBonus = false;
            child.CutenessStat.GrowthBonus = true;
            child.ArtistryStat.GrowthBonus = false;
            child.AthleticismStat.GrowthBonus = false;

            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.yellow;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.white;
        });

        child_stat_panel.transform.Find("ArtistryFocusButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            child.IntelligenceStat.GrowthBonus = false;
            child.PopularityStat.GrowthBonus = false;
            child.CutenessStat.GrowthBonus = false;
            child.ArtistryStat.GrowthBonus = true;
            child.AthleticismStat.GrowthBonus = false;

            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.yellow;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.white;
        });

        child_stat_panel.transform.Find("AthleticismFocusButton").GetComponent<Button>().onClick.AddListener(() =>
        {
            child.IntelligenceStat.GrowthBonus = false;
            child.PopularityStat.GrowthBonus = false;
            child.CutenessStat.GrowthBonus = false;
            child.ArtistryStat.GrowthBonus = false;
            child.AthleticismStat.GrowthBonus = true;

            child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.white;
            child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.yellow;
        });
    }

    public void ResetChildFocusButtons(Child ch)
    {
        Child child = ch;
        child_stat_panel.transform.Find("Intelligence").GetComponent<Text>().color = Color.white;
        child_stat_panel.transform.Find("Popularity").GetComponent<Text>().color = Color.white;
        child_stat_panel.transform.Find("Cuteness").GetComponent<Text>().color = Color.white;
        child_stat_panel.transform.Find("Artistry").GetComponent<Text>().color = Color.white;
        child_stat_panel.transform.Find("Athleticism").GetComponent<Text>().color = Color.white;

        child_stat_panel.transform.Find("IntelligenceFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
        child_stat_panel.transform.Find("AthleticismFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
        child_stat_panel.transform.Find("CutenessFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
        child_stat_panel.transform.Find("ArtistryFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
        child_stat_panel.transform.Find("PopularityFocusButton").GetComponent<Button>().onClick.RemoveAllListeners();
    }
}