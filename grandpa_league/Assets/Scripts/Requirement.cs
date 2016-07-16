public class Requirement
{
    //set by what is read in from events.xml
    private bool m_childNeeded = false;
    private bool m_parentNeeded = false;
    private bool m_grandpaNeeded = false;
    private bool m_moneyNeeded = false;
    private bool m_acceptRejectNeeded = false;

    //set by user's choices
    private Child m_child = null;
    private Parent m_parent = null;
    private Grandpa m_grandpa = null;
    private int m_money = 0;
    private bool m_accepted = false;

    public Requirement(bool child, bool parent, bool grandpa, bool money, bool accept)
    {
        this.m_childNeeded = child;
        this.m_parentNeeded = parent;
        this.m_grandpaNeeded = grandpa;
        this.m_moneyNeeded = money;
        this.m_acceptRejectNeeded = accept;
    }
    public Parent Parent
    {
        get { return this.m_parent; }
        set { this.m_parent = value; }
    }

    public Child Child
    {
        get { return this.m_child; }
        set { this.m_child = value; }
    }

    public Grandpa Grandpa
    {
        get { return this.m_grandpa; }
        set { this.m_grandpa = value; }
    }

    public int Money
    {
        get { return this.m_money; }
        set { this.m_money = value; }
    }

    public bool Accept
    {
        get { return this.m_accepted; }
        set { this.m_accepted = value; }
    }
}