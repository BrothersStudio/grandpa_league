using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class Family
{
    private     string          m_familyName    = "";
    private     List<Grandpa>   m_grandpa       = new List<Grandpa>();
	private     List<Parent>    m_parents       = new List<Parent>();
    private     List<Child>     m_children      = new List<Child>();
    private     List<Mail>      m_mailbox       = new List<Mail>();
       
	private     int           m_chemistry     = 0;

    public Family(bool random=false)
    {
        if (!random)
            return;

        this.m_grandpa.Add(CharacterManager.GetRandomGrandpa());
		this.m_parents = CharacterManager.GetRandomParents(Constants.INITIAL_PARENTS);
        this.m_children = CharacterManager.GetRandomChildren(Constants.INITIAL_CHILDREN);

        string[] splitName = this.m_grandpa[0].Name.Split(' ');
        if (splitName.Length >= 2)
            this.m_familyName = splitName[splitName.Length - 1];
        else
            this.m_familyName = Constants.Player.DEFAULT_SURNAME;
    }

    public List<Character> GetAllCharacters()
    {
        List<Character> allChar = new List<Character>();
        allChar.AddRange(m_children.Cast<Character>());
        allChar.AddRange(m_parents.Cast<Character>());
        allChar.AddRange(m_grandpa.Cast<Character>());
        return allChar;
    }

    public Child GetRandomEligibleChild(int minAge, int maxAge)
    {
        List<Child> eligible = new List<Child>();
        foreach (Child ch in this.m_children)
            if (ch.MeetsAgeRequirement(minAge, maxAge))
                eligible.Add(ch);
        if (eligible.Count == 0)
            return null;
        else
            return eligible[Constants.RANDOM.Next(0, eligible.Count - 1)];
    }

    public Parent GetRandomParent()
    {
        return this.m_parents[Constants.RANDOM.Next(0, this.m_parents.Count - 1)];
    }

    public void ApplyStatUpgrades()
    {
        this.m_grandpa[0].Insanity *= (1 + this.m_grandpa[0].InsanityGrowth);

        this.m_grandpa[0].Wisdom *= (1 + this.m_grandpa[0].WisdomGrowth);

        this.m_grandpa[0].Money += (this.m_grandpa[0].MoneyGrowth);

		foreach (Parent parent in this.m_parents)
        {
			parent.Intelligence *= (1 + parent.IntelligenceGrowth);
            parent.Popularity *= (1 + parent.PopularityGrowth);
            parent.Love *= (1 + parent.LoveGrowth);
            parent.UpdateValue();
        }

        foreach(Child child in this.m_children)
        {
            child.Intelligence *= (1 + child.IntelligenceGrowth);
            child.Cuteness *= (1 + child.CutenessGrowth);
            child.Artistry *= (1 + child.ArtistryGrowth);
            child.Athleticism *= (1 + child.AthleticismGrowth);
            child.Popularity *= (1 + child.PopularityGrowth);
            child.UpdateValue();
        }
    }

	public int FamilySize
	{
		get {return this.m_parents.Count + this.m_children.Count + 1;}
	}

    public string FamilyName
    {
        get { return this.m_familyName; }
        set { this.m_familyName = value; }
    }

    public int Chemistry
    {
        get { return this.m_chemistry; }
        set { this.m_chemistry = value; }
    }

    public Grandpa Grandpa
    {
        get { return this.m_grandpa[0]; }
        set { this.m_grandpa[0] = value; }
    }

    public List<Grandpa> GrandpaList
    {
        get { return this.m_grandpa; }
        set { this.m_grandpa = value; }
    }

    public List<Parent> Parents
    {
        get { return this.m_parents; }
        set { this.m_parents = value; }
    }

    public List<Child> Children
    {
        get { return this.m_children; }
        set { this.m_children = value; }
    }

    public List<Mail> Mailbox
    {
        get { return this.m_mailbox;  }
    }
}