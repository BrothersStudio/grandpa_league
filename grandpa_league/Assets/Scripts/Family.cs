using System.Collections.Generic;

public class Family
{
    private     string      m_familyName    = "";
    private     Grandpa     m_grandpa       = null;
    private     Parent      m_parent        = null;
    private     List<Child> m_children      = new List<Child>();

    public Family(bool random=false)
    {
        if (!random)
            return;

        this.m_grandpa = CharacterManager.GetRandomGrandpa();
        this.m_parent = CharacterManager.GetRandomParent();
        this.m_children = CharacterManager.GetRandomChildren(Constants.INITIAL_CHILDREN);
        this.m_familyName = this.m_grandpa.Name.Split(' ')[1];      //TODO: FIX THIS HACK
    }

    public Grandpa Grandpa
    {
        get { return this.m_grandpa; }
        set { this.m_grandpa = value; }
    }

    public Parent Parent
    {
        get { return this.m_parent; }
        set { this.m_parent = value; }
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

        this.m_parent.Intelligence *= (int)(1 + this.m_parent.IntelligenceGrowth);
        this.m_parent.Wealth *= (int)(1 + this.m_parent.WealthGrowth);

        foreach(Child child in this.m_children)
        {
            child.Intelligence *= (int)(1 + child.IntelligenceGrowth);
            child.Cuteness *= (int)(1 + child.CutenessGrowth);
            child.FineMotorSkills *= (int)(1 + child.FineMotorGrowth);
            child.GrossMotorSkills *= (int)(1 + child.GrossMotorGrowth);
            child.Disposition *= (int)(1 + child.DispositionGrowth);
        }
    }
}