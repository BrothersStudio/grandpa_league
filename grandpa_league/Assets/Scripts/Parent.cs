using System.Collections;
using System;

public class Parent : Character
{
    private double m_intelligence          = 0;
	private double m_popularity            = 0;
	private double m_love                  = 0;

    private double  m_intelligenceGrowth    = 0;
	private double  m_popularityGrowth      = 0;
	private double  m_loveGrowth            = 0;

    private double  m_value                   = 0;

	public Parent(string name, int gender, int age, int intelligence, int popularity, int love)
    {
        this.m_name = name;
        this.m_gender = gender;
        this.m_age = age;
        this.m_intelligence = intelligence;
		this.m_popularity = popularity;
		this.m_love = love;

        this.m_intelligenceGrowth = (this.m_intelligence / 300);
		this.m_popularityGrowth = (this.m_popularity / 300);
		this.m_loveGrowth = (this.m_love / 300);

        this.UpdateValue();
    }

    public void UpdateValue()
    {
        this.m_value = (this.m_intelligence + this.m_popularity + this.m_love);
    }

    public double Intelligence
    {
        get { return this.m_intelligence; }
        set { this.m_intelligence = value > 100 ? 100 : value; }
    }

    public double IntelligenceGrowth
    {
        get { return this.m_intelligenceGrowth; }
		set { this.m_intelligenceGrowth = value; }
    }
		
	public double Popularity
	{
		get { return this.m_popularity; }
		set { this.m_popularity = value > 100 ? 100 : value; }
	}

	public double PopularityGrowth
	{
		get { return this.m_popularityGrowth; }
		set { this.m_popularityGrowth = value; }
	}

	public double Love
	{
		get { return this.m_love; }
		set { this.m_love = value > 100 ? 100 : value; }
	}

	public double LoveGrowth
	{
		get { return this.m_loveGrowth; }
		set { this.m_loveGrowth = value; }
	}

    public double Value
    {
        get { return this.m_value; }
        set { this.m_value = value; }
    }
}