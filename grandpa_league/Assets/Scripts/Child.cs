using System.Collections;
using System;

public class Child : Character
{
    private     int     m_cuteness              = 0;
    private     int     m_intelligence          = 0;
    private     int     m_artistry              = 0;
    private     int     m_athleticism           = 0;
    private     int     m_popularity            = 0;

    private     double  m_cutenessGrowth        = 0;
    private     double  m_intelligenceGrowth    = 0;
	private     double  m_artistryGrowth        = 0;
    private     double  m_athleticismGrowth     = 0;
    private     double  m_popularityGrowth      = 0;

    private     int     m_value                   = 0;

	public Child(string name, int gender, int age, int cuteness, int intelligence, int artistry, int athleticism, int popularity)
    {
        this.m_name = name;
        this.m_gender = gender;
        this.m_age = age;
        this.m_cuteness = cuteness;
        this.m_intelligence = intelligence;
		this.m_artistry = artistry;
		this.m_athleticism = athleticism;
		this.m_popularity = popularity;

        this.m_cutenessGrowth = -1 * this.m_cuteness / 300;
        this.m_intelligenceGrowth = this.m_intelligence / 300;
		this.m_artistryGrowth = this.m_artistry / 300;
		this.m_athleticismGrowth = this.m_athleticism / 300;
		this.m_popularityGrowth = -1 * this.m_popularity / 300;

        this.m_value = (this.m_cuteness + this.m_intelligence + this.m_artistry + this.m_athleticism + this.m_popularity) 
            * (int)(this.m_cutenessGrowth + this.m_intelligenceGrowth + this.m_artistryGrowth + this.m_athleticismGrowth + this.m_popularityGrowth);
}
    
    public int Cuteness
    {
        get { return this.m_cuteness;  }
        set { this.m_cuteness = value; }
    }

    public double CutenessGrowth
    {
        get { return this.m_cutenessGrowth; }
        set { this.m_cutenessGrowth = value; }
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

    public int Artistry
    {
		get { return this.m_artistry; }
		set { this.m_artistry = value; }
    }

    public double ArtistryGrowth
    {
		get { return this.m_artistryGrowth; }
		set { this.m_artistryGrowth = value; }
    }

    public int Athleticism
    {
		get { return this.m_athleticism; }
		set { this.m_athleticism = value; }
    }

	public double AthleticismGrowth
    {
        get { return this.m_athleticismGrowth; }
		set { this.m_athleticismGrowth = value; }
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

    public int Value
    {
        get { return this.m_value; }
        set { this.m_value = value; }
    }
}