using System.Collections;
using System;

public class Child : Character
{
    private     double   m_cuteness              = 0;
    private     double   m_intelligence          = 0;
    private     double   m_artistry              = 0;
    private     double   m_athleticism           = 0;
    private     double   m_popularity            = 0;

    private     double  m_cutenessGrowth        = 0;
    private     double  m_intelligenceGrowth    = 0;
	private     double  m_artistryGrowth        = 0;
    private     double  m_athleticismGrowth     = 0;
    private     double  m_popularityGrowth      = 0;

    private     double  m_value                   = 0;

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

        this.UpdateValue();
    }

    public override void UpgradeRandomStat(double amt)
    {
        base.UpgradeRandomStat(amt);

        int random = Constants.RANDOM.Next(0, 4);
        switch (random)
        {
            case 0:
                this.m_cuteness += amt;
                break;
            case 1:
                this.m_intelligence += amt;
                break;
            case 2:
                this.m_artistry += amt;
                break;
            case 3:
                this.m_athleticism += amt;
                break;
            case 4:
                this.m_popularity += amt;
                break;
        }
    }

    public override void UpgradeRandomStatGrowth(double amt)
    {
        base.UpgradeRandomStatGrowth(amt);

        int random = Constants.RANDOM.Next(0, 4);
        switch (random)
        {
            case 0:
                this.m_cutenessGrowth += amt;
                break;
            case 1:
                this.m_intelligenceGrowth += amt;
                break;
            case 2:
                this.m_artistryGrowth += amt;
                break;
            case 3:
                this.m_athleticismGrowth += amt;
                break;
            case 4:
                this.m_popularityGrowth += amt;
                break;
        }
    }

    public void UpdateValue()
    {
        this.m_value = (this.m_cuteness + this.m_intelligence + this.m_artistry + this.m_athleticism + this.m_popularity);
    }
    
    public double Cuteness
    {
        get { return this.m_cuteness;  }
        set { this.m_cuteness = value > 100 ? 100 : value; }
    }

    public double CutenessGrowth
    {
        get { return this.m_cutenessGrowth; }
        set { this.m_cutenessGrowth = value; }
    }

    public double Intelligence
    {
        get { return this.m_intelligence; }
        set { this.m_intelligence = value > 100 ? 100 : value; }
    }

    public double IntelligenceGrowth
    {
        get { return this.m_intelligenceGrowth; }
        set { this.m_intelligenceGrowth = value > 100 ? 100 : value; }
    }

    public double Artistry
    {
		get { return this.m_artistry; }
		set { this.m_artistry = value > 100 ? 100 : value; }
    }

    public double ArtistryGrowth
    {
		get { return this.m_artistryGrowth; }
		set { this.m_artistryGrowth = value; }
    }

    public double Athleticism
    {
		get { return this.m_athleticism; }
		set { this.m_athleticism = value > 100 ? 100 : value; }
    }

	public double AthleticismGrowth
    {
        get { return this.m_athleticismGrowth; }
		set { this.m_athleticismGrowth = value; }
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

    public double Value
    {
        get { this.UpdateValue();  return this.m_value; }
        set { this.m_value = value; }
    }
}