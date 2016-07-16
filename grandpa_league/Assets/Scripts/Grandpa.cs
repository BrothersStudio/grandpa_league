using System;

public class Grandpa : Character
{
    private int     m_insanity          = 0;
    private int     m_wisdom            = 0;
    private double  m_money             = 100.00;
    private int     m_pride             = 0;            //currently pride should only apply to PlayerCharacter but we can keep it here

    private double  m_insanityGrowth    = 0;
    private double  m_wisdomGrowth      = 0;
    
	public Grandpa(string name, string age, string wisdom, string insanity)
	{
        this.m_name = name;
        this.m_age = Int32.Parse(age);
        this.m_wisdom = Int32.Parse(wisdom);
        this.m_insanity = Int32.Parse(insanity);

        this.m_insanityGrowth = -1 * (this.m_insanity / 300);
        this.m_wisdomGrowth = (this.m_wisdom / 300);
	}

    public Grandpa(string name)
    {
        this.m_name = name;
    }

    public int Wisdom
    {
        get { return this.m_wisdom; }
        set { this.m_wisdom = value; }
    }

    public double WisdomGrowth
    {
        get { return this.m_wisdomGrowth; }
        set { this.m_wisdomGrowth = value; }
    }

    public int Insanity
    {
        get { return this.m_insanity; }
        set { this.m_insanity = value; }
    }

    public double InsanityGrowth
    {
        get { return this.m_insanityGrowth; }
        set { this.m_insanityGrowth = value; }
    }

    public double Money
    {
        get { return this.m_money; }
        set { this.m_money = value; }
    }

    public int Pride
    {
        get { return this.m_pride; }
        set { this.m_pride = value; }
    }
}