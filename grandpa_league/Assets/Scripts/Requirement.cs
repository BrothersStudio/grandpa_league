using System;
using System.Collections.Generic;

[Serializable]
public class Requirement
{
    //set by what is read in from events.xml
    private bool m_childNeeded = false;
    private bool m_parentNeeded = false;
    private bool m_grandpaNeeded = false;
    private bool m_moneyNeeded = false;
    private bool m_acceptRejectNeeded = false;

    private bool m_randomChild = false;
    private bool m_randomParent = false;
    private bool m_randomGrandpa = false;

    private int m_qualification = 0;
    private int m_minAge = 0;
    private int m_maxAge = 1000;

    private List<Trade> m_trade = new List<Trade>();

    //set by user's choices
    private Child m_child = null;
    private Parent m_parent = null;
    private Grandpa m_grandpa = null;
    private int m_money = 0;
    private bool m_accepted = false;

    public Requirement(bool child, bool parent, bool grandpa, bool money, bool accept, bool randomChild, bool randomParent, bool randomGrandpa, int qualification, string age)
    {
        this.m_childNeeded = child;
        this.m_parentNeeded = parent;
        this.m_grandpaNeeded = grandpa;
        this.m_moneyNeeded = money;
        this.m_acceptRejectNeeded = accept;

        this.m_randomChild = randomChild;
        this.m_randomParent = randomParent;
        this.m_randomGrandpa = randomGrandpa;

        this.m_qualification = qualification;

        if (age != null)
        {
            var split = age.Split('-');
            this.m_minAge = Int32.Parse(split[0]);
            this.m_maxAge = split.Length == 1 ? 1000 : Int32.Parse(split[1]);
        }
    }

    public bool HasInputRequirements()
    {
        return this.m_moneyNeeded | this.m_acceptRejectNeeded | (this.m_grandpaNeeded & !this.m_randomGrandpa) |
                (this.m_childNeeded & !this.m_randomChild) | (this.m_parentNeeded & !this.m_randomParent);
    }

    public Parent Parent
    {
        get { return this.m_parent; }
        set { this.m_parent = value; }
    }

    public bool ReqParent
    {
        get { return this.m_parentNeeded; }
    }

    public bool RandomParent
    {
        get { return this.m_randomParent; }
    }

    public Child Child
    {
        get { return this.m_child; }
        set { this.m_child = value; }
    }

    public bool ReqChild
    {
        get { return this.m_childNeeded; }
    }

    public bool RandomChild
    {
        get { return this.m_randomChild; }
    }

    public Grandpa Grandpa
    {
        get { return this.m_grandpa; }
        set { this.m_grandpa = value; }
    }

    public bool ReqGrandpa
    {
        get { return this.m_grandpaNeeded;  }
    }

    public bool RandomGrandpa
    {
        get { return this.m_randomGrandpa; }
    }

    public int Money
    {
        get { return this.m_money; }
        set { this.m_money = value < 0 ? 0 : value; }
    }

    public bool ReqMoney
    {
        get { return this.m_moneyNeeded; }
    }


    public bool Accept
    {
        get { return this.m_accepted; }
        set { this.m_accepted = value; }
    }

    public bool ReqAccept
    {
        get { return this.m_acceptRejectNeeded; }
    }

    public int Qualification
    {
        get { return this.m_qualification; }
    }

    public int MinAge
    {
        get { return this.m_minAge; }
    }

    public int MaxAge
    {
        get { return this.m_maxAge; }
    }

    public List<Trade> Trade
    {
        get { return this.m_trade; }
        set { this.m_trade = value; }
    }
}