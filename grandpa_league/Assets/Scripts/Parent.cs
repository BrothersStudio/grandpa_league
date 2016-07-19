using System.Collections;
using System;

public class Parent : Character
{
    private int     m_intelligence  = 0;
	private int     m_popularity    = 0;
	private int     m_love          = 0;

    private double  m_intelligenceGrowth  = 0;
	private double  m_popularityGrowth    = 0;
	private double  m_loveGrowth          = 0;

	public Parent(string name, string gender, string age, string intelligence, string popularity, string love)
    {
        this.m_name = name;
        this.m_gender = Int32.Parse(gender);
        this.m_age = Int32.Parse(age);
        this.m_intelligence = Int32.Parse(intelligence);
		this.m_popularity = Int32.Parse(popularity);
		this.m_love = Int32.Parse(love);

        this.m_intelligenceGrowth = (this.m_intelligence / 300);
		this.m_popularityGrowth = (this.m_popularity / 300);
		this.m_loveGrowth = (this.m_love / 300);
    }

    public int Intelligence
    {
        get { return this.m_intelligence; }
        set { this.m_intelligence = value; }
    }

    public double IntelligenceGrowth
    {
        get { return this.m_intelligenceGrowth; }
		set { this.m_intelligenceGrowth = value; }
    }
		
	public int Popularity
	{
		get { return this.m_popularity; }
		set { this.m_popularity = value; }
	}

	public double PopularityGrowth
	{
		get { return this.m_popularityGrowth; }
		set { this.m_popularityGrowth = value; }
	}

	public int Love
	{
		get { return this.m_love; }
		set { this.m_love = value; }
	}

	public double LoveGrowth
	{
		get { return this.m_loveGrowth; }
		set { this.m_loveGrowth = value; }
	}
}