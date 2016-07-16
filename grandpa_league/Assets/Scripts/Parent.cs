using System.Collections;
using System;

public class Parent : Character
{
    private int     m_wealth        = 0;
    private int     m_intelligence  = 0;

    private double  m_wealthGrowth  = 0;
    private double  m_intGrowth     = 0;

    public Parent(string name, string gender, string age, string wealth, string intelligence)
    {
        this.m_name = name;
        this.m_gender = Int32.Parse(gender);
        this.m_age = Int32.Parse(age);
        this.m_wealth = Int32.Parse(wealth);
        this.m_intelligence = Int32.Parse(intelligence);

        this.m_wealthGrowth = (this.m_wealth / 300);
        this.m_intGrowth = (this.m_intelligence / 300);
    }

    public int Wealth
    {
        get { return this.m_wealth; }
        set { this.m_wealth = value; }
    }

    public double WealthGrowth
    {
        get { return this.m_wealthGrowth; }
        set { this.m_wealthGrowth = value; }
    }

    public int Intelligence
    {
        get { return this.m_intelligence; }
        set { this.m_intelligence = value; }
    }

    public double IntelligenceGrowth
    {
        get { return this.m_intGrowth; }
        set { this.m_intGrowth = value; }
    }
}