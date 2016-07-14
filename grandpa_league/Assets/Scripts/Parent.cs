using UnityEngine;
using System.Collections;
using System;

public class Parent : Character
{
    private int     m_wealth        = 0;
    private int     m_intelligence  = 0;

    public Parent(string name, string gender, string age, string wealth, string intelligence)
    {
        this.m_name = name;
        this.m_gender = Int32.Parse(gender);
        this.m_age = Int32.Parse(age);
        this.m_wealth = Int32.Parse(wealth);
        this.m_intelligence = Int32.Parse(intelligence);
    }

    public int Wealth
    {
        get { return this.m_wealth; }
        set { this.m_wealth = value; }
    }
    public int Intelligence
    {
        get { return this.m_intelligence; }
        set { this.m_intelligence = value; }
    }
}