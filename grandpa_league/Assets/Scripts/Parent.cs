using System.Collections;
using System;

[Serializable]
public class Parent : Character
{
    private Stat m_intelligence          = new Stat();
    private Stat m_popularity            = new Stat();
	private Stat m_love                  = new Stat();


    private double  m_value                   = 0;

	public Parent(string name, int gender, int age, int intelligence, int popularity, int love)
    {
        this.m_name = name;
        this.m_gender = gender;
        this.m_age = age;
        this.m_intelligence.Value = intelligence;
		this.m_popularity.Value = popularity;
		this.m_love.Value = love;

        this.m_intelligence.GrowthRate = (intelligence / Constants.Character.GROWTH_DIVIDER) > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : (intelligence / Constants.Character.GROWTH_DIVIDER);
        this.m_popularity.GrowthRate = (-1 * popularity / Constants.Character.GROWTH_DIVIDER) > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : (-1 * popularity / Constants.Character.GROWTH_DIVIDER);
        this.m_love.GrowthRate = (popularity / Constants.Character.GROWTH_DIVIDER) > Constants.Character.MAX_INITIAL_GROWTH ? Constants.Character.MAX_INITIAL_GROWTH : (love / Constants.Character.GROWTH_DIVIDER);

        this.UpdateValue();
    }

    public override void UpgradeRandomStat(double amt)
    {
        base.UpgradeRandomStat(amt);

        int random = Constants.RANDOM.Next(0, 2);
        switch (random)
        {
            case 0:
                this.m_love.Value += amt;
                break;
            case 1:
                this.m_intelligence.Value += amt;
                break;
            case 2:
                this.m_popularity.Value += amt;
                break;
        }
    }

    public override void UpgradeRandomStatGrowth(double amt)
    {
        base.UpgradeRandomStatGrowth(amt);

        int random = Constants.RANDOM.Next(0, 2);
        switch (random)
        {
            case 0:
                this.m_love.GrowthRate += amt;
                break;
            case 1:
                this.m_intelligence.GrowthRate += amt;
                break;
            case 2:
                this.m_popularity.GrowthRate += amt;
                break;
        }
    }

    public void UpdateValue()
    {
        this.m_value = (this.m_intelligence.Value + this.m_popularity.Value + this.m_love.Value);
    }

    public double Intelligence
    {
        get { return this.m_intelligence.Value; }
        set { this.m_intelligence.Value = value > 100 ? 100 : value; }
    }

    public double IntelligenceGrowth
    {
        get { return this.m_intelligence.GrowthRate; }
		set { this.m_intelligence.GrowthRate = value; }
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

	public double Love
	{
		get { return this.m_love.Value; }
		set { this.m_love.Value = value > 100 ? 100 : value; }
	}

	public double LoveGrowth
	{
		get { return this.m_love.GrowthRate; }
		set { this.m_love.GrowthRate = value; }
	}

    public double Value
    {
        get { return this.m_value; }
        set { this.m_value = value; }
    }
}