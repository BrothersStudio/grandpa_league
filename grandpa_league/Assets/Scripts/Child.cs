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

	public Child(string name, string gender, string age, string cuteness, string intelligence, string artistry, string athleticism, string popularity)
    {
        this.m_name = name;
        this.m_gender = Int32.Parse(gender);
        this.m_age = Int32.Parse(age);
        this.m_cuteness = Int32.Parse(cuteness);
        this.m_intelligence = Int32.Parse(intelligence);
		this.m_artistry = Int32.Parse(artistry);
		this.m_athleticism = Int32.Parse(athleticism);
		this.m_popularity = Int32.Parse(popularity);

        this.m_cutenessGrowth = -1 * this.m_cuteness / 300;
        this.m_intelligenceGrowth = this.m_intelligence / 300;
		this.m_artistryGrowth = this.m_artistry / 300;
		this.m_athleticismGrowth = this.m_athleticism / 300;
		this.m_popularityGrowth = -1 * this.m_popularity / 300;
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
}