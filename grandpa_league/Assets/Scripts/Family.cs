using System.Collections.Generic;
using System.Linq;

public class Family
{
    private     string        m_familyName    = "";
    private     Grandpa       m_grandpa       = null;
	private     List<Parent>  m_parents       = new List<Parent>();
    private     List<Child>   m_children      = new List<Child>();

	private     int           m_chemistry     = 0;

    public Family(bool random=false)
    {
        if (!random)
            return;

        this.m_grandpa = CharacterManager.GetRandomGrandpa();
		this.m_parents = CharacterManager.GetRandomParents(Constants.INITIAL_PARENTS);
        this.m_children = CharacterManager.GetRandomChildren(Constants.INITIAL_CHILDREN);
        this.m_familyName = this.m_grandpa.Name.Split(' ')[1];      //TODO: FIX THIS HACK
    }

	public string FamilyName
	{
		get { return this.m_familyName;  }
		set { this.m_familyName = value; }
	}

	public int Chemistry
	{
		get { return this.m_chemistry;  }
		set { this.m_chemistry = value; }
	}

    public Grandpa Grandpa
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

    public List<Character> GetAllCharacters()
    {
        List<Character> allChar = new List<Character>();
        allChar.AddRange(m_children.Cast<Character>());
        allChar.AddRange(m_parents.Cast<Character>());
        allChar.Add((Character)m_grandpa);
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
        this.m_grandpa.Insanity *= (1 + this.m_grandpa.InsanityGrowth);
        //this.m_grandpa.Insanity = this.m_grandpa.Insanity > 100 ? 100 : this.m_grandpa.Insanity;

        this.m_grandpa.Wisdom *= (1 + this.m_grandpa.WisdomGrowth);
        //this.m_grandpa.Wisdom = this.m_grandpa.Wisdom > 100 ? 100 : this.m_grandpa.Wisdom;

        this.m_grandpa.Money += (this.m_grandpa.MoneyGrowth);

		foreach (Parent parent in this.m_parents)
        {
			parent.Intelligence *= (1 + parent.IntelligenceGrowth);
            //parent.Intelligence = parent.Intelligence > 100 ? 100 : parent.Intelligence;

            parent.Popularity *= (1 + parent.PopularityGrowth);
            //parent.Popularity = parent.Popularity > 100 ? 100 : parent.Popularity;

            parent.Love *= (1 + parent.LoveGrowth);
            //parent.Love = parent.Love > 100 ? 100 : parent.Love;
        }

        foreach(Child child in this.m_children)
        {
            child.Intelligence *= (1 + child.IntelligenceGrowth);
            //child.Intelligence = child.Intelligence > 100 ? 100 : child.Intelligence;

            child.Cuteness *= (1 + child.CutenessGrowth);
            //child.Cuteness = child.Cuteness > 100 ? 100 : child.Cuteness;

            child.Artistry *= (1 + child.ArtistryGrowth);
            //child.Artistry = child.Artistry > 100 ? 100 : child.Artistry;

            child.Athleticism *= (1 + child.AthleticismGrowth);
            //child.Athleticism = child.Athleticism > 100 ? 100 : child.Athleticism;

            child.Popularity *= (1 + child.PopularityGrowth);
            //child.Popularity = child.Popularity > 100 ? 100 : child.Popularity;
        }
    }

	public int FamilySize
	{
		get {return this.m_parents.Count + this.m_children.Count + 1;}
	}
}