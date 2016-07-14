using UnityEngine;
using System.Collections;
using System;

public class Grandpa : Character
{
    private int     m_insanity  = 0;
    private int     m_wisdom    = 0;
    private double  m_money     = 100.00;
    
	public Grandpa(string name, string age, string wisdom, string insanity)
	{
        this.m_name = name;
        this.m_age = Int32.Parse(age);
        this.m_wisdom = Int32.Parse(wisdom);
        this.m_insanity = Int32.Parse(insanity);
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

    public int Insanity
    {
        get { return this.m_insanity; }
        set { this.m_insanity = value; }
    }

    public double Money
    {
        get { return this.m_money; }
        set { this.m_money = value; }
    }
}