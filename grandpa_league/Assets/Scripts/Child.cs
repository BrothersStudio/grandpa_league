using System.Collections;
using System;

[Serializable]
public class Child : Character
{
    private     Stat    m_cuteness              = new Stat();
    private     Stat    m_intelligence          = new Stat();
    private     Stat    m_artistry              = new Stat();
    private     Stat    m_athleticism           = new Stat();
    private     Stat    m_popularity            = new Stat();

    private     double  m_value                   = 0;

	public Child(string name, int gender, int age, int cuteness, int intelligence, int artistry, int athleticism, int popularity)
    {
        this.m_name = name;
        this.m_gender = gender;
        this.m_age = age;
        this.m_cuteness.Value = cuteness;
        this.m_intelligence.Value = intelligence;
		this.m_artistry.Value = artistry;
		this.m_athleticism.Value = athleticism;
		this.m_popularity.Value = popularity;

        this.m_cuteness.GrowthRate = (-1 * cuteness / Constants.Character.GROWTH_DIVIDER) > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : (cuteness / Constants.Character.GROWTH_DIVIDER);
        this.m_intelligence.GrowthRate = (intelligence / Constants.Character.GROWTH_DIVIDER) > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : (intelligence / Constants.Character.GROWTH_DIVIDER);
        this.m_artistry.GrowthRate = (artistry / Constants.Character.GROWTH_DIVIDER) > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : (artistry / Constants.Character.GROWTH_DIVIDER);
        this.m_athleticism.GrowthRate = (athleticism / Constants.Character.GROWTH_DIVIDER) > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : (athleticism / Constants.Character.GROWTH_DIVIDER);
        this.m_popularity.GrowthRate = (-1 * popularity / Constants.Character.GROWTH_DIVIDER) > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : (-1 * popularity / Constants.Character.GROWTH_DIVIDER);

        this.UpdateValue();
    }

    public override void UpgradeRandomStat(double amt)
    {
        base.UpgradeRandomStat(amt);

        int random = Constants.RANDOM.Next(0, 4);
        switch (random)
        {
            case 0:
                this.m_cuteness.Value += amt;
                break;
            case 1:
                this.m_intelligence.Value += amt;
                break;
            case 2:
                this.m_artistry.Value += amt;
                break;
            case 3:
                this.m_athleticism.Value += amt;
                break;
            case 4:
                this.m_popularity.Value += amt;
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
                this.m_cuteness.GrowthRate += amt;
                break;
            case 1:
                this.m_intelligence.GrowthRate += amt;
                break;
            case 2:
                this.m_artistry.GrowthRate += amt;
                break;
            case 3:
                this.m_athleticism.GrowthRate += amt;
                break;
            case 4:
                this.m_popularity.GrowthRate += amt;
                break;
        }
    }

    public void UpdateValue()
    {
        this.m_value = (this.m_cuteness.Value + this.m_intelligence.Value + this.m_artistry.Value + this.m_athleticism.Value + this.m_popularity.Value);
    }
    
    public double Cuteness
    {
        get { return this.m_cuteness.Value;  }
        set { this.m_cuteness.Value = value > 100 ? 100 : value; }
    }

    public double CutenessGrowth
    {
        get { return this.m_cuteness.GrowthRate; }
        set { this.m_cuteness.GrowthRate = value; }
    }

    public double Intelligence
    {
        get { return this.m_intelligence.Value; }
        set { this.m_intelligence.Value = value > 100 ? 100 : value; }
    }

    public double IntelligenceGrowth
    {
        get { return this.m_intelligence.GrowthRate; }
        set { this.m_intelligence.GrowthRate = value > 100 ? 100 : value; }
    }

    public double Artistry
    {
		get { return this.m_artistry.Value; }
		set { this.m_artistry.Value = value > 100 ? 100 : value; }
    }

    public double ArtistryGrowth
    {
		get { return this.m_artistry.GrowthRate; }
		set { this.m_artistry.GrowthRate = value; }
    }

    public double Athleticism
    {
		get { return this.m_athleticism.Value; }
		set { this.m_athleticism.Value = value > 100 ? 100 : value; }
    }

	public double AthleticismGrowth
    {
        get { return this.m_athleticism.GrowthRate; }
		set { this.m_athleticism.GrowthRate = value; }
    }

    public double Popularity
    {
        get { return this.m_popularity.Value; }
		set { this.m_popularity.Value = value > 100 ? 100 : value; }
    }

    public double PopularityGrowth
    {
		get { return this.m_popularity.GrowthRate; }
		set { this.m_popularity.GrowthRate = value; }
    }

    public double Value
    {
        get { this.UpdateValue();  return this.m_value; }
        set { this.m_value = value; }
    }
}