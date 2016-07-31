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
       
	private     double          m_chemistry     = 0;
    private     double          m_upkeep        = 0;

    private     int             m_wins          = 0;
    private     int             m_losses        = 0;
    private     int             m_draws         = 0;

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
            return eligible[Constants.RANDOM.Next(0, eligible.Count)];
    }

    public Parent GetRandomParent()
    {
        return this.m_parents[Constants.RANDOM.Next(0, this.m_parents.Count - 1)];
    }

    public void ApplyStatUpgrades()
    {
        this.m_grandpa[0].Insanity *= (1 + this.m_grandpa[0].InsanityGrowth);

        this.m_grandpa[0].Wisdom *= (1 + this.m_grandpa[0].WisdomGrowth);

        this.m_grandpa[0].Money += (this.m_grandpa[0].MoneyGrowth - this.m_upkeep);

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

    public double CalculateChemistry()
    {
        int familySize = this.m_children.Count + this.m_parents.Count + this.m_grandpa.Count;
        double totalLove = 0;
        foreach(Parent parent in this.m_parents)
        {
            totalLove += parent.Love;
        }

        double totalChildrenStats = 0;
        foreach(Child child in this.m_children)
        {
            totalChildrenStats += (child.Artistry + child.Athleticism + child.Cuteness + child.Intelligence + child.Popularity);
        }

        double grandpaInstanityFactor = 2 * (m_grandpa[0].Insanity - m_grandpa[0].Wisdom);

        this.m_chemistry = (totalChildrenStats + grandpaInstanityFactor) / (totalLove * familySize);
        return this.m_chemistry;
    }

    public double CalculateUpkeep()
    {
        double upkeep = 0;
        upkeep += Constants.Family.GRANDPA_UPKEEP;

        upkeep += (this.m_children.Count * Constants.Family.CHILD_UPKEEP);

        upkeep += (this.m_parents.Count * Constants.Family.PARENT_UPKEEP);

        this.m_upkeep = upkeep;
        return this.m_upkeep;
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

    public double Chemistry
    {
        get { return this.m_chemistry; }
        set { this.m_chemistry = value; }
    }

    public double Upkeep
    {
        get { return this.CalculateUpkeep(); }
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

    public int Wins
    {
        get { return this.m_wins; }
        set { this.m_wins = value; }
    }

    public int Losses
    {
        get { return this.m_losses; }
        set { this.m_losses = value; }
    }

    public int Draws
    {
        get { return this.m_draws; }
        set { this.m_draws = value; }
    }
}