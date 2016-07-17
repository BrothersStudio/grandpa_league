using System.Collections.Generic;

public class Family
{
    private     string        m_familyName    = "";
    private     Grandpa       m_grandpa       = null;
	private     List<Parent>  m_parents        = new List<Parent>();
    private     List<Child>   m_children      = new List<Child>();

    public Family(bool random=false)
    {
        if (!random)
            return;

        this.m_grandpa = CharacterManager.GetRandomGrandpa();
		this.m_parents = CharacterManager.GetRandomParents(Constants.INITIAL_PARENTS);
        this.m_children = CharacterManager.GetRandomChildren(Constants.INITIAL_CHILDREN);
        this.m_familyName = this.m_grandpa.Name.Split(' ')[1];      //TODO: FIX THIS HACK
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

    public void ApplyStatUpgrades()
    {
        this.m_grandpa.Insanity *= (int)(1 + this.m_grandpa.InsanityGrowth);
        this.m_grandpa.Wisdom *= (int)(1 + this.m_grandpa.WisdomGrowth);

		foreach (Parent parent in this.m_parents) {
			parent.Intelligence *= (int)(1 + parent.IntelligenceGrowth);
			parent.Wealth *= (int)(1 + parent.WealthGrowth);
		}

        foreach(Child child in this.m_children)
        {
            child.Intelligence *= (int)(1 + child.IntelligenceGrowth);
            child.Cuteness *= (int)(1 + child.CutenessGrowth);
            child.FineMotorSkills *= (int)(1 + child.FineMotorGrowth);
            child.GrossMotorSkills *= (int)(1 + child.GrossMotorGrowth);
            child.Disposition *= (int)(1 + child.DispositionGrowth);
        }
    }

	public int FamilySize
	{
		get {return this.m_parents.Count + this.m_children.Count + 1;}
	}

	/*
	public List<string> FamilyFirstNames
	{
		get 
		{
			List<string> returnList = new List<string>();

			returnList.Add (this.m_grandpa.Name);
			foreach (Parent parent in this.m_parents)
			{
				returnList.Add (parent.Name);
			}
			foreach (Child child in this.m_children)
			{
				returnList.Add (child.Name);
			}
			return returnList;
		}
	}*/
}